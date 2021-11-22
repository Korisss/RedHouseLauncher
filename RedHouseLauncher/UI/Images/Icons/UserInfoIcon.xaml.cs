using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Images.Icons
{
    /// <summary>
    /// Interaction logic for UserInfoIcon.xaml
    /// </summary>
    public partial class UserInfoIcon : UserControl
    {
        public UserInfoIcon()
        {
            InitializeComponent();
        }

        private void AddHighlight(object sender, MouseEventArgs e)
        {
            LineBrush.Brush = Brushes.White;
        }

        private void RemoveHighlight(object sender, MouseEventArgs e)
        {
            object colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            LineBrush.Brush = new SolidColorBrush((Color)colorConverter);
        }
    }
}
