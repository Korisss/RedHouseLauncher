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
    public partial class ServerList
    {
        public ServerList()
        {
            InitializeComponent();

            _ = UpdateServerListEvery10Seconds();
        }

        #region Выбор сервера

        private async void SelectServer(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ServerListItemModel selectedServer = (ServerListItemModel) ServerListView.SelectedItem;

                ServerIcon.Source = selectedServer.ServerIcon;
                ServerName.Content = selectedServer.Name;
                ServerOnline.Content = selectedServer.Online;

                ServerDescription.Text =
                    await selectedServer.GetDescription();
            }
            catch
            {
                //ignore
            }
        }

        #endregion

        #region Нажатие кнопки играть

        internal async void StartGame(object? sender, MouseButtonEventArgs? e)
        {
            if (MainWindow.MainWindowStatic == null)
            {
                return;
            }

            ServerListItemModel selectedServer = (ServerListItemModel)MainWindow.MainWindowStatic.ServerListTab.ServerListView.SelectedItem;

            if (GameChecker.IsChecking)
            {
                MessageBox.Show("Идёт проверка игры, подождите.");
                return;
            }

            if (!await GameChecker.CheckGame(MainWindow.MainWindowStatic.ServerListTab.Progressbar))
            {
                return;
            }

            SkyMpSettings? skyMpSettings = await SkyMpSettings.Load();

            if (skyMpSettings == null)
            {
                MessageBox.Show("Не загружены настройки мультиплеера.");
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
                MessageBox.Show($"Ошибка во время получения сессии.\n\n{err}");
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

            foreach (ServerListItemModel serverListItem in ServerListView.Items)
            {
                if (selectedServer != null && serverListItem.Ip == selectedServer.Ip && serverListItem.Port == selectedServer.Port)
                {
                    ServerListView.SelectedItem = serverListItem;
                }
            }
        }

        #endregion

        #region Ховер для кнопки играть

        private void PlayButtonHoverEnable(object sender, MouseEventArgs e)
        {
            object? colorConverter = ColorConverter.ConvertFromString("#D6D6D6");

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
