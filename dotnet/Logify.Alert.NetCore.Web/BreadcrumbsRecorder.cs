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

        internal void AddBreadcrumb(HttpContext context) {
            if(context.Request != null && context.Request.Path != null && context.Response != null) {
                Breadcrumb breadcrumb = new Breadcrumb();
                breadcrumb.Event = BreadcrumbEvent.Request;
                breadcrumb.CustomData = new Dictionary<string, string>() {
                    { "method", context.Request.Method },
                    { "url", context.Request.Path.Value },
                    { "status", context.Response.StatusCode.ToString() },
                    { "session", TryGetSessionId(context) }
                };

                base.AddBreadcrumb(breadcrumb);
            }
        }
        internal void UpdateBreadcrumb(HttpContext context) {
            HttpRequest request = context.Request;
            if(request == null)
                return;

            Breadcrumb breadcrumb = LogifyAlert.Instance.Breadcrumbs.Where(b =>
                b.GetIsAuto() &&
                b.Event == BreadcrumbEvent.Request &&
                b.CustomData != null &&
                b.CustomData["method"] == context.Request.Method &&
                b.CustomData["url"] == context.Request.Path.Value &&
                b.CustomData["session"] == TryGetSessionId(context)
            ).First();

            if(breadcrumb != null)
                breadcrumb.CustomData["status"] = "Failed";
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
                Guid validGuid = Guid.Empty;
                if(Guid.TryParse(context.Request.Cookies[CookieName], out validGuid))
                    cookieValue = context.Request.Cookies[CookieName];
                if(string.IsNullOrEmpty(cookieValue)) {
                    cookieValue = Guid.NewGuid().ToString();
                    context.Response.Cookies.Append(CookieName, cookieValue, new CookieOptions() { HttpOnly = true });
                }
            } catch { }
            return cookieValue;
        }
    }
}
