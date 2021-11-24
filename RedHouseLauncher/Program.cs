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
            if (CheckAlreadyStarted()) return;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            Task.Run(async () => await Updater.Update());
            Task.Run(async () => await Settings.Load());            

            App app = new();
            app.InitializeComponent();
            app.Run();
        }

        private static bool CheckAlreadyStarted()
        {
            string exe = AppDomain.CurrentDomain.FriendlyName;

            Process[] launcherProcesses = Process.GetProcessesByName(exe);

            if (launcherProcesses.Length < 2)
            {
                return false;
            }

            MessageBox.Show("Лаунчер уже запущен.");

            return true;
        }
    }
}
