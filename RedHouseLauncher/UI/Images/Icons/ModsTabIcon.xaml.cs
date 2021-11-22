using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Images.Icons
{
    /// <summary>
    /// Interaction logic for ModsTabIcon.xaml
    /// </summary>
    public partial class ModsTabIcon : UserControl
    {
        public bool IsClicked = false;

        public ModsTabIcon()
        {
            InitializeComponent();
        }

        public void AddHighlightModsIcon(object sender, MouseEventArgs e)
        {
            Path1.Fill = Brushes.White;
            Path2.Fill = Brushes.White;
        }

        public void RemoveHighlightModsIcon(object sender, MouseEventArgs e)
        {
            if (IsClicked)
            {
                return;
            }

            object colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            Brush unselectedColor = new SolidColorBrush((Color)colorConverter);
            Path1.Fill = unselectedColor;
            Path2.Fill = unselectedColor;
        }
    }
}
