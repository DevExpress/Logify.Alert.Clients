﻿using DevExpress.Logify.Core;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Web {
    public class LogifyAlertTraceListener : LogifyAlertTraceListenerBase {
        protected override void InitializeInstance() {
            if (LogifyClientBase.Instance == null)
                LogifyAlert.InitializeInstance();
        }
    }
}