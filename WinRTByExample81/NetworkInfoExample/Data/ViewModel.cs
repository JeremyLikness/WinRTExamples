namespace NetworkInfoExample.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Windows.Networking.Connectivity;

    public class ViewModel : INotifyPropertyChanged 
    {
        public List<ConnectionInfo> ConnectionProfiles { get; set; }

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

            ConnectionProfiles =
                NetworkInformation.GetConnectionProfiles().Select(ConnectionInfo.FromConnectionProfile).ToList();
            var internet = NetworkInformation.GetInternetConnectionProfile();
            var profile =
                ConnectionProfiles.FirstOrDefault(p => p.NetworkAdapterId == internet.NetworkAdapter.NetworkAdapterId);
            if (internet != null && profile == null)
            {
                var internetInfo = ConnectionInfo.FromConnectionProfile(internet);
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
                var connectionInfo = new ConnectionInfo();
                connectionInfo.AuthenticationType = RandomItem(AuthenticationTypes, random);
                connectionInfo.EncryptionType = RandomItem(EncryptionTypes, random);
                connectionInfo.ConnectivityLevel = RandomItem(ConnectivityLevels, random);
                connectionInfo.DomainConnectivityLevel = RandomItem(DomainLevels, random);
                connectionInfo.IncomingBitsPerSecond = random.Next(0, int.MaxValue);
                connectionInfo.OutgoingBitsPerSecond = random.Next(0, int.MaxValue);
                connectionInfo.IsWlan = random.Next(0, 1) == 0;
                connectionInfo.IsWwan = random.Next(0, 1) == 0;
                connectionInfo.Name = Guid.NewGuid().ToString().Replace('-', ' ');
                connectionInfo.NetworkAdapterId = Guid.NewGuid();
                connectionInfo.NetworkType = RandomItem(NetworkTypeList, random);
                connectionInfo.CostType = RandomItem(CostTypes, random);
                connectionInfo.Flags =
                    string.Format(
                        "{0} {1}",
                        random.Next(0, 1) == 0
                            ? (random.Next(0, 1) == 0 ? "Approaching Data Limit" : "Over Data Limit")
                            : string.Empty,
                        random.Next(0, 1) == 0 ? "Roaming" : string.Empty).Trim();
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