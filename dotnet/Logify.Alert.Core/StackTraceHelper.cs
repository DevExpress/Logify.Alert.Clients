using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    public class StackTraceHelper : IStackTraceHelper {
        public string GetOuterStackTrace(int skipFrames) {
#if NETSTANDARD
            return String.Empty;
#else
            try {
                StackTrace trace = new StackTrace(skipFrames + 1, true); // remove GetOuterStackTrace call from stack trace
                return trace.ToString();
            }
            catch {
                return String.Empty;
            }
#endif
        }
#if NETSTANDARD
        public string GetOuterNormalizedStackTrace(int skipFrames) {
            return String.Empty;
        }
#else
        public string GetOuterNormalizedStackTrace(int skipFrames) {
            CultureInfo prevCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo prevUICulture = Thread.CurrentThread.CurrentUICulture;
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                StackTrace trace = new StackTrace(skipFrames + 1, true); // remove GetOuterNormalizedStackTrace call from stack trace
                return ExceptionNormalizedStackCollector.NormalizeStackTrace(trace.ToString());
            }
            catch {
                return String.Empty;
            }
            finally {
                Thread.CurrentThread.CurrentCulture = prevCulture;
                Thread.CurrentThread.CurrentUICulture = prevUICulture;
            }
        }
#endif
        }
}