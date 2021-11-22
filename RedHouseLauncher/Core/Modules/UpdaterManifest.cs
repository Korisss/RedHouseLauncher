using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedHouseLauncher.Core.Modules
{
    internal class UpdaterManifest
    {
        internal static List<string> EspsToEnable = new List<string>();

        internal UpdaterManifest()
        {
            Required = Array.Empty<string>();
            UnRequired = Array.Empty<string>();
        }

        [JsonProperty("required")] internal string[] Required { get; private set; }
        [JsonProperty("unRequired")] internal string[] UnRequired { get; private set; }

        internal async Task<bool> UpdateAll(UI.Views.Components.ProgressBar progressbar)
        {
            LocalManifest localManifest = await LocalManifest.Load();
            List<string> checkedModules = new();

            try
            {
                foreach (string required in Required)
                {
                    if (checkedModules.Contains(required))
                    {
                        continue;
                    }

                    localManifest.EnableModule(required);

                    UpdaterModule? module = await UpdaterModule.Get(required, true);

                    if (module == null)
                    {
                        continue;
                    }

                    #region Проверка версии

                    if (!module.IsUpdated(localManifest))
                    {
                        if (module.Name == "skyrimPlatform")
                        {
                            DialogResult result = MessageBox.Show(
                                "Желаете ли Вы ознакомиться с исходным кодом проекта?", "Оповещение",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                Process.Start("https://github.com/alekcey0211/red-house-internal");
                            }
                        }

                        await module.Install(progressbar, localManifest);
                    }

                    #endregion

                    foreach (string file in module.Files)
                    {
                        if (IsEsp(file) && !EspsToEnable.Contains(file))
                        {
                            EspsToEnable.Add(file.Substring(5));
                        }
                    }

                    checkedModules.Add(required);
                }

                foreach (string enabledModule in localManifest.EnabledModules)
                {
                    localManifest.EnableModule(enabledModule);

                    if (checkedModules.Contains(enabledModule))
                    {
                        continue;
                    }

                    UpdaterModule? module = await UpdaterModule.Get(enabledModule, false);

                    if (module == null)
                    {
                        continue;
                    }

                    #region Проверка версии

                    if (!module.IsUpdated(localManifest))
                    {
                        await module.Install(progressbar, localManifest);
                    }

                    #endregion

                    EspsToEnable.AddRange(module.GetEspsList());

                    checkedModules.Add(enabledModule);
                }

                localManifest.Save();

                return true;
            }
            catch (Exception err)
            {
                throw new Exception($"Произошла ошибка во время проверки либо скачивания модулей. \n\n{err}");
            }
        }

        internal static async Task<UpdaterManifest> Get()
        {
            try
            {
                string? response = await Networking.RequestAsync($"{Config.UpdaterUri}modules/manifest.json");

                if (response == null)
                {
                    return new UpdaterManifest();
                }

                UpdaterManifest gameManifest = JsonConvert.DeserializeObject<UpdaterManifest>(response) ?? new UpdaterManifest();

                return gameManifest;
            }
            catch (Exception err)
            {
                throw new Exception($"Ошибка во время получения манифеста с модификациями:\n\n{err}");
            }
        }

        private static bool IsEsp(string file)
        {
            return file.EndsWith(".esp") || file.EndsWith(".esm") || file.EndsWith(".esl");
        }
    }
}
