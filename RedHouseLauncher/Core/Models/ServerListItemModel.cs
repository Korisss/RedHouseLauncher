using System;
using System.Net.Cache;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RedHouseLauncher.Core.Models
{
    internal class ServerListItemModel
    {
        private ServerListItemModel()
        {
            ServerIcon = null;
            Name = null;
            Online = null;
            Setting = "Role Play";
            Lang = "RU";
            Ping = "-";
            Ip = null;
            Port = -1;
        }

        public BitmapImage? ServerIcon { get; private set; }
        public string? Name { get; private init; }
        public string? Online { get; private init; }
        public string? Setting { get; }
        public string? Lang { get; }
        public string? Ping { get; set; }
        internal string? Ip { get; private init; }
        internal short Port { get; private init; }

        internal static async Task<ServerListItemModel[]> GetServerListItemsAsync()
        {
            try
            {
                ServerModel[] servers = await ServerModel.GetServerList();
                ServerListItemModel[] serverListItems = new ServerListItemModel[servers.Length];

                for (int i = 0; i < servers.Length; i++)
                {
                    ServerListItemModel serverListItem = new()
                    {
                        Name = servers[i].Name,
                        Online = servers[i].Online + "/" + servers[i].MaxPlayers,
                        Port = servers[i].Port,
                        Ip = servers[i].Ip
                    };

                    serverListItem.Ping = await serverListItem.GetPing();
                    serverListItem.UpdateIcon();

                    serverListItems[i] = serverListItem;
                }

                return serverListItems;
            }
            catch
            {
                // TODO: Разобраться с ошибкой при верстке
                return Array.Empty<ServerListItemModel>();
            }
        }

        internal void UpdateIcon()
        {
            ServerIcon = GetServerIcon();
        }

        internal async Task<string> GetDescription()
        {
            string? desc = null;

            try
            {
                desc = await Networking.RequestAsync($"http://{Ip}:{Port + 1}/desc.txt");
            }
            catch
            {
                // ignored
            }

            return desc ?? "Не удалось получить описание";
        }

        static Ping ping = new();

        private async Task<string> GetPing()
        {
            if (Ip == null)
            {
                return "-";
            }

            try
            {
                PingReply reply = await ping.SendPingAsync(Ip, 200);

                if (reply.Status == IPStatus.Success)
                {
                    return reply.RoundtripTime.ToString();
                }
            }
            catch
            {
                // ignored
            }

            return "-";
        }

        private BitmapImage GetServerIcon()
        {
            if (Ping == "-")
            {
                BitmapImage logo = new();

                logo.BeginInit();
                logo.UriSource = new Uri(@"pack://application:,,,/UI/Images/Icons/ServerIcon.png");
                logo.EndInit();

                return logo;
            }

            BitmapImage logo1 = new();
            logo1.BeginInit();

            logo1.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
            logo1.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

            logo1.UriSource = new Uri($"http://{Ip}:{Port + 1}/servericon.png");

            logo1.EndInit();

            return logo1;
        }

    }
}
