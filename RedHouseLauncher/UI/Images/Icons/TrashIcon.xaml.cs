using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Images.Icons
{
    /// <summary>
    /// Interaction logic for TrashIcon.xaml
    /// </summary>
    public partial class TrashIcon : UserControl
    {
        public TrashIcon()
        {
            InitializeComponent();
        }

        private void AddHighlight(object sender, MouseEventArgs e)
        {
            Path1.Fill = Brushes.White;
            Path2.Fill = Brushes.White;
            Border1.Background = Brushes.White;
        }

        private void RemoveHighlight(object sender, MouseEventArgs e)
        {
            object colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            Brush unHoveredColor = new SolidColorBrush((Color)colorConverter);

            Path1.Fill = unHoveredColor;
            Path2.Fill = unHoveredColor;
            Border1.Background = unHoveredColor;
        }
    }
}
