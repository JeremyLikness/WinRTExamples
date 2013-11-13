namespace ProximityExample.Data
{
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;

    using Windows.Networking.Proximity;

    public class FoundPeer
    {
        public FoundPeer()
        {
            
        }

        public FoundPeer(PeerInformation peerInformation)
        {
            Information = peerInformation;
            Name = peerInformation.DisplayName;
            if (peerInformation.DiscoveryData.Length > 0)
            {
                Text = Encoding.UTF8.GetString(
                    peerInformation.DiscoveryData.ToArray(),
                    0,
                    (int)peerInformation.DiscoveryData.Length);
            }
        }

        public PeerInformation Information { get; private set; }
        public string Name { get; set; } 
        public string Text { get; set; }

        public override int GetHashCode()
        {
            return this.Information == null ? Name.GetHashCode() : this.Information.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Information != null && this.Information.Equals(obj);
        }
    }
}