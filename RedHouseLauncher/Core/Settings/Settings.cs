﻿using System.Threading.Tasks;

namespace RedHouseLauncher.Core.Settings
{
    internal class Settings
    {
        private static string? _pathToSkyrim;

        private static string? _masterServer;
        internal static long UserId { get; set; }
        internal static string? UserToken { get; set; }
        internal static string? UserName { get; set; }

        internal static string? PathToSkyrim
        {
            get => _pathToSkyrim == null ? null : _pathToSkyrim.EndsWith("\\") ? _pathToSkyrim : $"{_pathToSkyrim}\\";

            set => _pathToSkyrim = value == null ? null : value.EndsWith("\\") ? value : $"{value}\\";
        }

        internal static string? MasterServer
        {
            get => _masterServer == null ? null : _masterServer.EndsWith("/") ? _masterServer : $"{_masterServer}/";

            set => _masterServer = value == null ? null : value.EndsWith("/") ? value : $"{value}/";
        }

        internal static async Task Load()
        {
            SettingsFile settingsFile = await SettingsFile.Load();

            UserId = settingsFile.UserId;
            UserToken = settingsFile.UserToken;
            UserName = settingsFile.UserName;
            _pathToSkyrim = settingsFile.PathToSkyrim;
            // _masterServer = settingsFile.MasterServer;

            _masterServer = "http://92.38.222.103:3000/api/";
        }

        internal static void LoadSync()
        {
            SettingsFile settingsFile = SettingsFile.LoadSync();

            UserId = settingsFile.UserId;
            UserToken = settingsFile.UserToken;
            UserName = settingsFile.UserName;
            _pathToSkyrim = settingsFile.PathToSkyrim;
            // _masterServer = settingsFile.MasterServer;

            _masterServer = "http://92.38.222.103:3000/api/";
        }

        internal static async Task Save()
        {
            SettingsFile settingsFile = new(UserId, UserToken, UserName, _pathToSkyrim, _masterServer);

            await settingsFile.Save();
        }
    }
}
