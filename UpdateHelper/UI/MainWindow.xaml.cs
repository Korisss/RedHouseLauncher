using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace UpdateHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Task.Run(async () =>
            {
                try
                {
                    await Progressbar.DownloadFile(
                        $"{Config.UpdaterUri}RedHouseLauncher.exe",
                        File.ReadAllText("RHLauncher_Path.txt"));
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Не удалось обновить лаунчер.\n\n{err}");
                    Application.Current.Shutdown();
                }
            });
        }

        #region Тулбар

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            }
            catch { }
        }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseApp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
