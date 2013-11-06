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
            this.Url = "http://csharperimage.jeremylikness.com/"; // http://www.gutenberg.org/files/4300/4300-h/4300-h.htm

            this.LoadUrlCommand = new ActionCommand(obj => this.LoadUrl(), obj => this.CanLoadUrl());
            this.CancelUrlCommand = new ActionCommand(obj => this.CancelUrl(), obj => this.CanCancelUrl());

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.BytesReceived = 500;
                this.Content = "Blah blah blah.";
            }
            else
            {
                this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
                this.client = new HttpClient();
            }
        }

        private readonly CoreDispatcher dispatcher;

        private IProgress<ulong> progress; 

        private ulong bytesReceived;

        private string url;

        private string content;

        private bool loading;

        private readonly HttpClient client;

        private CancellationTokenSource cancellation;

        public ActionCommand LoadUrlCommand { get; private set; }

        public ActionCommand CancelUrlCommand { get; private set; }

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
                    this.Content = "{{ loading }}";
                    this.cancellation = new CancellationTokenSource();
                    var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    this.loading = true;
                    this.LoadUrlCommand.OnCanExecuteChanged();
                    this.CancelUrlCommand.OnCanExecuteChanged();
                    var response = await client.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead).AsTask(cancellation.Token);
                    if (!cancellation.IsCancellationRequested && response.IsSuccessStatusCode)
                    {
                        cancellation.Dispose();
                        cancellation = new CancellationTokenSource();
                        this.progress = new Progress<ulong>(ProgressHandler);
                        var stringContent = await response.Content.ReadAsStringAsync().AsTask(cancellation.Token, this.progress);
                        if (stringContent.Length > 10000)
                        {
                            stringContent = stringContent.Substring(0, 10000);
                        }
                        this.Content = stringContent;
                        this.loading = false;
                        this.cancellation.Dispose();
                        this.cancellation = null;
                        this.LoadUrlCommand.OnCanExecuteChanged();
                        this.CancelUrlCommand.OnCanExecuteChanged();
                        this.progress = null;
                    }
                }
            }
        }

        private void ProgressHandler(ulong progressArgs)
        {
            Action<ulong> update = p =>
                {
                    this.BytesReceived = p;                    
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
                this.loading = false;
                this.cancellation = null;
                this.progress = null;
                this.Content = "Cancelled.";
                this.BytesReceived = 0;
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