namespace SocketsGame
{
    using System;
    using System.Threading.Tasks;

    using Windows.Networking;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
    using Windows.UI.Core;
    using Windows.UI.Xaml;

    using SocketsGame.Data;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly Parser parser = new Parser();

        private StreamSocketListener serverSocket;

        private DataWriter serverWriter;

        private DataReader serverReader; 

        private StreamSocket clientSocket;

        private DataWriter clientWriter;

        private DataReader clientReader;

        private const string ServiceName = "21212";

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += (o,e) => this.OnLoaded();            
        }

        private async void OnLoaded()
        {
            this.Command.Focus(FocusState.Keyboard);

            this.ConsoleText.SizeChanged += (o, e) => this.ServerConsole.ChangeView(0, double.MaxValue, null);
            this.GameText.SizeChanged += (o, e) => this.GameConsole.ChangeView(0, double.MaxValue, null);           
            
            try
            {
                this.serverSocket = new StreamSocketListener();
                this.serverSocket.ConnectionReceived += this.ServerSocketConnectionReceived;
                await this.serverSocket.BindServiceNameAsync(ServiceName);   
                this.SafeAddText(s => ConsoleText.Text += s, "Server is listening...");
            }
            catch (Exception ex)
            {
                this.ConsoleText.Text = GetErrorText(ex);
                throw;
            }

            try
            {
                var hostName = new HostName("localhost");
                this.clientSocket = new StreamSocket();
                await this.clientSocket.ConnectAsync(hostName, ServiceName);
                clientWriter = new DataWriter(this.clientSocket.OutputStream);
                clientReader = new DataReader(this.clientSocket.InputStream);
                this.SafeAddText(s => GameText.Text += s, "Connected to server!");
                var longRunningTask = Task.Factory.StartNew(ClientListener, null, TaskCreationOptions.LongRunning);
                await SendString(clientWriter, "look");                
            }
            catch (Exception ex)
            {
                this.GameText.Text = GetErrorText(ex);
                throw;
            }
        }

        private async void Go_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Command.Text))
            {
                return;
            }

            var stringToSend = this.Command.Text;

            try
            {
                await SendString(clientWriter, stringToSend);
            }
            catch (Exception ex)
            {
                this.SafeAddText(s => this.GameText.Text += s, GetErrorText(ex));
            }

            this.Command.Text = string.Empty;
            this.Command.Focus(FocusState.Keyboard);
        }

        private async void ServerSocketConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            if (serverWriter == null)
            {
                serverWriter = new DataWriter(args.Socket.OutputStream);
                serverReader = new DataReader(args.Socket.InputStream);
            }

            try
            {
                while (true)
                {
                    var data = await GetStringFromReader(serverReader);
                    this.SafeAddText(s => this.ConsoleText.Text += s, string.Format("Received: {0}", data));
                    
                    var reply = parser.Parse(data, App.World);

                    this.SafeAddText(
                        s => this.ConsoleText.Text += s,
                        string.Format("Responded with: {0}...", reply.Length < 80 ? reply : reply.Substring(0, 80)));

                    await SendString(serverWriter, reply);
                }
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                this.SafeAddText(s => this.ConsoleText.Text += s, GetErrorText(exception));
            }            
        }

        private async void ClientListener(object state)
        {
            try
            {
                while (true)
                {
                    var data = await GetStringFromReader(clientReader);
                    this.SafeAddText(s => this.GameText.Text += s, data);     
                }
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                this.SafeAddText(s => this.GameText.Text += s, GetErrorText(exception));
            }
        }

        private void SafeAddText(Action<string> assignment, string text)
        {
            Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () => assignment(string.Format("{0}{1}", Environment.NewLine, text)));
        }

        private static string GetErrorText(Exception ex)
        {
            return string.Format("{0} ({1})", ex.Message, SocketError.GetStatus(ex.GetBaseException().HResult));
        }

        private static async Task SendString(IDataWriter writer, string text)
        {
            writer.WriteUInt32(writer.MeasureString(text));
            writer.WriteString(text);
            await writer.StoreAsync();
        }        

        private static async Task<string> GetStringFromReader(IDataReader reader)
        {
            // Read first 4 bytes (length of the subsequent string).
            var sizeFieldCount = await reader.LoadAsync(sizeof(uint));
            if (sizeFieldCount != sizeof(uint))
            {
                // The underlying socket was closed before we were able to read the whole data.
                return string.Empty;
            }

            // Read the string.
            var stringLength = reader.ReadUInt32();
            var actualStringLength = await reader.LoadAsync(stringLength);
            if (stringLength != actualStringLength)
            {
                // The underlying socket was closed before we were able to read the whole data.
                return string.Empty;
            }

            var data = reader.ReadString(actualStringLength);
            return data;
        }
    }
}
