using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RedHouseLauncher.Core.Auth;
using RedHouseLauncher.Core.GameUtils;
using RedHouseLauncher.Core.Settings;
using RedHouseLauncher.UI.Models;
using RedHouseLauncher.UI.ViewModels;

namespace RedHouseLauncher.UI.Views.Components
{
    /// <summary>
    /// Interaction logic for ServerList.xaml
    /// </summary>
    public partial class ServerList
    {
        public ServerList()
        {
            InitializeComponent();
            _ = UpdateServerListEvery10Seconds();
        }

        #region Нажатие кнопки играть

        internal async void StartGame(object? sender, MouseButtonEventArgs? e)
        {
            if (MainWindow.MainWindowStatic == null)
            {
                return;
            }

            Server? selectedServer = (Server)MainWindow.MainWindowStatic.ServerListTab.ServerListView.SelectedItem;

            if (selectedServer == null)
            {
                return;
            }

            if (GameChecker.IsChecking)
            {
                _ = MessageBox.Show("Идёт проверка игры, подождите.");
                return;
            }

            if (!await GameChecker.CheckGame(MainWindow.MainWindowStatic.ServerListTab.Progressbar))
            {
                return;
            }

            SkyMpSettings? skyMpSettings = await SkyMpSettings.Load();

            if (skyMpSettings == null)
            {
                _ = MessageBox.Show("Не загружены настройки мультиплеера.");
                return;
            }

            skyMpSettings.ServerIp = selectedServer.Ip;
            skyMpSettings.ServerPort = selectedServer.Port;

            try
            {
                object? gameData = await AccountWorker.GetSession(skyMpSettings.ServerIp + ':' + skyMpSettings.ServerPort);
                skyMpSettings.GameData = gameData;
                await skyMpSettings.Save();
            }
            catch (Exception err)
            {
                _ = MessageBox.Show($"Ошибка во время получения сессии.\n\n{err}");
                return;
            }

            try
            {
                await ServerFilesDownloader.DownloadFiles(
                    $"http://{skyMpSettings.ServerIp}:{skyMpSettings.ServerPort + 1}/",
                    MainWindow.MainWindowStatic.ServerListTab.Progressbar);
            }
            catch (Exception err)
            {
                _ = MessageBox.Show($"Ошибка во время загрузки модификаций сервера.\n\n{err}");
                return;
            }

            try
            {
                await GameUtils.StartGame(MainWindow.MainWindowStatic);
            }
            catch (Exception err)
            {
                _ = MessageBox.Show($"Ошибка во время запуска игры.\n\n{err}");
            }
        }

        #endregion

        #region Обновление списка

        private async Task UpdateServerListEvery10Seconds()
        {
            while (true)
            {
                UpdateServerList();
                await Task.Delay(10000);
            }
        }

        internal void UpdateServerList()
        {
            if (GameUtils.IsGameRunning)
            {
                return;
            }

            Server selectedServer = (Server)ServerListView.SelectedItem;

            DataContext = new ServerListViewModel();

            foreach (Server serverListItem in ServerListView.Items)
            {
                if (selectedServer != null && serverListItem.Ip == selectedServer.Ip && serverListItem.Port == selectedServer.Port)
                {
                    ServerListView.SelectedItem = serverListItem;
                }
            }
        }

        #endregion
    }
}
