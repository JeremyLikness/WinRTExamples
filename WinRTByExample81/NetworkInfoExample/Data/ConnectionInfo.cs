namespace NetworkInfoExample.Data
{
    using System;
    using System.Threading.Tasks;

    using Windows.Networking.Connectivity;

    public class ConnectionInfo
    {
        public string Name { get; set; } 
        public bool IsWwan { get; set; }
        public bool IsWlan { get; set; }
        public long IncomingBitsPerSecond { get; set; }
        public long OutgoingBitsPerSecond { get; set; }
        public string AuthenticationType { get; set; }
        public string ConnectivityLevel { get; set; }
        public string DomainConnectivityLevel { get; set; }
        public string EncryptionType { get; set; }
        public string CostType { get; set; }
        public string Flags { get; set; }
        public ulong BytesSentLastDay { get; set; }
        public ulong BytesReceivedLastDay { get; set; }
        public Guid? NetworkAdapterId { get; set; }
        public string NetworkType { get; set; }
        public short? SignalBars { get; set; }
        public DataPlanInfo DataPlan { get; set; }

        public bool HasDataPlan
        {
            get
            {
                return DataPlan != null;
            }
        }

        public static async Task<ConnectionInfo> FromConnectionProfile(ConnectionProfile profile)
        {
            var connectionInfo = new ConnectionInfo
                                     {
                                         Name = profile.ProfileName,
                                         IsWlan = profile.IsWlanConnectionProfile,
                                         IsWwan = profile.IsWwanConnectionProfile,
                                         ConnectivityLevel =
                                             profile.GetNetworkConnectivityLevel().ToString(),
                                         DomainConnectivityLevel =
                                             profile.GetDomainConnectivityLevel().ToString()
                                     };

            var costType = profile.GetConnectionCost();
            connectionInfo.CostType = costType.NetworkCostType.ToString();
            connectionInfo.Flags = string.Format(
                "{0} {1} {2}",
                costType.ApproachingDataLimit ? "Approaching Data Limit" : string.Empty,
                costType.OverDataLimit ? "Over Data Limit" : string.Empty,
                costType.Roaming ? "Roaming" : string.Empty).Trim();

            connectionInfo.NetworkAdapterId = profile.ServiceProviderGuid;

            if (profile.NetworkAdapter != null)
            {
                connectionInfo.IncomingBitsPerSecond = (long)profile.NetworkAdapter.InboundMaxBitsPerSecond;
                connectionInfo.OutgoingBitsPerSecond = (long)profile.NetworkAdapter.OutboundMaxBitsPerSecond;
                connectionInfo.NetworkType = profile.NetworkAdapter.NetworkItem.GetNetworkTypes().ToString();               
            }

            if (profile.NetworkSecuritySettings != null)
            {
                connectionInfo.AuthenticationType = profile.NetworkSecuritySettings.NetworkAuthenticationType.ToString();
                connectionInfo.EncryptionType = profile.NetworkSecuritySettings.NetworkEncryptionType.ToString();
            }

            connectionInfo.SignalBars = profile.GetSignalBars();

            connectionInfo.DataPlan = DataPlanInfo.FromProfile(profile);
            
            var usage =
                await
                profile.GetNetworkUsageAsync(
                    DateTimeOffset.Now.AddDays(-1),
                    DateTimeOffset.Now,
                    DataUsageGranularity.Total,
                    new NetworkUsageStates { Roaming = TriStates.DoNotCare, Shared = TriStates.DoNotCare });
            
            if (usage != null && usage.Count > 0)
            {
                connectionInfo.BytesReceivedLastDay = usage[0].BytesReceived;
                connectionInfo.BytesSentLastDay = usage[0].BytesSent;
            }
            
            return connectionInfo;
        }
    }
}