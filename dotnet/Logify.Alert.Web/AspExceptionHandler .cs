using System;
using System.Web;

namespace DevExpress.Logify.Web {
    public class AspExceptionHandler : HttpModuleBase {
        public override void OnInit(HttpApplication context) {
            try {
                if(LogifyAlert.Instance.CollectBreadcrumbs)
                    context.AcquireRequestState += this.OnAcquireRequestState;
                context.Error += this.OnError;
            }
            catch { }
        }

        protected virtual void OnAcquireRequestState(object sender, EventArgs e) {
            AspBreadcrumbsRecorder.Instance.AddBreadcrumb(sender as HttpApplication);
        }

        protected virtual void OnError(object sender, EventArgs args) {
            try {
                HttpApplication httpApplication = sender as HttpApplication;
                if (httpApplication == null)
                    return;

                HttpServerUtility server = httpApplication.Server;
                if (server == null)
                    return;

                Exception lastError = server.GetLastError();
                if(LogifyAlert.Instance.CollectBreadcrumbs)
                    AspBreadcrumbsRecorder.Instance.UpdateBreadcrumb(sender as HttpApplication, lastError);

                LogifyAlert.Instance.Send(lastError);
            }
            catch { }
        }
    }
}