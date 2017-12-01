using System;
using System.Web;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Web {
    public class AspExceptionHandler : HttpModuleBase {
        public override void OnInit(HttpApplication context) {
            try {
                if(LogifyAlert.Instance.CollectBreadcrumbs)
                    context.BeginRequest += this.OnBeginRequest;
                context.Error += this.OnError;
            }
            catch { }
        }

        protected virtual void OnBeginRequest(object sender, EventArgs e) {
            AspBreadcrumbsRecorder.Instance.AddBreadcrumb(sender as HttpApplication);
        }

        [IgnoreCallTracking]
        protected virtual void OnError(object sender, EventArgs args) {
            try {
                HttpApplication httpApplication = sender as HttpApplication;
                if (httpApplication == null)
                    return;

                HttpServerUtility server = httpApplication.Server;
                if (server == null)
                    return;

                if(LogifyAlert.Instance.CollectBreadcrumbs)
                    AspBreadcrumbsRecorder.Instance.UpdateBreadcrumb(sender as HttpApplication);

                Exception lastError = server.GetLastError();
                LogifyAlert.Instance.Send(lastError);
            }
            catch { }
        }
    }
}