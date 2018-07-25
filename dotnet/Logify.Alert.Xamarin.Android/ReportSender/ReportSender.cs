using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    class XamarinExceptionReportSender : ServiceExceptionReportSender {
       public override IExceptionReportSender CreateEmptyClone() {
            return new XamarinExceptionReportSender();
        }
    }
}