using RedHouseLauncher.Core.Settings;
using System;
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

            App app = new();
            app.InitializeComponent();
            app.Run();
        }
    }
}
