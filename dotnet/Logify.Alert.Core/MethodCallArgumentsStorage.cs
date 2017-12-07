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
        public int FirstChanceFrameCount { get; set; }
    }
    public class MethodCallArgumentMap : Dictionary<Exception, MethodCallStackArgumentMap> {
    }
    public static class MethodCallTracker {
        [ThreadStatic]
        static MethodCallArgumentMap methodArgumentsMap;

        public static MethodCallArgumentMap MethodArgumentsMap { get { return methodArgumentsMap; } }

        [IgnoreCallTracking]
        public static void Reset() {
             methodArgumentsMap = null;
        }
        [IgnoreCallTracking]
        static void TryCreate() {
            if (methodArgumentsMap == null)
                methodArgumentsMap = new MethodCallArgumentMap();
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        [IgnoreCallTracking]
        public static void TrackException(Exception ex, object instance, params object[] args) {
            try {
                if (ex == null)
                    return;

                MethodCallInfo call = new MethodCallInfo() {
                    Instance = instance,
                    Arguments = args
                };

                StackTrace trace = new StackTrace(0, false);
                int frameCount = trace.FrameCount;
                if (frameCount > 0)
                    call.Method = trace.GetFrame(1).GetMethod();

                TrackException(ex, frameCount, call);
            }
            catch {
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [IgnoreCallTracking]
        public static void TrackException(Exception ex, MethodCallInfo call, int skipFrames = 1) {
            try {
                if (ex == null || call == null || call.Arguments == null || call.Arguments.Count <= 0)
                    return;

                StackTrace trace = new StackTrace(0, false);
                int frameCount = trace.FrameCount;
                if (call.Method == null)
                    call.Method = trace.GetFrame(skipFrames).GetMethod();

                TrackException(ex, frameCount, call);
            }
            catch {
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        [IgnoreCallTracking]
        public static void TrackException(Exception ex, int frameCount, MethodCallInfo call) {
            try {
                if (ex == null || call == null || call.Arguments == null || call.Arguments.Count <= 0)
                    return;
                if (call.Method == null)
                    return;

                TryCreate();

                MethodCallStackArgumentMap map;
                if (!MethodArgumentsMap.TryGetValue(ex, out map)) {
                    map = new MethodCallStackArgumentMap();
                    map.FirstChanceFrameCount = frameCount;
                    MethodArgumentsMap[ex] = map;
                }
                int lineIndex = map.FirstChanceFrameCount - frameCount;
                map[lineIndex] = call;
            }
            catch {
            }
        }
    }
}