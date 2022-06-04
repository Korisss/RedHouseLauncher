using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using RedHouseLauncher.Core.Modules;

namespace RedHouseLauncher.UI.Views.Components
{
    /// <summary>
    /// Interaction logic for ModsPage.xaml
    /// </summary>
    public partial class ModsPage
    {
        public ModsPage()
        {
            InitializeComponent();

            MainWindow.ModsPageStatic = this;
        }

        internal async Task UpdateList()
        {
            ModsListView.Items.Clear();

            UpdaterManifest manifest = await UpdaterManifest.Get();

            string[] modulesNames = manifest.UnRequired;
            string[] reqModulesNames = manifest.Required;

            LocalManifest localManifest = await LocalManifest.Load();

            List<ListItem> installedItems = new();
            List<ListItem> notInstalledItems = new();

            foreach (string moduleName in reqModulesNames)
            {
                UpdaterModule? module = await UpdaterModule.Get(moduleName, true);

                if (module == null)
                {
                    continue;
                }

                bool isInstalled = localManifest.EnabledModules.Contains(module.Name);
                ListItem item = new(module, isInstalled);

                if (isInstalled)
                {
                    installedItems.Add(item);
                    continue;
                }

                notInstalledItems.Add(item);
            }

            foreach (string moduleName in modulesNames)
            {
                UpdaterModule? module = await UpdaterModule.Get(moduleName, false);

                if (module == null)
                {
                    continue;
                }

                bool isInstalled = localManifest.EnabledModules.Contains(module.Name);
                ListItem item = new(module, isInstalled);

                if (isInstalled)
                {
                    installedItems.Add(item);
                    continue;
                }

                notInstalledItems.Add(item);
            }

            foreach (ListItem moduleName in installedItems)
            {
                _ = ModsListView.Items.Add(moduleName);
            }

            foreach (ListItem moduleName in notInstalledItems)
            {
                _ = ModsListView.Items.Add(moduleName);
            }
        }

        private async void ToggleMod(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.ModsPageStatic == null)
            {
                return;
            }

            ListItem item = (ListItem)ModsListView.SelectedItem;

            if (item.IsInstalled)
            {
                _ = item.Delete();

                await MainWindow.ModsPageStatic.UpdateList();

                return;
            }

            LocalManifest localManifest = await LocalManifest.Load();

            localManifest.EnableModule(item.Name);
            localManifest.Save();

            await MainWindow.ModsPageStatic.UpdateList();
        }

        internal class ListItem : UpdaterModule
        {
            public ListItem(UpdaterModule module, bool isInstalled = false)
            {
                Name = module.Name;
                Required = module.Required;
                Hashes = module.Hashes;
                Files = module.Files;
                Version = module.Version;
                IsInstalled = isInstalled;
            }

            public bool IsInstalled { get; }
        }
    }
}
