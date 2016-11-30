using System.Web.Http.Filters;

namespace DevExpress.Logify.Web {
    public class WebApiExceptionHandler : ExceptionFilterAttribute {
        public WebApiExceptionHandler() {
        }

        public override void OnException(HttpActionExecutedContext context) {
            LogifyAlert.Instance.Send(context.Exception);
            base.OnException(context);
        }
    }
}