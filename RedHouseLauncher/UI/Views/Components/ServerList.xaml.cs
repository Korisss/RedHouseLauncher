using RedHouseLauncher.Core.Auth;
using RedHouseLauncher.Core.GameUtils;
using RedHouseLauncher.Core.Models;
using RedHouseLauncher.Core.Settings;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Views.Components
{
    /// <summary>
    /// Interaction logic for ServerList.xaml
    /// </summary>
    public partial class ServerList : UserControl
    {
        public ServerList()
        {
            InitializeComponent();

            _ = UpdateServerListEvery10Seconds();
        }

        #region Выбор сервера

        private async void SelectServer(object sender, SelectionChangedEventArgs e)
        {
            ServerListItemModel selectedServer = (ServerListItemModel)ServerListView.SelectedItem;

            if (selectedServer == null)
            {
                return;
            }

            ServerIcon.Source = selectedServer.ServerIcon;
            ServerName.Content = selectedServer.Name;
            ServerOnline.Content = selectedServer.Online;

            ServerDescription.Text =
                await selectedServer.GetDescription();
        }

        #endregion

        #region Нажатие кнопки играть

        internal async void StartGame(object sender, MouseButtonEventArgs e)
        {
            ServerListItemModel selectedServer = (ServerListItemModel)MainWindow.MainWindowStatic.ServerListTab.ServerListView.SelectedItem;

            if (selectedServer == null)
            {
                MessageBox.Show("Выберите сервер.");
                return;
            }

            if (GameChecker.IsChecking)
            {
                MessageBox.Show("Идёт проверка игры, подождите.");
                return;
            }

            if (!await GameChecker.CheckGame(MainWindow.MainWindowStatic.ServerListTab.Progressbar))
            {
                return;
            }

            SkyMPSettings skyMPSettings = await SkyMPSettings.Load();

            skyMPSettings.ServerIp = selectedServer.Ip;
            skyMPSettings.ServerPort = selectedServer.Port;

            try
            {
                skyMPSettings.GameData = await AccountWorker.GetSession(skyMPSettings.ServerIp + ':' + skyMPSettings.ServerPort);
                await skyMPSettings.Save();
            }
            catch (Exception err)
            {
                MessageBox.Show($"Ошибка во время получения сессии.\n\n{err}");
                return;
            }

            try
            {
                await ServerFilesDownloader.DownloadFiles(
                    $"http://{skyMPSettings.ServerIp}:{skyMPSettings.ServerPort + 1}/",
                    MainWindow.MainWindowStatic.ServerListTab.Progressbar);
            }
            catch (Exception err)
            {
                MessageBox.Show($"Ошибка во время загрузки модификаций сервера.\n\n{err}");
                return;
            }

            try
            {
                await GameUtils.StartGame(MainWindow.MainWindowStatic);
            }
            catch (Exception err)
            {
                MessageBox.Show($"Ошибка во время запуска игры.\n\n{err}");
            }
        }

        #endregion

        #region Обновление списка

        private async Task UpdateServerListEvery10Seconds()
        {
            await Task.Delay(400);
            while (true)
            {
                await UpdateServerList();
                await Task.Delay(10000);
            }
        }

        internal async Task UpdateServerList()
        {
            if (GameUtils.IsGameRunning)
            {
                return;
            }

            ServerListItemModel selectedServer = (ServerListItemModel)ServerListView.SelectedItem;

            ServerListItemModel[] serverListItems = await ServerListItemModel.GetServerListItemsAsync();

            Dispatcher.Invoke(() => ServerListView.Items.Clear());

            foreach (ServerListItemModel serverListItem in serverListItems)
            {
                serverListItem.UpdateIcon();
                Dispatcher.Invoke(() => ServerListView.Items.Add(serverListItem));
            }

            if (selectedServer != null)
            {
                foreach (ServerListItemModel serverListItem in ServerListView.Items)
                {
                    if (serverListItem.Ip == selectedServer.Ip && serverListItem.Port == selectedServer.Port)
                    {
                        ServerListView.SelectedItem = serverListItem;
                    }
                }
            }
            else
            {
                if (ServerListView.Items.Count > 0)
                {
                    ServerListView.SelectedIndex = 0;
                }
            }
        }

        #endregion

        #region Ховер для кнопки играть

        private void PlayButtonHoverEnable(object sender, MouseEventArgs e)
        {
            object colorConverter = ColorConverter.ConvertFromString("#D6D6D6");

            if (colorConverter == null)
            {
                return;
            }

            PlayButton.Background = new SolidColorBrush((Color)colorConverter);
        }

        private void PlayButtonHoverDisable(object sender, MouseEventArgs e)
        {
            PlayButton.Background = Brushes.White;
        }

        #endregion
    }
}
