using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    class WebExceptionReportSender : ServiceExceptionReportSender {
       public override IExceptionReportSender CreateEmptyClone() {
            return new WebExceptionReportSender();
        }
    }
}