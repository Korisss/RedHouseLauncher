using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Images.Icons
{
    /// <summary>
    /// Interaction logic for FolderIcon.xaml
    /// </summary>
    public partial class FolderIcon
    {
        public FolderIcon()
        {
            InitializeComponent();
        }

        private void AddHighlight(object sender, MouseEventArgs e)
        {
            Path1.Fill = Brushes.White;
            Path2.Fill = Brushes.White;
        }

        private void RemoveHighlight(object sender, MouseEventArgs e)
        {
            object? colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            Color unselectedColor = (Color)colorConverter;

            Path1.Fill = new SolidColorBrush(unselectedColor);
            Path2.Fill = new SolidColorBrush(unselectedColor);
        }
    }
}
