using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using RedHouseLauncher.Core;
using RedHouseLauncher.Core.Settings;

namespace RedHouseLauncher.UI.Models
{
    internal class Server
    {
        [JsonProperty("ip")] private readonly string ip = default!;
        [JsonProperty("port")] private readonly int port = default!;
        [JsonProperty("name")] private readonly string name = default!;
        [JsonProperty("maxPlayers")] private readonly int maxPlayers = default!;
        [JsonProperty("online")] private readonly int online = default!;

        public string Ip => ip;

        public int Port => port;

        public string Name => name;

        public string Online => $"{online}/{maxPlayers}";

        public BitmapImage? ServerIcon
        {
            get
            {
                BitmapImage logo = new();

                logo.BeginInit();
                logo.UriSource = new Uri($"http://{Ip}:{Port + 1}/servericon.png");
                logo.EndInit();

                if (logo != null)
                {
                    return logo;
                }

                logo = new();
                logo.BeginInit();
                logo.UriSource = new Uri(@"pack://application:,,,/UI/Images/Icons/ServerIcon.png");
                logo.EndInit();

                return logo;
            }
        }

        public static string Setting => "Role Play";

        public static string Lang => "RU";

        public string Ping
        {
            get
            {
                Ping ping = new();

                try
                {
                    PingReply reply = ping.SendPingAsync(Ip, 200).Result;

                    if (reply.Status == IPStatus.Success)
                    {
                        return reply.RoundtripTime.ToString();
                    }
                }
                catch { }

                return "-";
            }
        }


        public string Description
        {
            get
            {
                try
                {
                    string? desc = Networking.Request($"http://{Ip}:{Port + 1}/desc.txt");
                    if (desc != null)
                    {
                        return desc;
                    }
                }
                catch { }

                return "Не удалось получить описание";
            }
        }

        internal static async Task<Server[]> GetServerList()
        {
            string? response = await Networking.RequestAsync(Settings.MasterServer + "servers");

            if (response == null)
            {
                return Array.Empty<Server>();
            }

            Server[]? serverList = JsonConvert.DeserializeObject<Server[]>(response); // JArray.Parse(response).ToObject<ServerModel[]>();
            return serverList ?? Array.Empty<Server>();
        }

        internal static Server[] GetServerListSync()
        {
            string? response = Networking.Request(Settings.MasterServer + "servers");

            if (response == null)
            {
                return Array.Empty<Server>();
            }

            Server[]? serverList = JsonConvert.DeserializeObject<Server[]>(response); // JArray.Parse(response).ToObject<ServerModel[]>();
            return serverList ?? Array.Empty<Server>();
        }
    }

}

