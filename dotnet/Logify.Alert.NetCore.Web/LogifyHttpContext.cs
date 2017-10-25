using Microsoft.AspNetCore.Http;
using System;

namespace DevExpress.Logify.Core.Internal {
    internal static class LogifyHttpContext {
        [ThreadStatic]
        static HttpContext current;

        internal static HttpContext Current { get { return current; } set { current = value; } }
    }
}