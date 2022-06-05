using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using RedHouseLauncher.Core.Settings;

namespace RedHouseLauncher.UI.Views.Components
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            GamePathBox.Text = Settings.PathToSkyrim;
        }

        #region Путь к игре

        private void OpenGamePathDialogue(object sender, MouseButtonEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new())
            {
                _ = folderBrowserDialog.ShowDialog();

                if (Path.IsPathRooted(folderBrowserDialog.SelectedPath))
                {
                    GamePathBox.Text = folderBrowserDialog.SelectedPath;
                }
            }

            SetGamePath(null, null);
        }

        private async void SetGamePath(object? sender, TextChangedEventArgs? e)
        {
            try
            {
                if (GamePathBox == null || !Directory.Exists(GamePathBox.Text))
                {
                    return;
                }

                Settings.PathToSkyrim = GamePathBox.Text;
                await Settings.Save();

            }
            catch { }
        }

        #endregion
    }
}
