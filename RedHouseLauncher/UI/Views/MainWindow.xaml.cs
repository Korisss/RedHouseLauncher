using RedHouseLauncher.Core.Auth;
using RedHouseLauncher.Core.Settings;
using RedHouseLauncher.UI.Views.Components;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        internal static MainWindow? MainWindowStatic;
        internal static ModsPage? ModsPageStatic;

        public MainWindow()
        {
            MainWindowStatic = this;

            InitializeComponent();

            #region Ник

            _ = Task.Run(async () =>
            {
                string? nickname = "Error";

                try
                {
                    nickname = await AccountWorker.GetLogin();
                }
                catch
                {
                    // ignored
                }

                Dispatcher.Invoke(() => PlayerName.Content = nickname ?? "Error");
            });

            #endregion
        }

        #region Бинды

        private async void CheckBinds(object sender, KeyEventArgs e)
        {
            if (ServerListTab == null || ServerListTab.Visibility != Visibility.Visible)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    ServerListTab.StartGame(sender: null, e: null);
                    break;
                case Key.F5:
                    await ServerListTab.UpdateServerList();
                    break;
                case Key.R when Keyboard.IsKeyDown(Key.LeftCtrl):
                    await ServerListTab.UpdateServerList();
                    break;
            }
        }

        #endregion

        #region Тулбар

        #region Вкладки

        private void SetMainTab(object sender, MouseButtonEventArgs e)
        {
            HideAllTabs();

            ServerListTab.Visibility = Visibility.Visible;
            MainTabLabel.Foreground = Brushes.White;
            MainTabLine.Visibility = Visibility.Visible;
        }

        private void SetSettingsTab(object sender, MouseButtonEventArgs e)
        {
            HideAllTabs();

            SettingsTab.Visibility = Visibility.Visible;
            SettingsTabIcon.IsClicked = true;
            SettingsTabIcon.AddHighlightSettingsIcon(null, null);
            SettingsTabLine.Visibility = Visibility.Visible;
        }

        private async void SetModsTab(object sender, MouseButtonEventArgs e)
        {
            HideAllTabs();

            if (ModsPageStatic == null)
            {
                return;
            }

            await ModsPageStatic.UpdateList();

            ModsTab.Visibility = Visibility.Visible;
            ModsTabIcon.IsClicked = true;
            ModsTabIcon.AddHighlightModsIcon(null, null);
            ModsTabLine.Visibility = Visibility.Visible;
        }

        private void HideAllTabs()
        {
            object? colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            SolidColorBrush unselectedColor = new SolidColorBrush((Color)colorConverter);

            ServerListTab.Visibility = Visibility.Hidden;
            MainTabLine.Visibility = Visibility.Hidden;
            MainTabLabel.Foreground = unselectedColor;

            SettingsTab.Visibility = Visibility.Hidden;
            SettingsTabLine.Visibility = Visibility.Hidden;
            SettingsTabIcon.IsClicked = false;
            SettingsTabIcon.RemoveHighlightSettingsIcon(null, null);

            ModsTab.Visibility = Visibility.Hidden;
            ModsTabLine.Visibility = Visibility.Hidden;
            ModsTabIcon.IsClicked = false;
            ModsTabIcon.RemoveHighlightModsIcon(null, null);
        }

        #endregion

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region Выход из аккаунта

        private void HideUnHideLogoutButton(object sender, MouseButtonEventArgs e)
        {
            if (LogoutButton.Visibility == Visibility.Hidden)
            {
                LogoutButton.Visibility = Visibility.Visible;
                UserInfoIcon.LayoutTransform = new RotateTransform(180);
            }
            else
            {
                LogoutButton.Visibility = Visibility.Hidden;
                UserInfoIcon.LayoutTransform = new RotateTransform(0);
            }
        }

        private async void Logout(object sender, MouseButtonEventArgs e)
        {
            Settings.UserId = -1;
            Settings.UserToken = "";
            Settings.UserName = "";

            await Settings.Save();

            AuthWindow login = new AuthWindow();

            Close();
            login.Show();
        }

        #endregion
    }
}
