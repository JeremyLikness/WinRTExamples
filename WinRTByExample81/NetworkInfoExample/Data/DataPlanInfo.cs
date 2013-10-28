namespace NetworkInfoExample.Data
{
    using System;

    using Windows.Networking.Connectivity;

    public class DataPlanInfo
    {
        public long MegabytesUsed { get; set; }
        public DateTimeOffset? LastSyncTime { get; set; }
        public DateTimeOffset? NextBillingCycle { get; set; }
        public ulong InboundBitsPerSecond { get; set; }
        public ulong OutboundBitsPerSecond { get; set; }
        public long DataLimitMegabytes { get; set; }

        public static DataPlanInfo FromProfile(ConnectionProfile profile)
        {
            var planStatus = profile.GetDataPlanStatus();
            if (planStatus == null || (planStatus.DataLimitInMegabytes == null && planStatus.DataPlanUsage == null && planStatus.InboundBitsPerSecond == null && 
                planStatus.MaxTransferSizeInMegabytes == null && planStatus.NextBillingCycle == null && planStatus.OutboundBitsPerSecond == null))
            {
                return null;
            }
            var dataPlan = new DataPlanInfo
                               {
                                   DataLimitMegabytes = planStatus.DataLimitInMegabytes ?? 0,
                                   InboundBitsPerSecond = planStatus.InboundBitsPerSecond ?? 0,
                                   OutboundBitsPerSecond = planStatus.OutboundBitsPerSecond ?? 0,
                                   NextBillingCycle = planStatus.NextBillingCycle
                               };

            if (planStatus.DataPlanUsage != null)
            {
                dataPlan.MegabytesUsed = planStatus.DataPlanUsage.MegabytesUsed;
                dataPlan.LastSyncTime = planStatus.DataPlanUsage.LastSyncTime;
            }
            else
            {
                dataPlan.LastSyncTime = null;
            }

            return dataPlan;
        }
    }
}