using Microsoft.AspNetCore.Http;
using System;

namespace DevExpress.Logify.Web {
    internal static class LogifyHttpContext {
        [ThreadStatic]
        static HttpContext current;

        internal static HttpContext Current { get { return current; } set { current = value; } }
    }
}