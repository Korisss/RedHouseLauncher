using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RedHouseLauncher.UI.Views.Components
{
    /// <summary>
    /// Логика взаимодействия для WhiteButton.xaml
    /// </summary>
    public partial class WhiteButton : UserControl
    {
        public WhiteButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Text { get; set; } = default!;

        private void HoverEnable(object sender, MouseEventArgs e)
        {
            object? colorConverter = ColorConverter.ConvertFromString("#D6D6D6");

            if (colorConverter == null)
            {
                return;
            }

            Btn.Background = new SolidColorBrush((Color)colorConverter);
        }

        private void HoverDisable(object sender, MouseEventArgs e)
        {
            Btn.Background = Brushes.White;
        }

    }
}
