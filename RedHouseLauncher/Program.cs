using Newtonsoft.Json;
using RedHouseLauncher.Core;
using RedHouseLauncher.Core.GameUtils;
using RedHouseLauncher.Core.Modules;
using RedHouseLauncher.Core.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedHouseLauncher
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (args.Length > 0)
            {
                List<string> argsList = args.ToList();

                if (!argsList.Contains("--run-game"))
                {
                    return;
                }

                if (!argsList.Contains("--no-started-check"))
                {
                    if (CheckAlreadyStarted())
                    {
                        return;
                    }
                }
                if (!argsList.Contains("--no-updates-check"))
                {
                    Task.Run(async () => await Updater.Update());
                }

                Settings.LoadSync();

                string? ip = null;
                ushort port = 0;

                foreach (string arg in argsList)
                {
                    if (arg.StartsWith("--ip="))
                    {
                        ip = arg[5..];
                    }
                    else if (arg.StartsWith("--port="))
                    {
                        port = Convert.ToUInt16(arg[7..]);
                    }
                }

                SkyMpSettings? skyMpSettings = SkyMpSettings.Load().Result;

                if (skyMpSettings == null)
                {
                    MessageBox.Show("Не загружены настройки мультиплеера.");
                    return;
                }

                skyMpSettings.ServerIp = ip;
                skyMpSettings.ServerPort = (short)port;

                try
                {
                    string url = $"{Settings.MasterServer}users/{Settings.UserId}/play/{skyMpSettings.ServerIp}:{skyMpSettings.ServerPort}";

                    using HttpClient httpClient = new();

                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    httpClient.BaseAddress = new Uri(url);
                    httpClient.DefaultRequestHeaders.Add("Authorization", Settings.UserToken);

                    const string data = "";

                    using HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                    response.EnsureSuccessStatusCode();

                    using Stream responseStream = response.Content.ReadAsStream();
                    using StreamReader streamReader = new(responseStream);

                    object? result = JsonConvert.DeserializeObject(streamReader.ReadToEnd());

                    skyMpSettings.GameData = result;
                    skyMpSettings.SaveSync();
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Ошибка во время получения сессии.\n\n{err}");
                    return;
                }

                if (!argsList.Contains("--no-modules-check"))
                {
                    // TODO
                }
                if (!argsList.Contains("--no-esp-check"))
                {
                    ServerManifest? serverManifest = ServerManifest.Load($"http://{ip}:{port + 1}/").Result;

                    if (serverManifest == null)
                    {
                        throw new Exception("Получен пустой манифест сервера.");
                    }

                    string appDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    string skyrimPluginsPath = $"{appDataLocal}\\Skyrim Special Edition\\Plugins.txt";

                    if (!File.Exists(skyrimPluginsPath))
                    {
                        Directory.CreateDirectory($"{appDataLocal}\\Skyrim Special Edition\\");
                    }

                    List<string> plugins = new();

                    foreach (string esp in serverManifest.LoadOrder)
                    {
                        if (ServerFilesDownloader.IgnoredFiles.Contains(esp) || plugins.Contains(esp))
                        {
                            continue;
                        }

                        plugins.Add(esp);
                    }

                    foreach (string esp in UpdaterManifest.EspsToEnable.Where(esp => !ServerFilesDownloader.IgnoredFiles.Contains(esp) && !plugins.Contains(esp)))
                    {
                        plugins.Add(esp);
                    }

                    string pluginsFile = plugins.Aggregate("", (current, esp) => current + $"*{esp}\n");

                    File.WriteAllText(skyrimPluginsPath, pluginsFile);
                }

                Process[] skyrimProcess = Process.GetProcessesByName("SkyrimSE");

                if (skyrimProcess.Length > 0)
                {
                    throw new Exception("Игра уже запущена");
                }

                ProcessStartInfo startInfo = new()
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = Paths.SkyrimExePath(),
                    WorkingDirectory = Settings.PathToSkyrim
                };

                Process.Start(startInfo);

                return;
            }

            if (CheckAlreadyStarted())
            {
                return;
            }


            Task.Run(async () => await Updater.Update());

            Settings.LoadSync();

            App app = new();
            app.InitializeComponent();
            app.Run();
        }

        private static bool CheckAlreadyStarted()
        {
            string exe = AppDomain.CurrentDomain.FriendlyName;

            Process[] launcherProcesses = Process.GetProcessesByName(exe);

            if (launcherProcesses.Length < 2)
            {
                return false;
            }

            MessageBox.Show("Лаунчер уже запущен.");

            return true;
        }
    }
}
