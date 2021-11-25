using RedHouseLauncher.Core.Settings;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Images.Icons
{
    /// <summary>
    /// Interaction logic for ExitIcon.xaml
    /// </summary>
    public partial class ExitIcon
    {
        public ExitIcon()
        {
            InitializeComponent();
        }

        private void AddHighlight(object sender, MouseEventArgs e)
        {
            LineBrush1.Brush = Brushes.White;
            LineBrush2.Brush = Brushes.White;
        }

        private void RemoveHighlight(object sender, MouseEventArgs e)
        {
            object? colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            LineBrush1.Brush = new SolidColorBrush((Color)colorConverter);
            LineBrush2.Brush = new SolidColorBrush((Color)colorConverter);
        }

        private async void CloseApp(object sender, MouseButtonEventArgs e)
        {
            await SkyMpSettings.DestroySession();
            Application.Current.Shutdown();
        }
    }
}
