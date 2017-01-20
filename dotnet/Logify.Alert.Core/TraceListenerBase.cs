using System;
using System.Diagnostics;

namespace DevExpress.Logify.Core {
    public abstract class LogifyAlertTraceListenerBase : TraceListener {
        public override void Write(string message) {
            //do nothing
        }

        public override void WriteLine(string message) {
            //do nothing
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data) {
            try {
                if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
                    return;

                TrySendException(eventType, data);
            }
            catch {
            }
        }
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data) {
            try {
                if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
                    return;

                TrySendException(eventType, data);
            }
            catch {
            }
        }
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args) {
            try {
                if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
                    return;

                TrySendException(eventType, args);
            }
            catch {
            }
        }

        void TrySendException(TraceEventType eventType, object data) {
            if (!IsError(eventType))
                return;

            Exception exception = GetException(data);
            if (exception == null)
                return;

            if (LogifyClientBase.Instance != null)
                LogifyClientBase.Instance.Send(exception);
        }
        void TrySendException(TraceEventType eventType, object[] data) {
            if (!IsError(eventType))
                return;

            Exception exception = GetException(data);
            if (exception == null)
                return;

            if (LogifyClientBase.Instance != null)
                LogifyClientBase.Instance.Send(exception);
        }

        bool IsError(TraceEventType eventType) {
            return eventType == TraceEventType.Critical || eventType == TraceEventType.Error;
        }

        Exception GetException(object data) {
            return data as Exception;
        }
        Exception GetException(object[] data) {
            if (data == null || data.Length <= 0)
                return null;

            try {
                foreach (object instance in data) {
                    Exception ex = instance as Exception;
                    if (ex != null)
                        return ex;
                }
                return null;
            }
            catch {
                return null;
            }
        }
    }
}