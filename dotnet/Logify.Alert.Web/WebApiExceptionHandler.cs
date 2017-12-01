using DevExpress.Logify.Core.Internal;
using System.Web.Http.Filters;

namespace DevExpress.Logify.Web {
    public class WebApiExceptionHandler : ExceptionFilterAttribute {
        public WebApiExceptionHandler() {
        }

        [IgnoreCallTracking]
        public override void OnException(HttpActionExecutedContext context) {
            LogifyAlert.Instance.Send(context.Exception);
            base.OnException(context);
        }
    }
}