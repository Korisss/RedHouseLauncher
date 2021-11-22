using RedHouseLauncher.Core.Modules;
using RedHouseLauncher.UI.Views.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core.GameUtils
{
    internal static class ServerFilesDownloader
    {
        private static readonly string[] IgnoredFiles =
        {
            "Skyrim.esm",
            "Update.esm",
            "Dawnguard.esm",
            "HearthFires.esm",
            "Dragonborn.esm"
        };

        internal static async Task DownloadFiles(string address, ProgressBar progressbar)
        {
            ServerManifest? serverManifest = await ServerManifest.Load(address);

            if (serverManifest == null)
            {
                throw new Exception("Получен пустой манифест сервера.");
            }

            ServerManifestMod[] mods = serverManifest.Mods;

            foreach (ServerManifestMod mod in mods)
            {
                string modPath = Settings.Settings.PathToSkyrim + "Data\\" + mod.Filename;

                if (!File.Exists(modPath))
                {
                    if (mod.Filename == null)
                    {
                        throw new Exception("Пустое название мода на сервере");
                    }

                    await progressbar.DownloadFile($"{address}{mod.Filename}", $"{Settings.Settings.PathToSkyrim}Data\\{mod.Filename}", mod.Filename);
                }
                else
                {
                    if (IgnoredFiles.Contains(mod.Filename))
                    {
                        continue;
                    }

                    byte[] buffer = await File.ReadAllBytesAsync(modPath);
                    int modCrc32 = Hash.Crc32(buffer);

                    if (mod.Crc32 != modCrc32)
                    {
                        if (mod.Filename == null)
                        {
                            throw new Exception("Пустое название мода на сервере");
                        }

                        await progressbar.DownloadFile($"{address}{mod.Filename}", $"{Settings.Settings.PathToSkyrim}Data\\{mod.Filename}", mod.Filename);
                    }
                }
            }

            string appDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string skyrimPluginsPath = $"{appDataLocal}\\Skyrim Special Edition\\Plugins.txt";

            if (!File.Exists(skyrimPluginsPath))
            {
                Directory.CreateDirectory($"{appDataLocal}\\Skyrim Special Edition\\");
            }

            List<string> plugins = new();

            string pluginsFile = "";

            foreach (string esp in serverManifest.LoadOrder)
            {
                if (IgnoredFiles.Contains(esp) || plugins.Contains(esp))
                {
                    continue;
                }

                plugins.Add(esp);
            }

            foreach (string esp in UpdaterManifest.EspsToEnable)
            {
                if (IgnoredFiles.Contains(esp) || plugins.Contains(esp))
                {
                    continue;
                }

                plugins.Add(esp);
            }

            foreach (string esp in plugins)
            {
                pluginsFile += $"*{esp}\n";

            }

            File.WriteAllText(skyrimPluginsPath, pluginsFile);
        }
    }
}
