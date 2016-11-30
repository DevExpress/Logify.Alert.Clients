using System;
using System.Web;

namespace DevExpress.Logify.Web {
    public class HttpModuleBase : IHttpModule {
        void IHttpModule.Init(HttpApplication context) {
            if (context == null)
                return;
            this.OnInit(context);
        }
        void IHttpModule.Dispose() {
            this.OnDispose();
        }
        public virtual void OnDispose() {
        }

        public virtual void OnInit(HttpApplication context) {
        }
    }
}
