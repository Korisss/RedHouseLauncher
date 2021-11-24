using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace RedHouseLauncher.Core
{
    internal class Updater
    {
        private const short version = 214;
        internal static async Task Update()
        {
            try
            {
                if (await IsUpdated())
                {
                    return;
                }

                string launcherPath = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".exe";
                string tempPath = Path.GetTempPath();

                string url = $"{Config.UpdaterUri}UpdateHelper.exe";
                string downloadPath = $"{tempPath}UpdateHelper.exe";

                using (HttpClient httpClient = new())
                {
                    httpClient.BaseAddress = new Uri(launcherPath);

                    using Stream stream = await httpClient.GetStreamAsync(url);
                    using FileStream fileStream = File.OpenWrite(downloadPath);

                    stream.CopyTo(fileStream);
                    fileStream.Close();
                }

                File.WriteAllText($"{tempPath}RHLauncher_Path.txt", launcherPath);

                using (Process process = new())
                {
                    process.StartInfo.WorkingDirectory = tempPath;
                    process.StartInfo.FileName = downloadPath;

                    process.Start();
                }

                Application.Current.Shutdown();
            }
            catch (Exception err)
            {
                throw new Exception($"Произошла ошибка во время проверки обновлений:\n{err}");
            }
        }

        private static async Task<bool> IsUpdated()
        {
            string url = $"{Config.UpdaterUri}launcherVersion.txt";

            int versionFromSite = Convert.ToInt32(await Networking.RequestAsync(url));

            return version == versionFromSite;
        }
    }
}
