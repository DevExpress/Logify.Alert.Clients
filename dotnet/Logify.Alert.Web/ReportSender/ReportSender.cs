using DevExpress.Logify.Core;

namespace DevExpress.Logify.Web {
    class WebExceptionReportSender : ServiceExceptionReportSender {
       public override IExceptionReportSender CreateEmptyClone() {
            return new WebExceptionReportSender();
        }
    }
}