using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    public class MethodCallInfo {
        public object Instance { get; set; }
        public MethodBase Method { get; set; }
        public IList<object> Arguments { get; set; }
    }
    public class MethodCallStackArgumentMap : Dictionary<int, MethodCallInfo> {
        //public StackTrace FirstChanceStackTrace { get; set; }
        //public string FirstChanceNormalizedStackTrace { get; set; }
        public int FirstChanceFrameCount { get; set; }
    }
    public class MethodCallArgumentMap : Dictionary<Exception, MethodCallStackArgumentMap> {
    }
    public static class MethodCallArgumentsStorage {
        [ThreadStatic]
        static MethodCallArgumentMap methodArgumentsMap;

        public static MethodCallArgumentMap MethodArgumentsMap { get { return methodArgumentsMap; } }

        [IgnoreCallTracking]
        public static void Clear() {
            //if (methodArgumentsMap != null) {
            //methodArgumentsMap.Clear();
            if (methodArgumentsMap != null)
                methodArgumentsMap = null;
            //}
        }
        [IgnoreCallTracking]
        static void TryCreate() {
            if (methodArgumentsMap == null)
                methodArgumentsMap = new MethodCallArgumentMap();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [IgnoreCallTracking]
        public static void AddException(Exception ex, MethodCallInfo call, int skipFrames = 1) {
            if (ex == null || call == null || call.Arguments == null || call.Arguments.Count <= 0)
                return;

            TryCreate();
            StackTrace trace = new StackTrace(0, false);
            MethodCallStackArgumentMap map;
            if (!MethodArgumentsMap.TryGetValue(ex, out map)) {
                map = new MethodCallStackArgumentMap();
                map.FirstChanceFrameCount = trace.FrameCount;
                //map.FirstChanceStackTrace = trace;
                //map.FirstChanceNormalizedStackTrace = CreateNormalizedStackTrace(trace);
                MethodArgumentsMap[ex] = map;
            }
            int lineIndex = map.FirstChanceFrameCount - trace.FrameCount;
            map[lineIndex] = call;
            //map[trace.FrameCount - skipFrames - 1] = call;
        }
        /*
        [IgnoreCallTracking]
        static string CreateNormalizedStackTrace(StackTrace trace) {
            CultureInfo prevCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo prevUICulture = Thread.CurrentThread.CurrentUICulture;
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                return ExceptionNormalizedStackCollector.NormalizeStackTrace(trace.ToString());
            }
            finally {
                Thread.CurrentThread.CurrentCulture = prevCulture;
                Thread.CurrentThread.CurrentUICulture = prevUICulture;
            }
        }
        */
    }

}