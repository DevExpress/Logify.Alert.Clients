using System.Collections.Generic;
using System.Threading;
using DevExpress.Logify.Core;
using Microsoft.AspNetCore.Http;

namespace DevExpress.Logify.Web {
    public class NetCoreWebBreadcrumbsRecorder : BreadcrumbsRecorderBase {
        static volatile NetCoreWebBreadcrumbsRecorder instance = null;
        public static NetCoreWebBreadcrumbsRecorder Instance {
            get {
                if(instance != null)
                    return instance;

                InitializeInstance(null);
                return instance;
            }
        }

        internal BreadcrumbCollection Breadcrumbs {
            get {
                return this._storage.Breadcrumbs;
            }
        }
        IBreadcrumbsStorage _storage;
        private NetCoreWebBreadcrumbsRecorder() { }
        public NetCoreWebBreadcrumbsRecorder(IBreadcrumbsStorage storage) {
            this._storage = storage;
        }

        internal static void InitializeInstance(IBreadcrumbsStorage storage) {
            lock(typeof(NetCoreWebBreadcrumbsRecorder)) {
                if(instance != null)
                    return;
                if(storage != null)
                    instance = new NetCoreWebBreadcrumbsRecorder(storage);
                else
                    instance = new NetCoreWebBreadcrumbsRecorder(new InMemoryBreadcrumbsStorage());
            }
        }

        internal new void AddBreadcrumb(Breadcrumb item) {
            this._storage.Breadcrumbs.Add(item);
        }
        internal void AddBreadcrumb(HttpContext context) {
            if(context.Request != null && context.Request.Path != null) {
                Breadcrumb breadcrumb = new Breadcrumb();
                base.PopulateCommonBreadcrumbInfo(breadcrumb);
                breadcrumb.Category = "request";
                breadcrumb.Event = BreadcrumbEvent.None;
                breadcrumb.MethodName = context.Request.Method;
                breadcrumb.CustomData = new Dictionary<string, string>() {
                    { "url", context.Request.Path.ToString() },
                    { "status", context.Response.StatusCode.ToString() },
                    { "session", TryGetSessionId(context) }
                };

                this.AddBreadcrumb(breadcrumb);
            }
        }
        protected override string GetThreadId() {
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }
        string TryGetSessionId(HttpContext context) {
            string result = null;
            try {
                if(context.Session != null)
                    result = context.Session.Id;
            } catch { }
            return result;
        }
    }
}
