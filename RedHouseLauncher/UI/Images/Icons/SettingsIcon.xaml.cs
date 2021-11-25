using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Images.Icons
{
    /// <summary>
    /// Interaction logic for SettingsIcon.xaml
    /// </summary>
    public partial class SettingsIcon
    {
        public bool IsClicked = false;

        public SettingsIcon()
        {
            InitializeComponent();
        }

        public void AddHighlightSettingsIcon(object? sender, MouseEventArgs? e)
        {
            Drawer.Brush = Brushes.White;
        }

        public void RemoveHighlightSettingsIcon(object? sender, MouseEventArgs? e)
        {
            if (IsClicked)
            {
                return;
            }

            object? colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            Drawer.Brush = new SolidColorBrush((Color)colorConverter);
        }
    }
}
