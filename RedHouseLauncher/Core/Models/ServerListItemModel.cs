﻿using System;
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
        public string? Name { get; private set; }
        public string? Online { get; private set; }
        public string? Setting { get; }
        public string? Lang { get; }
        public string? Ping { get; set; }
        internal string? Ip { get; private set; }
        internal short Port { get; private set; }

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

                    serverListItems[i] = serverListItem;
                }

                return serverListItems;
            }
            catch
            {
                // Разобраться с ошибкой при верстке
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
            catch { }

            return desc ?? "Не удалось получить описание";
        }

        private async Task<string> GetPing()
        {
            if (Ip == null)
            {
                return "-";
            }

            try
            {
                Ping ping = new();
                PingReply reply = await ping.SendPingAsync(Ip);

                if (reply.Status == IPStatus.Success)
                {
                    return reply.RoundtripTime.ToString();
                }
            }
            catch { }

            return "-";
        }

        private BitmapImage? GetServerIcon()
        {
            try
            {
                BitmapImage logo = new();
                logo.BeginInit();

                logo.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);

                logo.UriSource = new Uri($"http://{Ip}:{Port + 1}/servericon.png");

                logo.EndInit();

                return logo;
            }
            catch { }

            return null;
        }
    }
}
