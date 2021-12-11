using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UpdateHelper.UI.Components
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : UserControl
    {
        public ProgressBar()
        {
            InitializeComponent();
        }

        private void UpdateLength(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());

            if (totalBytes == 0)
            {
                Dispatcher.Invoke(() => ProgressBarLength.Width = 0);
                return;
            }

            double percentage = bytesIn / totalBytes * 100;
            double progressBarLength = 740 * percentage / 100;

            Dispatcher.Invoke(() => ProgressBarLength.Width = progressBarLength);
        }

        internal async Task DownloadFile(string uri, string path)
        {
            Dispatcher.Invoke(() => ProgressBarLength.Width = 0);

            foreach (Process process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path)))
            {
                process.Kill();
            }

            Thread.Sleep(100);

#pragma warning disable SYSLIB0014
            using WebClient client = new();
#pragma warning restore SYSLIB0014

            client.DownloadProgressChanged += UpdateLength;

            await client.DownloadFileTaskAsync(new Uri(uri), path);

            Process.Start(path);

            Dispatcher.Invoke(() => Application.Current.Shutdown());
        }
    }
}
