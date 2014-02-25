namespace LoggingHelper
{
    using System.Diagnostics.Tracing;

    public sealed class LogEventSource : EventSource
    {
        private static readonly LogEventSource LogSource = new LogEventSource();

        public const int VerboseLevel= 1, InformationalLevel = 2, WarningLevel = 3, ErrorLevel = 4, CriticalLevel = 5;

        public static LogEventSource Log
        {
            get
            {
                return LogSource;
            }
        }

        [Event(VerboseLevel, Level = EventLevel.Verbose)]
        public void Debug(string message)
        {
            this.WriteEvent(VerboseLevel, message);
        }

        [Event(InformationalLevel, Level = EventLevel.Informational)]
        public void Info(string message)
        {
            this.WriteEvent(InformationalLevel, message);
        }

        [Event(WarningLevel, Level = EventLevel.Warning)]
        public void Warn(string message)
        {
            this.WriteEvent(WarningLevel, message);
        }

        [Event(ErrorLevel, Level = EventLevel.Error)]
        public void Error(string message)
        {
            this.WriteEvent(ErrorLevel, message);
        }

        [Event(CriticalLevel, Level = EventLevel.Critical)]
        public void Critical(string message)
        {
            this.WriteEvent(CriticalLevel, message);
        }
    }
}
