using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core.Settings
{
    internal class SkyMpSettings
    {
        internal SkyMpSettings()
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
            SkyMpSettings? skyMpSettings = await Load();

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
            await skyMpSettings.Save();
        }

        internal static async Task<SkyMpSettings?> Load()
        {
            string path = Paths.SkyMpSettingsFilePath();

            if (!File.Exists(path))
            {
                return new SkyMpSettings();
            }

            string fileContent = await File.ReadAllTextAsync(path);

            if (string.IsNullOrEmpty(fileContent))
            {
                return new SkyMpSettings();
            }

            SkyMpSettings? deserializedSettings = JsonConvert.DeserializeObject<SkyMpSettings>(fileContent);

            return deserializedSettings;
        }

        internal async Task Save()
        {
            string serializedSettings = JsonConvert.SerializeObject(this);

            await File.WriteAllTextAsync(Paths.SkyMpSettingsFilePath(), serializedSettings);
        }
    }
}
