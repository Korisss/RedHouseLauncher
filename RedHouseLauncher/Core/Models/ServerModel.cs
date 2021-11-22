using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core.Models
{
    internal class ServerModel
    {
        public ServerModel()
        {
            Ip = null;
            Port = -1;
            Name = null;
            MaxPlayers = null;
            Online = null;
        }
        [JsonProperty("ip")] internal string? Ip { get; set; }
        [JsonProperty("port")] internal short Port { get; set; }
        [JsonProperty("name")] internal string? Name { get; set; }
        [JsonProperty("maxPlayers")] internal string? MaxPlayers { get; set; }
        [JsonProperty("online")] internal string? Online { get; set; }

        internal static async Task<ServerModel[]> GetServerList()
        {
            string? response = await Networking.RequestAsync(Settings.Settings.MasterServer + "servers");

            if (response == null)
            {
                return Array.Empty<ServerModel>();
            }

            return JArray.Parse(response).ToObject<ServerModel[]>() ?? Array.Empty<ServerModel>();
        }
    }
}
