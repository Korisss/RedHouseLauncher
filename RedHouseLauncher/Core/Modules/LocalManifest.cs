using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Modules
{
    internal class LocalManifest
    {
        internal LocalManifest()
        {
            InstalledModules = new List<LocalModule>();
            EnabledModules = new List<string>();
        }

        internal LocalManifest(List<LocalModule> installedModules, List<string> enabledModules)
        {
            InstalledModules = installedModules;
            EnabledModules = enabledModules;
        }

        [JsonProperty("installedModules")] internal List<LocalModule> InstalledModules { get; private set; }
        [JsonProperty("enabledModules")] internal List<string> EnabledModules { get; private set; }

        internal static async Task<LocalManifest> Load()
        {
            string path = Paths.LauncherFilesPath() + "gameManifest.json";

            if (!File.Exists(path))
            {
                return new LocalManifest();
            }

            string gameManifestJson = await File.ReadAllTextAsync(path);

            if (gameManifestJson == "")
            {
                return new LocalManifest();
            }

            LocalManifest gameManifest = JsonConvert.DeserializeObject<LocalManifest>(gameManifestJson) ?? new LocalManifest();

            return gameManifest;
        }

        internal void AddModule(UpdaterModule module)
        {
            LocalModule newModule = new(module.Name, module.Version);
            if (newModule.Name == null)
            {
                return;
            }

            for (int i = 0; i < InstalledModules.Count; i++)
            {
                if (InstalledModules[i].Name == newModule.Name)
                {
                    InstalledModules.RemoveAt(i);
                }
            }

            EnableModule(newModule.Name);
            InstalledModules.Add(newModule);
            Save();
        }

        internal void RemoveModule(UpdaterModule module)
        {
            LocalModule newModule = new(module.Name, module.Version);
            if (newModule.Name == null)
            {
                return;
            }

            for (int i = 0; i < InstalledModules.Count; i++)
            {
                if (InstalledModules[i].Name == newModule.Name)
                {
                    _ = InstalledModules.Remove(InstalledModules[i]);
                }
            }

            DisableModule(newModule.Name);
            Save();
        }

        internal void EnableModule(string name)
        {
            if (EnabledModules.Contains(name))
            {
                return;
            }

            EnabledModules.Add(name);
            Save();
        }

        private void DisableModule(string name)
        {
            if (!EnabledModules.Contains(name))
            {
                return;
            }

            _ = EnabledModules.Remove(name);
            Save();
        }

        internal void Save()
        {
            string launcherFilesPath = Paths.LauncherFilesPath();
            string manifestPath = launcherFilesPath + "gameManifest.json";
            string serializedManifest = JsonConvert.SerializeObject(this);

            if (!Directory.Exists(launcherFilesPath))
            {
                _ = Directory.CreateDirectory(launcherFilesPath);
            }

            File.WriteAllText(manifestPath, serializedManifest);
        }
    }
}
