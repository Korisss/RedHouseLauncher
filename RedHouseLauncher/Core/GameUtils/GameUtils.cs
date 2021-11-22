using RedHouseLauncher.Core.Settings;
using RedHouseLauncher.UI.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core.GameUtils
{
    internal static class GameUtils
    {
        internal static bool IsGameRunning;

        internal static async Task StartGame(MainWindow window)
        {
            Process[] skyrimProcess = Process.GetProcessesByName("SkyrimSE");

            if (skyrimProcess.Length > 0)
            {
                throw new Exception("Игра уже запущена");
            }

            window.Hide();

            ProcessStartInfo startInfo = new(Paths.SkyrimExePath)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                WorkingDirectory = Settings.Settings.PathToSkyrim
            };

            Process.Start(startInfo);

            IsGameRunning = true;

            await WaitGame();

            IsGameRunning = false;

            window.Show();
        }

        private static async Task WaitGame()
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(120 * 1000);
                await SkyMPSettings.DestroySession();
            });

            while (true)
            {
                await Task.Delay(1000);

                Process[] skyrimProcess = Process.GetProcessesByName("SkyrimSE");

                if (skyrimProcess.Length < 1)
                {
                    return;
                }

                Process[] creationKits = Process.GetProcessesByName("CreationKit");

                foreach (Process creationKit in creationKits)
                {
                    creationKit.Kill();
                }
            }
        }
    }
}
