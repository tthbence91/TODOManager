using System.Diagnostics;
using Microsoft.Extensions.Logging;
namespace TodoManagerTests.Fakes
{
    public class FakeLogger<T> : ILogger<T>
    {
        public List<LogEntry> LogEntries { get; } = new();

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            LogEntries.Add(new LogEntry(logLevel, state?.ToString(), exception));
        }
    }

    public class LogEntry
    {
        public LogLevel LogLevel { get; }
        public string? Message { get; }
        public Exception Exception { get; }

        public LogEntry(LogLevel logLevel, string? message, Exception exception)
        {
            LogLevel = logLevel;
            Message = message;
            Exception = exception;
        }
    }
}
