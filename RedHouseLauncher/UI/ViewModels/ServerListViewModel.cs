using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RedHouseLauncher.UI.Models;

namespace RedHouseLauncher.UI.ViewModels
{
    internal class ServerListViewModel : INotifyPropertyChanged
    {
        private Server selectedServer;

        public ServerListViewModel()
        {
            Servers = new ObservableCollection<Server>(Server.GetServerListSync());
            selectedServer = Servers[0];
        }

        public Server SelectedServer
        {
            get => selectedServer;

            set
            {
                selectedServer = value;
                OnPropertyChanged("SelectedServer");
            }
        }

        public ObservableCollection<Server> Servers { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
