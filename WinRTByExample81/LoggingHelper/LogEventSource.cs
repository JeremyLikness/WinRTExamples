namespace LoggingHelper
{
    using System.Diagnostics.Tracing;
    using System.Runtime.CompilerServices;

    public sealed class LogEventSource : EventSource
    {
        private static readonly LogEventSource LogSource = new LogEventSource();
        private const string UnknownMember = "UNKNOWN";

        public const int VerboseLevel= 1, InformationalLevel = 2, WarningLevel = 3, ErrorLevel = 4, CriticalLevel = 5;

        public static LogEventSource Log
        {
            get
            {
                return LogSource;
            }
        }

        [Event(VerboseLevel, Level = EventLevel.Verbose)]
        public void Debug(string message, [CallerMemberName]string member = UnknownMember)
        {
            this.WriteEvent(VerboseLevel, Format(message, member));
        }

        [Event(InformationalLevel, Level = EventLevel.Informational)]
        public void Info(string message, [CallerMemberName]string member = UnknownMember)
        {
            this.WriteEvent(InformationalLevel, Format(message, member));
        }

        [Event(WarningLevel, Level = EventLevel.Warning)]
        public void Warn(string message, [CallerMemberName]string member = UnknownMember)
        {
            this.WriteEvent(WarningLevel, Format(message, member));
        }

        [Event(ErrorLevel, Level = EventLevel.Error)]
        public void Error(string message, [CallerMemberName]string member = UnknownMember)
        {
            this.WriteEvent(ErrorLevel, Format(message, member));
        }

        [Event(CriticalLevel, Level = EventLevel.Critical)]
        public void Critical(string message, [CallerMemberName]string member = UnknownMember)
        {
            this.WriteEvent(CriticalLevel, Format(message, member));
        }

        private static string Format(string message, string member)
        {
            return string.Format("{0},{1}", member, message);
        }
    }
}
