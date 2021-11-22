using System;

namespace RedHouseLauncher.Core
{
    internal static class Paths
    {
        internal static readonly string LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        internal static readonly string SettingsFilePath = LocalAppDataPath + @"\RedHouseLauncher\settings.json";

        // internal static string SkyMpSettingsFilePath = Settings.PathToSkyrim + "Data\\Platform\\Plugins\\skymp5-client-settings.txt";

        //internal static string LauncherFilesPath = Settings.PathToSkyrim + "Launcher_Files\\";

        // internal static string SkyrimExePath = Settings.PathToSkyrim + "skse64_loader.exe";
    }
}
