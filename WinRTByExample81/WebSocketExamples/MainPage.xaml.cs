namespace WebSocketExamples
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading.Tasks;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
    using Windows.UI.Core;

    using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private MessageWebSocket socket;

        private StreamWebSocket streamSocket;

        private readonly Uri echoService = new Uri("ws://echo.websocket.org", UriKind.Absolute);

        public MainPage()
        {
            this.InitializeComponent();            
        }

        /// <summary>
        /// Logic to send a simple <see cref="MessageWebSocket"/> message
        /// </summary>
        /// <param name="sender">The button</param>
        /// <param name="e">Event arguments</param>
        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // avoid multiple clicks
            ((Button)sender).IsEnabled = false;

            // make sure we have something to send 
            if (!string.IsNullOrWhiteSpace(this.Text.Text))
            {
                this.Response.Text = string.Empty;
                this.Status.Text = "Initiating...";
                try
                {
                    // reuse socket if already exists
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
                    await this.socket.ConnectAsync(echoService);
                    this.Status.Text = "Connected.";
                    var writer = new DataWriter(this.socket.OutputStream);
                    writer.WriteString(this.Text.Text);
                    this.Status.Text = "Sending Message...";
                    await writer.StoreAsync();
                    this.Status.Text = "Message Sent.";
                }
                catch (Exception ex)
                {
                    this.Status.Text = ToErrorMessage(ex);
                    if (this.socket != null)
                    {
                        this.socket.Dispose();
                        this.socket = null;
                    }
                }
            }

            // turn it back on 
            ((Button)sender).IsEnabled = true;
        }

        // When it is echoed back
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
                Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => this.Status.Text = ToErrorMessage(ex));
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

        // Start sending primes to the echo service
        private async void Start_OnClick(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            End.IsEnabled = true;
            this.Primes.Text = string.Empty;

            if (this.streamSocket != null)
            {
                return;
            }

            try
            {
                this.streamSocket = new StreamWebSocket();

                this.streamSocket.Closed +=
                    async (senderSocket, args) =>
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, this.CloseStream);
                        };

                await this.streamSocket.ConnectAsync(echoService);

                // we just let these run for the purposes of the demo
                Task.Factory.StartNew(this.ComputePrimes, this.streamSocket.OutputStream, TaskCreationOptions.LongRunning);

                // same here
                Task.Factory.StartNew(
                    this.ReceiveEcho,
                    this.streamSocket.InputStream.AsStreamForRead(),
                    TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                this.CloseStream();
                var status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                this.Primes.Text = string.Format("{0}: {1}", ex.Message, status);
            }
        }

        // stop it! 
        private void End_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.streamSocket != null)
                {
                    streamSocket.Close(1000, "Closed per user request.");
                    this.CloseStream();
                }
            }
            catch (Exception ex)
            {
                var status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                this.Primes.Text = string.Format("{0}: {1}", ex.Message, status);
            }
        }

        // clean up
        private void CloseStream()
        {
            Start.IsEnabled = true;
            End.IsEnabled = false;

            if (this.streamSocket != null)
            {
                this.streamSocket.Dispose();
                this.streamSocket = null;
            }
        }

        // long-running task, waits a millisecond and tests each number for a prime, then waits a full second after each successful prime 
        private async void ComputePrimes(object state)
        {
            var x = 0;

            try
            {
                var outputStream = (IOutputStream)state;

                while (x < int.MaxValue)
                {
                    x++;
                    if (IsPrime(x))
                    {
                        var array = Encoding.UTF8.GetBytes(string.Format(" {0} ", x));
                        await outputStream.WriteAsync(array.AsBuffer());
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                this.DispatchTextToPrimes("... and writing stopped.");                
            }
            catch (Exception ex)
            {
                this.DispatchTextToPrimes(string.Format("... send error: {0}", ToErrorMessage(ex)));                
            }
        }

        // this receives the echo from the serivce and shows it in the output 
        private async void ReceiveEcho(object state)
        {
            try
            {
                var stream = (Stream)state;
                var buffer = new byte[1000];
                
                while (true)
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        var text = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        this.DispatchTextToPrimes(text);                        
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                this.DispatchTextToPrimes("... and writing stopped.");                
            }
            catch (Exception ex)
            {
                this.DispatchTextToPrimes(string.Format("... receive error: {0}", ToErrorMessage(ex)));                
            }
        }

        // convert the cryptic message thrown from the socket to something more readable
        private static string ToErrorMessage(Exception ex)
        {
            var status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
            return string.Format("{0} ({1})", ex.Message, status);
        }

        // common dispatcher for updating the prime field
        private void DispatchTextToPrimes(string text)
        {
            Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                    {
                        var oldText = this.Primes.Text;
                        this.Primes.Text = string.Format("{0} {1}", oldText, text);
                    });
        }

        // is x a prime?
        private static bool IsPrime(int x)
        {
            if ((x & 1) == 0)
            {
                return x == 2;
            }

            for (var i = 3; (i * i) <= x; i += 2)
            {
                if ((x % i) == 0)
                {
                    return false;
                }
            }

            return x != 1;
        }
    }
}