using System;

namespace RedHouseLauncher
{
    internal static class Program
    {
        [STAThread]
        public static int Main()
        {
            App app = new();
            app.InitializeComponent();
            app.Run();

            return 0;
        }
    }
}
