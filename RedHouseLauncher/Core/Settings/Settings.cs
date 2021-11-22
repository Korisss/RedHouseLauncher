using System;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core.Settings
{
    internal class Settings
    {
        private static string _pathToSkyrim = "";

        private static string _masterServer = "";
        internal static int UserId { get; set; }
        internal static string? UserToken { get; set; }
        internal static string? UserName { get; set; }

        internal static string PathToSkyrim
        {
            get => _pathToSkyrim.EndsWith("\\") ? _pathToSkyrim : $"{_pathToSkyrim}\\";
            set => _pathToSkyrim = value.EndsWith("\\") ? value : $"{value}\\";
        }

        internal static string MasterServer
        {
            get => _masterServer.EndsWith("/") ? _masterServer : $"{_masterServer}/";
            set => _masterServer = value.EndsWith("/") ? value : $"{value}/";
        }

        internal static async Task Load()
        {
            SettingsFile? settingsFile = await SettingsFile.Load();

            if (settingsFile == null)
            {
                throw new Exception("Неожиданная ошибка при загрузке настроек.");
            }

            if (settingsFile.PathToSkyrim == null)
            {
                return;
            }

            UserId = settingsFile.UserId;
            UserToken = settingsFile.UserToken;
            UserName = settingsFile.UserName;
            _pathToSkyrim = settingsFile.PathToSkyrim;
            _masterServer = settingsFile.MasterServer;
        }

        internal static async Task Save()
        {
            SettingsFile settingsFile = new(UserId, UserToken, UserName, _pathToSkyrim, _masterServer);

            await settingsFile.Save();
        }
    }
}
