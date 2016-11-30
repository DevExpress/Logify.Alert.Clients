using System;
using System.Web;
using System.Web.Configuration;

namespace DevExpress.Logify.Web {
    public class AspExceptionHandler : HttpModuleBase {
        public override void OnInit(HttpApplication context) {
            try {
                context.Error += this.OnError;
            }
            catch {
            }
        }

        protected virtual void OnError(object sender, EventArgs args) {
            try {
                HttpApplication httpApplication = sender as HttpApplication;
                if (httpApplication == null)
                    return;

                HttpServerUtility server = httpApplication.Server;
                if (server == null)
                    return;

                LogifyAlert.Instance.Send(server.GetLastError());
            }
            catch {
            }
        }
    }
}