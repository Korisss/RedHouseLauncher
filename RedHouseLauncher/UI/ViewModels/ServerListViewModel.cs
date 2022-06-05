using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RedHouseLauncher.UI.Models;

namespace RedHouseLauncher.UI.ViewModels
{
    internal class ServerListViewModel : INotifyPropertyChanged
	{
        private Server selectedServer;
        private ObservableCollection<Server> servers;

        public ServerListViewModel()
        {
            servers = new ObservableCollection<Server>(Server.GetServerListSync());
        }

		public Server SelectedServer
		{
			get
			{
				return selectedServer;
			}

			set
			{
				selectedServer = value;
				OnPropertyChanged("SelectedServer");
			}
		}

		public ObservableCollection<Server> Servers
		{
			get
			{
				return servers;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
	}
}
