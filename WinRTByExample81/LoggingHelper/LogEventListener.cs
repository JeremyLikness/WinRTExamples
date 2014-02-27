namespace LoggingHelper
{
    using System;
    using System.Diagnostics.Tracing;
    using System.Threading;

    using Windows.ApplicationModel;
    using Windows.Storage;

    public sealed class LogEventListener : EventListener
    {
        private const string DefaultTimeFormat = "{0:yyyy-MM-dd HH\\:mm\\:ss\\:ffff}";
        private IStorageFile logFile;

        private readonly string logName;

        private readonly string timeFormat;

        private readonly SemaphoreSlim semaphore;

        public LogEventListener()
            : this(Package.Current.Id.Name)
        {
            
        }

        public LogEventListener(string name)
            : this(name, DefaultTimeFormat)
        {
            
        }

        public LogEventListener(string name, string timeFormat)
        {
            this.logName = string.Format("{0}_log.csv", name.Replace(" ", "_"));
            this.timeFormat = timeFormat;
            this.semaphore = new SemaphoreSlim(1);
            this.SetupLogFile();
        }

        private async void SetupLogFile()
        {
            this.logFile =
                await
                ApplicationData.Current.LocalFolder.CreateFileAsync(this.logName, CreationCollisionOption.OpenIfExists);
        }

        private async void Write(string info)
        {
            await this.semaphore.WaitAsync();
            try
            {
                await FileIO.AppendLinesAsync(this.logFile, new[] { info });
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            var eventInfo =
            String.Format(
                "{0},{1},{2}",
                string.Format(this.timeFormat, DateTime.Now),
                eventData.Level,
                eventData.Payload[0]);
            this.Write(eventInfo);
        }
    }
}