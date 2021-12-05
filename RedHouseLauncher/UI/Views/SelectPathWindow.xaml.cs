using RedHouseLauncher.Core.Settings;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace RedHouseLauncher.UI.Views
{
    /// <summary>
    /// Interaction logic for SelectPathWindow.xaml
    /// </summary>
    public partial class SelectPathWindow
    {
        public SelectPathWindow()
        {
            try
            {
                if (Settings.PathToSkyrim is "" or "\\" or null)
                {
                    InitializeComponent();
                    return;
                }

                new MainWindow().Show();

                Close();
            }
            catch (Exception err)
            {
                MessageBox.Show($"Произошла ошибка во время инициализации окна выбора пути к Скайриму:\n\n{err}");
            }
        }

        private async void SelectPathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new();

            folderBrowserDialog.ShowDialog();

            if (!Path.IsPathRooted(folderBrowserDialog.SelectedPath))
            {
                return;
            }

            PathTextBox.Text = folderBrowserDialog.SelectedPath;

            Settings.PathToSkyrim = PathTextBox.Text;
            await Settings.Save();

            new MainWindow().Show();

            Close();
        }
    }
}
