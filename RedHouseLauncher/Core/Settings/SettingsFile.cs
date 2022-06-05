using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Settings
{
    internal class SettingsFile
    {
        internal SettingsFile()
        {
            UserId = -1;
            UserToken = null;
            PathToSkyrim = null;
            UserName = null;
        }

        internal SettingsFile(long userId, string? userToken, string? userName, string? pathToSkyrim)
        {
            UserId = userId;
            UserToken = userToken;
            UserName = userName;
            PathToSkyrim = pathToSkyrim;
        }

        [JsonProperty("userId")] internal long UserId { get; set; }
        [JsonProperty("userToken")] internal string? UserToken { get; set; }
        [JsonProperty("userName")] internal string? UserName { get; set; }
        [JsonProperty("pathToSkyrim")] internal string? PathToSkyrim { get; set; }

        internal static async Task<SettingsFile> Load()
        {
            string settingsPath = Paths.SettingsFilePath;
            if (!File.Exists(settingsPath))
            {
                return new SettingsFile();
            }

            string fileContent = await File.ReadAllTextAsync(settingsPath);

            SettingsFile deserializedSettingsFile = JsonConvert.DeserializeObject<SettingsFile>(fileContent) ?? new SettingsFile();

            return deserializedSettingsFile;

        }

        internal static SettingsFile LoadSync()
        {
            string settingsPath = Paths.SettingsFilePath;
            if (!File.Exists(settingsPath))
            {
                return new SettingsFile();
            }

            string fileContent = File.ReadAllText(settingsPath);

            SettingsFile deserializedSettingsFile = JsonConvert.DeserializeObject<SettingsFile>(fileContent) ?? new SettingsFile();

            return deserializedSettingsFile;

        }

        internal async Task Save()
        {
            string dirPath = Paths.LocalAppDataPath + @"\RedHouseLauncher";

            if (!Directory.Exists(dirPath))
            {
                _ = Directory.CreateDirectory(dirPath);
            }

            await File.WriteAllTextAsync(Paths.SettingsFilePath, JsonConvert.SerializeObject(this));
        }
    }
}
