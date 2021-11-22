using System;

namespace RedHouseLauncher.Core
{
    internal static class Paths
    {
        internal static readonly string LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        internal static readonly string SettingsFilePath = LocalAppDataPath + @"\RedHouseLauncher\settings.json";

        internal static string SkyMPSettingsFilePath()
        {
            return Settings.Settings.PathToSkyrim + "Data\\Platform\\Plugins\\skymp5-client-settings.txt";
        }

        internal static string LauncherFilesPath = Settings.Settings.PathToSkyrim + "Launcher_Files\\";

        internal static string SkyrimExePath()
        {
            return Settings.Settings.PathToSkyrim + "skse64_loader.exe";
        }
    }
}
