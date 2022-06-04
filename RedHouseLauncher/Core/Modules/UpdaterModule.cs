using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedHouseLauncher.UI.Views.Components;

namespace RedHouseLauncher.Core.Modules
{
    internal class UpdaterModule
    {
        internal UpdaterModule()
        {
            Name = "";
            Required = false;
            Hashes = new Hashes();
            Files = Array.Empty<string>();
            Version = -1;
        }

        internal UpdaterModule(string name)
        {
            Name = name;
            Required = false;
            Hashes = new Hashes();
            Files = Array.Empty<string>();
            Version = -1;
        }

        internal string Name { get; set; }
        internal bool Required { get; set; }

        [JsonProperty("hashes")] internal Hashes Hashes { get; set; }
        [JsonProperty("files")] internal string[] Files { get; set; }
        [JsonProperty("version")] internal short Version { get; set; }

        #region Проверка установки

        private bool IsInstalled(LocalManifest localManifest)
        {
            return CheckAllFilesExists() && CheckHashes()
&& localManifest.InstalledModules.Any(moduleToCheck => moduleToCheck.Name == Name);
        }

        #endregion

        #region Проверка апдейта

        internal bool IsUpdated(LocalManifest localManifest)
        {
            return IsInstalled(localManifest) && localManifest.InstalledModules.Any(moduleToCheck =>
                moduleToCheck.Name == Name && moduleToCheck.Version == Version);
        }

        #endregion

        #region Проверка всех файлов модуля

        private bool CheckAllFilesExists()
        {
            return Files.All(fileToCheck => File.Exists($"{Settings.Settings.PathToSkyrim}{fileToCheck}"));
        }

        #endregion

        #region Проверка хэшей

        private bool CheckHashes()
        {
            SHA256 sha = SHA256.Create();


            for (int i = 0; i < Hashes.FilesToCheck.Length; i++)
            {
                byte[] buffer = File.ReadAllBytes($"{Settings.Settings.PathToSkyrim}{Hashes.FilesToCheck[i]}");

                byte[] sha256Result = sha.ComputeHash(buffer);

                StringBuilder stringBuilder = new();

                foreach (byte b in sha256Result)
                {
                    _ = stringBuilder.Append($"{b:X2}");
                }

                string hash = stringBuilder.ToString();

                if (hash != Hashes.FilesHashes[i])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Получение по имени

        internal static async Task<UpdaterModule?> Get(string name, bool isRequired)
        {
            string require = isRequired ? "required" : "unrequired";

            string? response = await Networking.RequestAsync($"{Config.UpdaterUri}modules/{require}/{name}/manifest.json");

            if (response == null)
            {
                return new UpdaterModule(name);
            }

            UpdaterModule module = JsonConvert.DeserializeObject<UpdaterModule>(response) ?? new UpdaterModule(name);

            module.Name = name;
            module.Required = isRequired;

            return module;
        }

        #endregion

        #region Установка

        internal async Task Install(ProgressBar progressbar, LocalManifest localManifest)
        {
            if (Settings.Settings.PathToSkyrim == null)
            {
                throw new Exception("[Установка модуля] Путь к игре пуст");
            }

            string require = Required ? "required" : "unrequired";
            string uri = $"{Config.UpdaterUri}/modules/{require}/{Name}/download.7z";
            string downloadPath = $"{Path.GetTempPath()}download.7z";

            await progressbar.DownloadFile(uri, downloadPath, Name);
            await progressbar.UnpackFile(downloadPath, Settings.Settings.PathToSkyrim, Name);

            File.Delete(downloadPath);

            localManifest.AddModule(this);
        }

        #endregion

        #region Удаление

        private void Delete(LocalManifest localManifest)
        {
            foreach (string file in Files)
            {
                string path = Settings.Settings.PathToSkyrim + file;

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            localManifest.RemoveModule(this);
        }

        internal async Task Delete()
        {
            LocalManifest localManifest = await LocalManifest.Load();

            Delete(localManifest);

            localManifest.Save();
        }

        #endregion

        #region Получение List esp

        internal IEnumerable<string> GetEspsList()
        {
            List<string> espsList = new();

            foreach (string file in Files)
            {
                if (IsEsp(file) && !espsList.Contains(file))
                {
                    espsList.Add(file[5..]);
                }
            }

            return espsList;
        }

        private static bool IsEsp(string file)
        {
            return file.EndsWith(".esp") || file.EndsWith(".esm") || file.EndsWith(".esl");
        }

        #endregion
    }

    internal class Hashes
    {
        internal Hashes()
        {
            FilesToCheck = Array.Empty<string>();
            FilesHashes = Array.Empty<string>();
        }

        [JsonProperty("filesToCheck")] internal string[] FilesToCheck { get; private set; }
        [JsonProperty("filesHashes")] internal string[] FilesHashes { get; private set; }
    }
}
