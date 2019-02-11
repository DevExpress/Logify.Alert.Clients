using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevExpress.Logify.Web;
using Microsoft.AspNetCore.Http;

namespace DevExpress.Logify.Core.Internal {
    public class NetCoreWebBreadcrumbsRecorder : BreadcrumbsRecorderBase {
        static volatile NetCoreWebBreadcrumbsRecorder instance = null;
        public static NetCoreWebBreadcrumbsRecorder Instance {
            get {
                if(instance != null)
                    return instance;

                InitializeInstance();
                return instance;
            }
        }

        private NetCoreWebBreadcrumbsRecorder() { }

        internal static void InitializeInstance() {
            lock(typeof(NetCoreWebBreadcrumbsRecorder)) {
                if(instance != null)
                    return;
                instance = new NetCoreWebBreadcrumbsRecorder();
            }
        }

        [IgnoreCallTracking]
        internal void AddBreadcrumb(HttpContext context) {
            if(context.Request != null && context.Response != null) {
                try {
                    Breadcrumb breadcrumb = new Breadcrumb();
                    breadcrumb.Event = BreadcrumbEvent.Request;
                    breadcrumb.CustomData = new Dictionary<string, string>() {
                        { "method", context.Request.Method },
                        { "url", Utils.GetRequestFullPath(context.Request) },
                        { "status", context.Response.StatusCode.ToString() },
                        { "session", TryGetSessionId(context) }
                    };

                    base.AddBreadcrumb(breadcrumb);
                } catch { }
            }
        }
        [IgnoreCallTracking]
        internal void UpdateBreadcrumb(HttpContext context) {
            HttpRequest request = context.Request;
            if(request == null)
                return;

            try {
                Breadcrumb breadcrumb = LogifyAlert.Instance.Breadcrumbs.Where(b =>
                    b.GetIsAuto() &&
                    b.Event == BreadcrumbEvent.Request &&
                    b.CustomData != null &&
                    b.CustomData["method"] == context.Request.Method &&
                    b.CustomData["url"] == Utils.GetRequestFullPath(context.Request) &&
                    b.CustomData["session"] == TryGetSessionId(context)
                ).First();

                if(breadcrumb != null)
                    breadcrumb.CustomData["status"] = "Failed";
            } catch { }
        }
        protected override string GetCategory() {
            return "request";
        }
        protected override string GetThreadId() {
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }
        const string CookieName = "BreadcrumbsCookie";
        string TryGetSessionId(HttpContext context) {
            string cookieValue = null;
            try {
                string cookie = context.Request.Cookies[CookieName];
                if(!string.IsNullOrEmpty(cookie)) {
                    Guid validGuid = Guid.Empty;
                    if(Guid.TryParse(cookie, out validGuid))
                        cookieValue = cookie;
                }
                if(string.IsNullOrEmpty(cookieValue)) {
                    cookieValue = Guid.NewGuid().ToString();
                    context.Response.Cookies.Append(CookieName, cookieValue, new CookieOptions() { HttpOnly = true });
                }
            } catch { }
            return cookieValue;
        }
    }
}
