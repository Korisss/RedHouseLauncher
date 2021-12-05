using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace RedHouseLauncher.Core
{
    internal static class Updater
    {
        private const short Version = 215;
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

                const string url = $"{Config.UpdaterUri}UpdateHelper.exe";
                string downloadPath = $"{tempPath}UpdateHelper.exe";

                using (HttpClient httpClient = new())
                {
                    httpClient.BaseAddress = new Uri(launcherPath);

                    await using Stream stream = await httpClient.GetStreamAsync(url);
                    await using FileStream fileStream = File.OpenWrite(downloadPath);

                    await stream.CopyToAsync(fileStream);
                    fileStream.Close();
                }

                await File.WriteAllTextAsync($"{tempPath}RHLauncher_Path.txt", launcherPath);

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
            const string url = $"{Config.UpdaterUri}launcherVersion.txt";

            int versionFromSite = Convert.ToInt32(await Networking.RequestAsync(url));

            return Version == versionFromSite;
        }
    }
}
