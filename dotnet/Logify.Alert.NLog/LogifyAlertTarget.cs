using System;
using DevExpress.Logify.Core;
using NLog.Targets;
using NLog;

namespace DevExpress.Logify.Alert.NLog {
    [Target("LogifyAlert")]
    public sealed class LogifyAlertTarget : TargetWithLayout {
        protected override void Write(LogEventInfo logEvent) {
            if (logEvent.Exception != null) {
                if (LogifyClientBase.Instance != null)
                    LogifyClientBase.Instance.Send(logEvent.Exception);
            }
        }
    }
}
