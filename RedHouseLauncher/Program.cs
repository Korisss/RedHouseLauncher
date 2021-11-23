using RedHouseLauncher.Core;
using RedHouseLauncher.Core.Settings;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace RedHouseLauncher
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            Task.Run(async () =>
            {
                await Updater.Update();
                await Settings.Load();
            });

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; // Попробовать потом вернуть SSL

            App app = new();

            CheckAlreadyStarted();

            app.InitializeComponent();
            app.Run();
        }

        private static void CheckAlreadyStarted()
        {
            string exe = AppDomain.CurrentDomain.FriendlyName;

            Process[] launcherProcesses = Process.GetProcessesByName(exe);

            if (launcherProcesses.Length < 2)
            {
                return;
            }

            MessageBox.Show("Лаунчер уже запущен.");
            Application.Current.Shutdown();
        }
    }
}
