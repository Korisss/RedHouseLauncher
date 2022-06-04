using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Models
{
    internal class Server
    {
        [JsonProperty("ip")] private readonly string ip;
        [JsonProperty("port")] private readonly int port;
        [JsonProperty("name")] private readonly string name;
        [JsonProperty("maxPlayers")] private readonly int maxPlayers;
        [JsonProperty("online")] private readonly int online;

        public string Ip => ip;

        public int Port => port;

        public string Name => name;

        public int Online => online;

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

        public string Setting => "Role Play";

        public string Lang => "RU";

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
            string? response = await Networking.RequestAsync(Settings.Settings.MasterServer + "servers");

            if (response == null)
            {
                return Array.Empty<Server>();
            }

            Server[]? serverList = JsonConvert.DeserializeObject<Server[]>(response); // JArray.Parse(response).ToObject<ServerModel[]>();
            return serverList ?? Array.Empty<Server>();
        }
    }

}

