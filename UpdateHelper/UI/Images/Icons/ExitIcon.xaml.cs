﻿using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UpdateHelper.UI.Images.Icons
{
    /// <summary>
    /// Interaction logic for ExitIcon.xaml
    /// </summary>
    public partial class ExitIcon : UserControl
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
            object colorConverter = ColorConverter.ConvertFromString("#8C8C8C");

            if (colorConverter == null)
            {
                return;
            }

            Color unselectedColor = (Color)colorConverter;

            LineBrush1.Brush = new SolidColorBrush(unselectedColor);
            LineBrush2.Brush = new SolidColorBrush(unselectedColor);
        }
    }
}
