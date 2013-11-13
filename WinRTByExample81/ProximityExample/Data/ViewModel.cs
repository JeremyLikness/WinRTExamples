namespace ProximityExample.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading.Tasks;

    using Windows.Networking.Proximity;
    using Windows.Networking.Sockets;
    using Windows.UI.Core;

    using ViewModelHelper;

    public class ViewModel : BaseViewModel
    {
        private const string DiscoveryText = "WinRT By Example Proximity";

        private readonly Action<Action> routeToUiThread = action => action();

        private readonly CoreDispatcher dispatcher;

        private readonly bool isDesignTime;

        public ViewModel()
        {
            this.Peers = new ObservableCollection<FoundPeer>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.isDesignTime = true;
                this.errorMessage = "Design-time error message.";
                this.Peers.Add(new FoundPeer { Name = "Design Peer"} );
                this.Peers.Add(new FoundPeer { Name = "Another Design Peer", Text = "Some Discovery Text"} );
                this.selectedPeer = this.Peers[0];
                this.connectedPeer = "Design-time Connected Peer";
            }
            else
            {
                this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
                this.routeToUiThread = action => this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
                this.supportsTriggeredConnect = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Triggered) > 0;
                this.supportsBrowseConnect = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) > 0;
            }

            Connect = new ActionCommand(async Object => await this.ConnectCommand(), obj => this.SelectedPeer != null && !this.IsConnecting);
            Browse = new ActionCommand(async obj => await this.BrowseCommand(), obj => this.IsAdvertising && !this.IsBrowsing && !this.IsConnecting);
            StartAdvertising = new ActionCommand(obj => this.StartAdvertisingCommand(), obj => this.SupportsPeer && !this.IsAdvertising);
            StopAdvertising = new ActionCommand(obj => this.StopAdvertisingCommand(), obj => this.IsAdvertising && !this.IsConnecting);
        }

        private bool isAdvertising, isBrowsing, isConnecting, supportsTriggeredConnect, supportsBrowseConnect;

        private string errorMessage, connectedPeer;

        private StreamSocket peerConnectionSocket;

        private FoundPeer selectedPeer; 

        public ActionCommand StartAdvertising { get; private set; }

        public ActionCommand StopAdvertising { get; private set; }

        public ActionCommand Browse { get; private set; }

        public ActionCommand Connect { get; private set; }

        public ObservableCollection<FoundPeer> Peers { get; private set; } 
        
        public bool IsAdvertising
        {
            get
            {
                return this.isAdvertising;
            }
            set
            {
                if (this.isAdvertising == value)
                {
                    return;
                }

                if (value)
                {
                    this.StartPeerFinder();
                }
                else
                {
                    this.StopPeerFinder();
                }

                this.isAdvertising = value;
                this.OnPropertyChanged();
                this.StartAdvertising.OnCanExecuteChanged();
                this.StopAdvertising.OnCanExecuteChanged();
                this.Browse.OnCanExecuteChanged();
            }
        }

        public bool IsBrowsing
        {
            get
            {
                return this.isBrowsing;
            }

            set
            {
                this.isBrowsing = value;
                this.OnPropertyChanged();
                this.Browse.OnCanExecuteChanged();
            }
        }

        public bool IsConnecting
        {
            get
            {
                return this.isConnecting;
            }

            set
            {
                this.isConnecting = value;
                this.OnPropertyChanged();
                this.Connect.OnCanExecuteChanged();
                this.Browse.OnCanExecuteChanged();
                this.StopAdvertising.OnCanExecuteChanged();
            }
        }

        public bool SupportsTriggeredConnect
        {
            get
            {
                return this.supportsTriggeredConnect;
            }

            set
            {
                this.supportsTriggeredConnect = value;
                this.OnPropertyChanged();                
                this.OnPropertyChanged("SupportsPeer");
            }
        }

        public bool SupportsBrowseConnect
        {
            get
            {
                return this.supportsBrowseConnect;
            }

            set
            {
                this.supportsBrowseConnect = value;
                this.OnPropertyChanged();             
                this.OnPropertyChanged("SupportsPeer");
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }

            set
            {
                this.errorMessage = value;
                this.OnPropertyChanged();
            }
        }

        public string ConnectedPeer
        {
            get
            {
                return this.connectedPeer;
            }

            set
            {
                this.connectedPeer = value;
                this.OnPropertyChanged();
            }
        }

        public bool SupportsPeer
        {
            get
            {
                return this.supportsTriggeredConnect || this.supportsBrowseConnect;
            }
        }

        public FoundPeer SelectedPeer
        {
            get
            {
                return this.selectedPeer;
            }

            set
            {
                this.selectedPeer = value;
                this.OnPropertyChanged();
                this.Connect.OnCanExecuteChanged();
            }
        }

        public override bool IsDesignTime
        {
            get
            {
                return this.isDesignTime;
            }
        }

        public override Action<Action> RouteToUiThread
        {
            get
            {
                return this.routeToUiThread;
            }
        }

        private void StartAdvertisingCommand()
        {
            if (this.IsAdvertising)
            {
                this.IsAdvertising = false;
            }

            this.IsAdvertising = true;            
        }

        private void StopAdvertisingCommand()
        {
            if (this.IsAdvertising)
            {
                this.IsAdvertising = false;                
            }
        }

        private void StartPeerFinder()
        {
            this.ErrorMessage = string.Empty;

            try
            {
                PeerFinder.TriggeredConnectionStateChanged += PeerFinder_TriggeredConnectionStateChanged;
                PeerFinder.ConnectionRequested += this.PeerFinderConnectionRequested;
                
                PeerFinder.Role = PeerRole.Peer;
                PeerFinder.DiscoveryData = Encoding.UTF8.GetBytes(DiscoveryText).AsBuffer();
                
                PeerFinder.Start();
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
        }

        private void StopPeerFinder()
        {
            this.ErrorMessage = string.Empty;

            try
            {
                PeerFinder.TriggeredConnectionStateChanged -= this.PeerFinder_TriggeredConnectionStateChanged;
                PeerFinder.ConnectionRequested -= this.PeerFinderConnectionRequested;
                PeerFinder.Stop();
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
        }

        private async Task BrowseCommand()
        {
            this.IsBrowsing = true;
            this.ErrorMessage = string.Empty;

            if (this.peerConnectionSocket != null)
            {
                this.peerConnectionSocket.Dispose();
                this.peerConnectionSocket = null;
                this.ConnectedPeer = null;
            }

            this.SelectedPeer = null;

            this.Peers.Clear();
            try
            {
                var peers = await PeerFinder.FindAllPeersAsync();

                if (peers != null && peers.Count > 0)
                {
                    foreach (var peer in peers)
                    {
                        this.Peers.Add(new FoundPeer(peer));
                    }                    
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
            this.IsBrowsing = false;
        }

        private async Task ConnectCommand()
        {
            this.IsConnecting = true;

            try
            {
                if (this.peerConnectionSocket != null)
                {
                    this.peerConnectionSocket.Dispose();
                    this.peerConnectionSocket = null;
                }

                this.ConnectedPeer = "Connecting...";

                this.peerConnectionSocket = await PeerFinder.ConnectAsync(this.SelectedPeer.Information);
                this.ConnectedPeer = this.SelectedPeer.Name; 
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                this.ConnectedPeer = "Connection Failed.";
            }

            this.IsConnecting = false;
        }

        private async void PeerFinderConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            if (this.isConnecting)
            {
                return; 
            }

            try
            {
                this.RouteToUiThread(
                    () =>
                        {
                            this.IsConnecting = true;
                            this.ConnectedPeer = string.Format(
                                "Incoming request from {0}...",
                                args.PeerInformation.DisplayName);
                        });                

                if (this.peerConnectionSocket != null)
                {
                    this.peerConnectionSocket.Dispose();
                    this.peerConnectionSocket = null;
                }

                this.peerConnectionSocket = await PeerFinder.ConnectAsync(args.PeerInformation);
                this.RouteToUiThread(
                    () =>
                        {
                            this.IsConnecting = false;
                            var peer =
                                this.Peers.FirstOrDefault(p => p.Information.Id == args.PeerInformation.Id);
                            if (peer == null)
                            {
                                peer = new FoundPeer(args.PeerInformation);
                                this.Peers.Add(peer);
                            }
                            this.SelectedPeer = peer;
                            this.ConnectedPeer = peer.Name;                            
                        });
            }
            catch (Exception ex)
            {
                this.RouteToUiThread(() => this.ErrorMessage = ex.Message);
            }
        }

        private void PeerFinder_TriggeredConnectionStateChanged(object sender, TriggeredConnectionStateChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

    }
}
