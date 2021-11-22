using System.Threading.Tasks;

namespace RedHouseLauncher.Core.Settings
{
    internal class Settings
    {
        private static string? _pathToSkyrim = null;

        private static string? _masterServer = null;
        internal static int UserId { get; set; }
        internal static string? UserToken { get; set; }
        internal static string? UserName { get; set; }

        internal static string? PathToSkyrim
        {
            get
            {
                if (_pathToSkyrim == null)
                {
                    return null;
                }

                return _pathToSkyrim.EndsWith("\\") ? _pathToSkyrim : $"{_pathToSkyrim}\\";
            }

            set
            {
                if (value == null)
                {
                    _pathToSkyrim = null;
                }
                else
                {
                    _pathToSkyrim = value.EndsWith("\\") ? value : $"{value}\\";
                }
            }
        }

        internal static string? MasterServer
        {
            get
            {
                if (_masterServer == null)
                {
                    return null;
                }

                return _masterServer.EndsWith("/") ? _masterServer : $"{_masterServer}/";
            }

            set
            {
                if (value == null)
                {
                    _masterServer = null;
                }
                else
                {
                    _masterServer = value.EndsWith("/") ? value : $"{value}/";
                }
            }
        }

        internal static async Task Load()
        {
            SettingsFile settingsFile = await SettingsFile.Load();

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
