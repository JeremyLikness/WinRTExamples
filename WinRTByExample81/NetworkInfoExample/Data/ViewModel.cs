namespace NetworkInfoExample.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Windows.Networking.Connectivity;
    using Windows.UI.Core;

    using NetworkInfoExample.Common;

    public class ViewModel : INotifyPropertyChanged 
    {
        public List<ConnectionInfo> ConnectionProfiles { get; set; }

        public IDialog Dialog { get; set; }

        private CoreDispatcher Dispatcher { get; set; }

        private static readonly NetworkAuthenticationType[] AuthenticationTypes = EnumList<NetworkAuthenticationType>();

        private static readonly NetworkEncryptionType[] EncryptionTypes = EnumList<NetworkEncryptionType>();

        private static readonly NetworkTypes[] NetworkTypeList = EnumList<NetworkTypes>();

        private static readonly NetworkConnectivityLevel[] ConnectivityLevels = EnumList<NetworkConnectivityLevel>();

        private static readonly DomainConnectivityLevel[] DomainLevels = EnumList<DomainConnectivityLevel>();

        private static readonly NetworkCostType[] CostTypes = EnumList<NetworkCostType>();

        private ConnectionInfo currentConnectionInfo;

        public ViewModel()
        {
            ConnectionProfiles = new List<ConnectionInfo>();
            
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                GenerateDesignMode();
                return;
            }

            Dispatcher = CoreWindow.GetForCurrentThread().Dispatcher; 
            Dialog = new DialogHandler(Dispatcher);

            NetworkInformation.NetworkStatusChanged += this.NetworkInformationNetworkStatusChanged;
        }

        public async Task Initialize()
        {
            await this.UpdateNetworkInformation();
        }

        private async void NetworkInformationNetworkStatusChanged(object sender)
        {
            await Dialog.ShowMessageAsync("The network status has changed. Network information will be refreshed.");
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await this.UpdateNetworkInformation());
        }

        private async Task UpdateNetworkInformation()
        {
            var profiles = NetworkInformation.GetConnectionProfiles();
            ConnectionProfiles = new List<ConnectionInfo>();
            foreach (var connectionProfile in profiles)
            {
                ConnectionProfiles.Add(await ConnectionInfo.FromConnectionProfile(connectionProfile));
            }
            PropertyChanged(this, new PropertyChangedEventArgs("ConnectionProfiles"));            
            var internet = NetworkInformation.GetInternetConnectionProfile();
            var profile =
                internet == null ? null : ConnectionProfiles.FirstOrDefault(p => p.Name == internet.ProfileName);
            if (internet != null && profile == null)
            {
                var internetInfo = await ConnectionInfo.FromConnectionProfile(internet);
                ConnectionProfiles.Add(internetInfo);
                CurrentConnectionInfo = internetInfo;
                return;
            }
            this.CurrentConnectionInfo = profile ?? this.ConnectionProfiles.FirstOrDefault();
            
        }

        public ConnectionInfo CurrentConnectionInfo
        {
            get
            {
                return this.currentConnectionInfo;
            }
            set
            {
                this.currentConnectionInfo = value;
                this.OnPropertyChanged();
            }
        }

        private void GenerateDesignMode()
        {
            var random = new Random();
            for (var x = 0; x < 10; x++)
            {
                var connectionInfo = new ConnectionInfo
                                         {
                                             AuthenticationType = RandomItem(AuthenticationTypes, random),
                                             EncryptionType = RandomItem(EncryptionTypes, random),
                                             ConnectivityLevel = RandomItem(ConnectivityLevels, random),
                                             DomainConnectivityLevel = RandomItem(DomainLevels, random),
                                             IncomingBitsPerSecond = random.Next(0, int.MaxValue),
                                             OutgoingBitsPerSecond = random.Next(0, int.MaxValue),
                                             IsWlan = random.Next(0, 1) == 0,
                                             IsWwan = random.Next(0, 1) == 0,
                                             Name = Guid.NewGuid().ToString().Replace('-', ' '),
                                             NetworkAdapterId = Guid.NewGuid(),
                                             NetworkType = RandomItem(NetworkTypeList, random),
                                             CostType = RandomItem(CostTypes, random),
                                             SignalBars = (short)random.Next(0, 5),
                                             Flags =
                                                 string.Format(
                                                     "{0} {1}",
                                                     random.Next(0, 1) == 0
                                                         ? (random.Next(0, 1) == 0
                                                                ? "Approaching Data Limit"
                                                                : "Over Data Limit")
                                                         : string.Empty,
                                                     random.Next(0, 1) == 0 ? "Roaming" : string.Empty)
                                                 .Trim()
                                         };
                if (random.Next(0, 1) == 0)
                {
                    var dataPlan = new DataPlanInfo
                                       {
                                           DataLimitMegabytes = random.Next(0, int.MaxValue),
                                           InboundBitsPerSecond = (ulong)random.Next(0, int.MaxValue),
                                           OutboundBitsPerSecond = (ulong)random.Next(0, int.MaxValue)
                                       };
                    if (random.Next(0, 1) == 0)
                    {
                        dataPlan.NextBillingCycle = DateTime.Now.AddDays(random.Next(1, 30));
                    }
                    if (random.Next(0, 1) == 0)
                    {
                        dataPlan.MegabytesUsed = random.Next(0, int.MaxValue);
                        dataPlan.LastSyncTime = DateTime.Now.AddHours(-1 * random.Next(2, 12));
                    }
                    connectionInfo.DataPlan = dataPlan; 
                }
                connectionInfo.BytesReceivedLastDay = (ulong)random.Next(0, int.MaxValue);
                connectionInfo.BytesSentLastDay = (ulong)random.Next(0, int.MaxValue);
                ConnectionProfiles.Add(connectionInfo);
            }
            CurrentConnectionInfo = ConnectionProfiles[random.Next(0, ConnectionProfiles.Count)];
        }

        private static T[] EnumList<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
   

        private static string RandomItem<T>(IList<T> list, Random random)
        {
            return list[random.Next(0, list.Count)].ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));            
        }
    }
}