﻿using RedHouseLauncher.Core.Settings;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using UserControl = System.Windows.Controls.UserControl;

namespace RedHouseLauncher.UI.Views.Components
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();

            AuthServerBox.Text = Settings.MasterServer;
            GamePathBox.Text = Settings.PathToSkyrim;
        }

        #region Сервер авторизации

        private async void SetAuthServer(object sender, TextChangedEventArgs e)
        {
            if (AuthServerBox == null || !IsUriValid(AuthServerBox.Text))
            {
                return;
            }

            Settings.MasterServer = AuthServerBox.Text;
            await Settings.Save();
        }

        #endregion

        #region Доп функции, которые надо вынести

        private static bool IsUriValid(string uri)
        {
            try
            {
                return new Uri(uri).IsWellFormedOriginalString();
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        #endregion

        #region Путь к игре

        private void OpenGamePathDialogue(object sender, MouseButtonEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.ShowDialog();

                if (Path.IsPathRooted(folderBrowserDialog.SelectedPath))
                {
                    GamePathBox.Text = folderBrowserDialog.SelectedPath;
                }
            }

            SetGamePath(null, null);
        }

        private async void SetGamePath(object? sender, TextChangedEventArgs? e)
        {
            if (GamePathBox == null || !IsUriValid(GamePathBox.Text))
            {
                return;
            }

            Settings.PathToSkyrim = GamePathBox.Text;
            await Settings.Save();
        }

        #endregion
    }
}