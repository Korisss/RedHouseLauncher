using System.Threading.Tasks;
using RedHouseLauncher.Core.Modules;
using RedHouseLauncher.UI.Views.Components;

namespace RedHouseLauncher.Core.GameUtils
{
    internal static class GameChecker
    {
        internal static bool IsChecking;

        internal static async Task<bool> CheckGame(ProgressBar progressbar)
        {
            if (Settings.Settings.PathToSkyrim == null)
            {
                return false;
            }

            IsChecking = true;

            #region Очистка символических ссылок

            SymlinksUtils.DeleteSymlinksRecursive(Settings.Settings.PathToSkyrim);

            #endregion

            #region Проверка всех модулей

            UpdaterManifest manifest = await UpdaterManifest.Get();
            if (!await manifest.UpdateAll(progressbar))
            {
                IsChecking = false;
                return false;
            }

            #endregion

            // Vivid Obsidian Cathedral

            IsChecking = false;
            return true;
        }
    }
}
