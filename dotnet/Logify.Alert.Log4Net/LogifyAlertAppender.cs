using System;
using DevExpress.Logify.Core;
using log4net.Appender;
using log4net.Core;

namespace DevExpress.Logify.Alert.Log4Net {
    public class LogifyAppender : AppenderSkeleton {
        public static void Init() {
        }

        protected override void Append(LoggingEvent loggingEvent) {
            if (loggingEvent != null && loggingEvent.ExceptionObject != null) {
                if (LogifyClientBase.Instance != null)
                    LogifyClientBase.Instance.Send(loggingEvent.ExceptionObject);
            }
        }
    }
}