using System;
using System.Collections.Generic;
using DevExpress.Logify.Core;
using log4net.Appender;
using log4net.Core;

namespace DevExpress.Logify.Alert.Log4Net {
    public class LogifyAppender : AppenderSkeleton {
        public static void Init() { }
        protected override void Append(LoggingEvent loggingEvent) {
            if (loggingEvent != null) {
                Exception exceptionObject = null;
                IDictionary<string, string> customData = null;
                if (loggingEvent.ExceptionObject != null)
                    exceptionObject = loggingEvent.ExceptionObject;
                if (loggingEvent.MessageObject != null) {
                    if (exceptionObject == null)
                        exceptionObject = loggingEvent.MessageObject as Exception;
                    else
                        customData = new Dictionary<string, string> {
                            { "LoggingEventMessage", loggingEvent.MessageObject.ToString() }
                        };
                }
                if (exceptionObject != null && LogifyClientBase.Instance != null)
                    LogifyClientBase.Instance.Send(exceptionObject, customData);
            }
        }
    }
}