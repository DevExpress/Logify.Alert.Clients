using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Web {
    public class AspBreadcrumbsRecorder : BreadcrumbsRecorderBase {
        static volatile AspBreadcrumbsRecorder instance = null;
        public static AspBreadcrumbsRecorder Instance {
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
        private AspBreadcrumbsRecorder() { }
        public AspBreadcrumbsRecorder(IBreadcrumbsStorage storage) {
            this._storage = storage;
        }

        internal static void InitializeInstance(IBreadcrumbsStorage storage) {
            lock(typeof(AspBreadcrumbsRecorder)) {
                if(storage != null)
                    instance = new AspBreadcrumbsRecorder(storage);
                if(instance != null)
                    return;
                else if(HttpContext.Current != null && HttpContext.Current.Session != null)
                    instance = new AspBreadcrumbsRecorder(new HttpBreadcrumbsStorage());
                else
                    instance = new AspBreadcrumbsRecorder(new InMemoryBreadcrumbsStorage());
            }
        }
        internal void SetBreadcrumbsStorage(IBreadcrumbsStorage storage) {
            InitializeInstance(storage);
        }

        internal new void AddBreadcrumb(Breadcrumb item) {
            //base.AddBreadcrumb(item);
            this._storage.Breadcrumbs.Add(item);
        }
        internal void AddBreadcrumb(HttpApplication httpApplication) {
            if(httpApplication == null)
                return;

            HttpRequest request = httpApplication.Context.Request;
            if(request == null)
                return;

            HttpResponse response = httpApplication.Context.Response;
            if(response == null)
                return;

            HttpSessionState session = httpApplication.Context.Session;

            Breadcrumb breadcrumb = new Breadcrumb();
            base.PopulateCommonBreadcrumbInfo(breadcrumb);
            breadcrumb.Category = "request";
            breadcrumb.Event = BreadcrumbEvent.None;
            breadcrumb.MethodName = request.HttpMethod;
            breadcrumb.CustomData = new Dictionary<string, string>() {
                { "url", request.Url.ToString() },
                { "status", response.StatusDescription },
                { "session", TryGetSessionId(request, response, session) },
                { "a", "1" }
            };

            this.AddBreadcrumb(breadcrumb);
        }
        internal void UpdateBreadcrumb() {
            Breadcrumb breadcrumb = Breadcrumbs.Where(b => b.CustomData != null && b.CustomData["a"] == "1").First();
            if(breadcrumb != null)
                breadcrumb.CustomData["status"] = "Failed";
        }
        protected override string GetThreadId() {
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }
        string TryGetSessionId(HttpRequest request, HttpResponse response, HttpSessionState session) {
            string result = TryGetCookie(request, response);
            if(string.IsNullOrEmpty(result) && session != null)
                result = session.SessionID;
            return result;
        }
        const string CookieName = "Logify.Web.Cookie";
        string TryGetCookie(HttpRequest request, HttpResponse response) {
            string cookieValue = null;
            //HttpCookie standardCookie = request.Cookies["ASP.NET_SessionId"];
            //if(standardCookie != null)
            //    cookieValue = standardCookie.Value;
            if(string.IsNullOrEmpty(cookieValue)) {
                HttpCookie logifyCookie = request.Cookies[CookieName];
                if(logifyCookie != null) {
                    cookieValue = logifyCookie.Value;
                } else {
                    cookieValue = Guid.NewGuid().ToString();
                    response.Cookies.Add(new HttpCookie(CookieName, cookieValue));
                }
            }
            return cookieValue;
        }
    }
}
