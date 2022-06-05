using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public string Text { get; set; }

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
