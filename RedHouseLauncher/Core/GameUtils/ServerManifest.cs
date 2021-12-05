using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core.GameUtils
{
    internal class ServerManifest
    {
        internal ServerManifest()
        {
            VersionMajor = 1;
            Mods = Array.Empty<ServerManifestMod>();
            LoadOrder = Array.Empty<string>();
        }

        [JsonProperty("mods")] internal ServerManifestMod[] Mods { get; private set; }
        [JsonProperty("versionMajor")] internal int VersionMajor { get; private set; }
        [JsonProperty("loadOrder")] internal string[] LoadOrder { get; private set; }

        internal static async Task<ServerManifest?> Load(string address)
        {
            string manifestAddress = address + "manifest.json";
            string? manifest = await Networking.RequestAsync(manifestAddress);

            return manifest == null ? null : JsonConvert.DeserializeObject<ServerManifest>(manifest);
        }
    }
    internal class ServerManifestMod
    {
        internal ServerManifestMod()
        {
            Crc32 = 0;
            Filename = null;
            Size = -1;
        }
        [JsonProperty("crc32")] internal int Crc32 { get; private set; }
        [JsonProperty("filename")] internal string? Filename { get; private set; }
        [JsonProperty("size")] internal int Size { get; private set; }
    }
}
