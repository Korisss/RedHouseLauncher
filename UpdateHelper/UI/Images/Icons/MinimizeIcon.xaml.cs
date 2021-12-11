using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UpdateHelper.UI.Images.Icons
{
    /// <summary>
    /// Interaction logic for MinimizeIcon.xaml
    /// </summary>
    public partial class MinimizeIcon : UserControl
    {
        public MinimizeIcon()
        {
            InitializeComponent();
        }
        private void AddHightlight(object sender, MouseEventArgs e)
        {
            MinimizeBrush.Brush = Brushes.White;
        }
        private void RemoveHightlight(object sender, MouseEventArgs e)
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
