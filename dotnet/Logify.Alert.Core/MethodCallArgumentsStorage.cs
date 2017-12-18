using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DevExpress.Logify.Core {
    public class MethodCallInfo {
        public object Instance { get; set; }
        public MethodBase Method { get; set; }
        public IList<object> Arguments { get; set; }
    }
}

namespace DevExpress.Logify.Core.Internal {
    public class MethodCallStackArgumentMap : Dictionary<int, MethodCallInfo> {
        public int FirstChanceFrameCount { get; set; }
    }
    public class MethodCallArgumentMap : Dictionary<Exception, MethodCallStackArgumentMap> {
    }
}