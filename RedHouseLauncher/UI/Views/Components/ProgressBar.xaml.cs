using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RedHouseLauncher.UI.Views.Components
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : UserControl
    {
        private long _lastBytes;

        private DateTime _lastUpdate;

        public ProgressBar()
        {
            InitializeComponent();
        }

        // private bool _isInWaitMode;
        private void SetPercentage(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());

            if (totalBytes == 0)
            {
                Dispatcher.Invoke(() =>
                {
                    PercentageLabel.Content = "0%";
                    ProgressBarLength.Width = 0;
                });

                return;
            }

            double percentage = bytesIn / totalBytes * 100;
            double progressBarLength = 775 * percentage / 100;

            Dispatcher.Invoke(() =>
            {
                ProgressBarLength.Width = progressBarLength;
                PercentageLabel.Content = Math.Floor(percentage) + "%";
            });
        }

        private void SetSpeed(object sender, DownloadProgressChangedEventArgs e)
        {
            if (_lastBytes == 0)
            {
                _lastUpdate = DateTime.Now;
                _lastBytes = e.BytesReceived;
                return;
            }

            DateTime now = DateTime.Now;
            TimeSpan timeSpan = now - _lastUpdate;
            double bytesChange = e.BytesReceived - _lastBytes;

            if (timeSpan.Seconds == 0)
            {
                return;
            }

            double bytesPerSecond = bytesChange / timeSpan.Seconds;

            _lastBytes = e.BytesReceived;
            _lastUpdate = now;

            Dispatcher.Invoke(() =>
            {
                Speed.Content = bytesPerSecond / 1024 / 1024 > 0
                    ? $"{Math.Floor(bytesPerSecond / 1024 / 1024)} MB/s"
                    : $"{Math.Floor(bytesPerSecond / 1024)} KB/s";
            });
        }

        private void SetDownloadName(string name)
        {
            Dispatcher.Invoke(() => DownloadName.Content = name);
        }

        internal async Task DownloadFile(string uri, string path, string name)
        {
            try
            {
                Visibility = Visibility.Visible;

                SetDownloadName($"Загрузка {name}");

                Dispatcher.Invoke(() =>
                {
                    Speed.Content = "0 MB/s";
                    PercentageLabel.Content = "0%";
                    ProgressBarLength.Width = 0;
                });

                // Перетащить на HttpClient при возможности
                using (WebClient client = new())
                {
                    client.DownloadFileCompleted += (sender, args) =>
                        Dispatcher.Invoke(() => Visibility = Visibility.Hidden);
                    client.DownloadProgressChanged += SetPercentage;
                    client.DownloadProgressChanged += SetSpeed;

                    await client.DownloadFileTaskAsync(new Uri(uri), path);
                }

                Dispatcher.Invoke(() => { Visibility = Visibility.Hidden; });
            }
            catch (Exception err)
            {
                throw new Exception($"Ошибка во время загрузки файла {name}:\n\n{err}");
            }
        }

        internal async Task UnpackFile(string filePath, string unpackPath, string name)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    Visibility = Visibility.Visible;

                    SetDownloadName($"Распаковка {name}");
                    Speed.Content = "";
                    PercentageLabel.Content = "0%";
                    ProgressBarLength.Width = 0;
                });

                using (SevenZipArchive archive = SevenZipArchive.Open(filePath))
                {
                    double totalBytes = archive.TotalUncompressSize;
                    double unpackedBytes = 0;
                    int percentage;

                    IReader reader = archive.ExtractAllEntries();

                    await Task.Run(() =>
                    {
                        while (reader.MoveToNextEntry())
                        {
                            if (reader.Entry.IsDirectory)
                            {
                                continue;
                            }

                            reader.WriteEntryToDirectory(unpackPath,
                                new ExtractionOptions { ExtractFullPath = true, Overwrite = true });

                            unpackedBytes += reader.Entry.Size;
                            percentage = (int)Math.Floor(unpackedBytes / totalBytes * 100);

                            int progressBarLength = 775 / 100 * percentage;

                            int percentage1 = percentage;

                            Dispatcher.Invoke(() =>
                            {
                                PercentageLabel.Content = $"{percentage1}%";

                                ProgressBarLength.Width = progressBarLength;
                            });
                        }
                    });
                }

                Dispatcher.Invoke(() => { Visibility = Visibility.Hidden; });
            }
            catch (Exception err)
            {
                throw new Exception($"Ошибка во время распаковки архива {name}:\n\n{err}");
            }
        }

        /*
                internal async Task StartWaitMode(string name)
                {
                    _ = await Dispatcher.InvokeAsync(async () =>
                      {
                          bool is775 = false;

                          Visibility = Visibility.Visible;
                          SetDownloadName(name);
                          Speed.Content = "";
                          PercentageLabel.Content = "";
                          ProgressBarLength.Width = 0;
                          _isInWaitMode = true;

                          while (true)
                          {
                              if (!_isInWaitMode)
                              {
                                  return;
                              }

                              await Task.Delay(1);

                              if (ProgressBarLength.Width >= 775)
                              {
                                  is775 = true;
                              }
                              else if (ProgressBarLength.Width <= 0)
                              {
                                  is775 = false;
                              }

                              if (is775)
                              {
                                  ProgressBarLength.Width -= 2;
                              }
                              else
                              {
                                  ProgressBarLength.Width += 2;
                              }
                          }
                      });
                }
        */

        /*
                internal void StopWaitMode()
                {
                    Dispatcher.Invoke(() =>
                    {
                        Visibility = Visibility.Hidden;
                        _isInWaitMode = false;
                    });
                }
        */
    }
}
