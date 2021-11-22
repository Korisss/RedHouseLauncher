using RedHouseLauncher.Core.Settings;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RedHouseLauncher
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            Task.Run(async () =>
            {
                await Settings.Load();
            });

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3; // Попробовать потом убрать SSL

            App app = new();
            app.InitializeComponent();
            app.Run();
        }
    }
}
