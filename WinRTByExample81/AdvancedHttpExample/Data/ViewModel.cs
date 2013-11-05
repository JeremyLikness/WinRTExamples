namespace AdvancedHttpExample.Data
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.UI.Core;
    using Windows.Web.Http;

    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            this.Url = "http://csharperimage.jeremylikness.com/";

            this.LoadUrlCommand = new ActionCommand(obj => this.LoadUrl(), obj => this.CanLoadUrl());
            this.CancelUrlCommand = new ActionCommand(obj => this.CancelUrl(), obj => this.CanCancelUrl());

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.BytesSent = 1000;
                this.BytesToSend = 10000;
                this.BytesReceived = 500;
                this.BytesToReceive = 1000;
                this.Content = "Blah blah blah.";
            }
            else
            {
                this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            }
        }

        private readonly CoreDispatcher dispatcher;

        private IProgress<HttpProgress> progress; 

        private ulong bytesSent;

        private ulong bytesToSend; 

        private ulong bytesReceived;

        private ulong bytesToReceive;

        private string url;

        private string content;

        private bool loading;

        private HttpClient client;

        private CancellationTokenSource cancellation;

        public ActionCommand LoadUrlCommand { get; private set; }

        public ActionCommand CancelUrlCommand { get; private set; }

        public ulong BytesSent
        {
            get
            {
                return this.bytesSent;
            }
            set
            {
                this.bytesSent = value;
                this.OnPropertyChanged();
            }
        }

        public ulong BytesToSend
        {
            get
            {
                return this.bytesToSend;
            }
            set
            {
                this.bytesToSend = value;
                this.OnPropertyChanged();
            }
        }

        public ulong BytesReceived
        {
            get
            {
                return this.bytesReceived;
            }
            set
            {
                this.bytesReceived = value;
                this.OnPropertyChanged();
            }
        }

        public ulong BytesToReceive
        {
            get
            {
                return this.bytesToReceive;
            }
            set
            {
                this.bytesToReceive = value;
                this.OnPropertyChanged();
            }
        }

        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
                this.OnPropertyChanged();
            }
        }

        public string Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.content = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool CanLoadUrl()
        {
            if (this.loading)
            {
                return false;
            }

            Uri uri;
            return Uri.TryCreate(this.Url, UriKind.Absolute, out uri);
        }

        private async Task LoadUrl()
        {
            if (this.CanLoadUrl())
            {
                Uri uri;
                if (Uri.TryCreate(this.Url, UriKind.Absolute, out uri))
                {
                    this.client = new HttpClient();
                    this.cancellation = new CancellationTokenSource();
                    var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    this.progress = new Progress<HttpProgress>(ProgressHandler);
                    this.loading = true;
                    this.LoadUrlCommand.OnCanExecuteChanged();
                    this.CancelUrlCommand.OnCanExecuteChanged();
                    var response = await client.SendRequestAsync(request).AsTask(cancellation.Token, progress);
                    if (!cancellation.IsCancellationRequested)
                    {
                        this.Content = await response.Content.ReadAsStringAsync();
                        this.loading = false;
                        this.cancellation.Dispose();
                        this.cancellation = null;
                        this.client.Dispose();
                        this.client = null;
                        this.LoadUrlCommand.OnCanExecuteChanged();
                        this.CancelUrlCommand.OnCanExecuteChanged();
                        this.progress = null;
                    }
                }
            }
        }

        private void ProgressHandler(HttpProgress progressArgs)
        {
            Action<HttpProgress> update = p =>
                {
                    this.bytesReceived = p.BytesReceived;
                    this.bytesToReceive = p.TotalBytesToReceive ?? 0;
                    this.bytesSent = p.BytesSent;
                    this.bytesToSend = p.TotalBytesToSend ?? 0;
                };

            if (this.dispatcher.HasThreadAccess)
            {
                update(progressArgs);
            }
            else
            {
                this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => update(progressArgs));
            }
        }

        private bool CanCancelUrl()
        {
            return this.loading && this.cancellation != null;
        }

        private void CancelUrl()
        {
            if (this.CanCancelUrl())
            {
                this.cancellation.Cancel();
                this.cancellation.Dispose();
                this.client.Dispose();
                this.client = null;
                this.loading = false;
                this.cancellation = null;
                this.progress = null;
                this.Content = "Cancelled.";
                this.LoadUrlCommand.OnCanExecuteChanged();
                this.CancelUrlCommand.OnCanExecuteChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}