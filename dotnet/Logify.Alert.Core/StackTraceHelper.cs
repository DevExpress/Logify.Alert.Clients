using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    public class StackTraceHelper : IStackTraceHelper {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetOuterStackTrace(int skipFrames) {
            try {
                StackTrace trace = new StackTrace(skipFrames + 1, true); // remove GetOuterStackTrace call from stack trace
                return trace.ToString();
            }
            catch {
                return String.Empty;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
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
    }
}