using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Configuration;
using DevExpress.Logify.Serilog;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Serilog {
    public class LogifyAlertSink : ILogEventSink {
        public void Emit(LogEvent logEvent) {
            try {
                if (logEvent != null && logEvent.Exception != null) {
                    if (LogifyClientBase.Instance != null)
                        LogifyClientBase.Instance.Send(logEvent.Exception);
                }
            }
            catch {
            }
        }
    }
}

namespace Serilog {
    public static class LogifyAlertSinkExtensions {
        public static LoggerConfiguration LogifyAlert(this LoggerSinkConfiguration sinkConfiguration, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose, LoggingLevelSwitch levelSwitch = null) {
            if (sinkConfiguration == null)
                throw new ArgumentNullException("sinkConfiguration");
            return sinkConfiguration.Sink(new LogifyAlertSink(), restrictedToMinimumLevel, levelSwitch);
        }
    }
}
