using DevExpress.Logify.Core;
using Microsoft.Extensions.Logging;
using System;

namespace DevExpress.Logify {
    public class LogifyAlertLoggerProvider : ILoggerProvider {
        [CLSCompliant(false)]
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) {
            return new LogifyAlertLogger();
        }
        public void Dispose() {
            //do nothing
        }
    }
    [CLSCompliant(false)]
    public class LogifyAlertLogger : Microsoft.Extensions.Logging.ILogger {
        class EmptyScope : IDisposable {
            public void Dispose() {
                //do nothing
            }
        }
        public IDisposable BeginScope<TState>(TState state) {
            return new EmptyScope();
        }

        public bool IsEnabled(LogLevel logLevel) {
            return LogifyClientBase.Instance != null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            if (exception != null && LogifyClientBase.Instance != null)
                LogifyClientBase.Instance.Send(exception);
        }
    }
    public static class ILoggerFactoryExtensions {
        [CLSCompliant(false)]
        public static ILoggerFactory UseLogifyAlert(this ILoggerFactory factory) {
            factory.AddProvider(new LogifyAlertLoggerProvider());
            return factory;
        }
    }
}