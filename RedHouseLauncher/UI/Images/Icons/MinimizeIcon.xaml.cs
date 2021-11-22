using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Images.Icons
{
    /// <summary>
    /// Interaction logic for MinimizeIcons.xaml
    /// </summary>
    public partial class MinimizeIcon : UserControl
    {
        public MinimizeIcon()
        {
            InitializeComponent();
        }

        private void AddHighlight(object sender, MouseEventArgs e)
        {
            MinimizeBrush.Brush = Brushes.White;
        }

        private void RemoveHighlight(object sender, MouseEventArgs e)
        {
            object colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            MinimizeBrush.Brush = new SolidColorBrush((Color)colorConverter);
        }
    }
}
