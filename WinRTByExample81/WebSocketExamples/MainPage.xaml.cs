namespace WebSocketExamples
{
    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
    using Windows.UI.Core;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private MessageWebSocket socket; 

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += (o, e) => this.Initialize();
        }

        private void Initialize()
        {
            this.Status.Text = "Idle";
            this.Text.Text = string.Empty;
            this.Response.Text = string.Empty;
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            
            if (!string.IsNullOrWhiteSpace(this.Text.Text))
            {
                this.Response.Text = string.Empty;
                this.Status.Text = "Initiating...";
                try
                {
                    if (this.socket == null)
                    {
                        this.socket = new MessageWebSocket();
                        this.socket.Control.MessageType = SocketMessageType.Utf8;
                        this.socket.MessageReceived += this.SocketMessageReceived;
                        this.socket.Closed += async (webSocket, args) =>
                            {
                                await Dispatcher.RunAsync(
                                    CoreDispatcherPriority.Normal,
                                    () =>
                                        {
                                            this.Status.Text = "Socket Closed."; 
                                            if (this.socket != null)
                                            {
                                                this.socket.Dispose();
                                                this.socket = null;
                                            }
                                        });
                            };
                    }

                    this.Status.Text = "Connecting...";
                    await this.socket.ConnectAsync(new Uri("ws://echo.websocket.org", UriKind.Absolute));
                    this.Status.Text = "Connected.";
                    var writer = new DataWriter(this.socket.OutputStream);
                    writer.WriteString(this.Text.Text);
                    this.Status.Text = "Sending Message..."; 
                    await writer.StoreAsync();
                    this.Status.Text = "Message Sent.";
                }
                catch (Exception ex)
                {
                    var status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                    this.Status.Text = string.Format("{0}: {1}", ex.Message, status);
                    if (this.socket != null)
                    {
                        this.socket.Dispose();
                        this.socket = null;
                    }
                }
            }

            ((Button)sender).IsEnabled = true;
        }

        private void SocketMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                        {
                            this.Status.Text = "Message Received.";
                            using (var reader = args.GetDataReader())
                            {
                                reader.UnicodeEncoding = UnicodeEncoding.Utf8;
                                var text = reader.ReadString(reader.UnconsumedBufferLength);
                                this.Response.Text = text;
                            }
                        });
            }
            catch (Exception ex)
            {
                var status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () =>
                        {
                            this.Status.Text = string.Format("{0}: {1}", ex.Message, status);
                        });
            }
            finally
            {
                if (this.socket != null)
                {
                    this.socket.Dispose();
                    this.socket = null;
                }
            }
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            End.IsEnabled = true;
        }

        private void End_OnClick(object sender, RoutedEventArgs e)
        {
            End.IsEnabled = false;
            Start.IsEnabled = true;
        }
    }
}
