using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core.Settings
{
    internal class SettingsFile
    {
        internal SettingsFile()
        {
            UserId = "";
            UserToken = null;
            PathToSkyrim = null;
            UserName = null;
            MasterServer = Config.DefaultMaster;
        }

        internal SettingsFile(string userId, string? userToken, string? userName, string? pathToSkyrim, string? masterServer)
        {
            UserId = userId;
            UserToken = userToken;
            UserName = userName;
            PathToSkyrim = pathToSkyrim;
            MasterServer = masterServer;
        }

        [JsonProperty("userId")] internal string UserId { get; set; }
        [JsonProperty("userToken")] internal string? UserToken { get; set; }
        [JsonProperty("userName")] internal string? UserName { get; set; }
        [JsonProperty("pathToSkyrim")] internal string? PathToSkyrim { get; set; }
        [JsonProperty("masterServer")] internal string? MasterServer { get; set; }

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
                Directory.CreateDirectory(dirPath);
            }

            await File.WriteAllTextAsync(Paths.SettingsFilePath, JsonConvert.SerializeObject(this));
        }
    }
}
