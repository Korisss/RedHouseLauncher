using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core.Settings
{
    internal class SkyMPSettings
    {
        internal SkyMPSettings()
        {
            ServerIp = null;
            ServerPort = -1;
            ShowMe = false;
            EnableConsole = false;
            GameData = null;
        }

        [JsonProperty("server-ip")] internal string? ServerIp { get; set; }
        [JsonProperty("server-port")] internal short ServerPort { get; set; }
        [JsonProperty("show-me")] internal bool ShowMe { get; set; }
        [JsonProperty("enable-console")] internal bool EnableConsole { get; set; }
        [JsonProperty("gameData")] internal object? GameData { get; set; }

        internal static async Task DestroySession()
        {
            SkyMPSettings? skyMpSettings = await Load();

            if (skyMpSettings == null)
            {
                return;
            }

            if (skyMpSettings.ServerIp == null && skyMpSettings.ServerPort == -1 && skyMpSettings.GameData == null)
            {
                return;
            }

            skyMpSettings.ServerIp = null;
            skyMpSettings.ServerPort = -1;

            skyMpSettings.GameData = null;
            skyMpSettings.Save();
        }

        internal static async Task<SkyMPSettings?> Load()
        {
            string path = Paths.SkyMPSettingsFilePath();

            if (!File.Exists(path))
            {
                return new SkyMPSettings();
            }

            string fileContent = await File.ReadAllTextAsync(path);

            if (fileContent == null || fileContent == "")
            {
                return new SkyMPSettings();
            }

            SkyMPSettings? deserializedSettings = JsonConvert.DeserializeObject<SkyMPSettings>(fileContent);

            return deserializedSettings;
        }

        internal async Task Save()
        {
            string serializedSettings = JsonConvert.SerializeObject(this);

            await File.WriteAllTextAsync(Paths.SkyMPSettingsFilePath(), serializedSettings);
        }
    }
}
