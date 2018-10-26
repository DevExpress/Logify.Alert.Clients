using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    class UWPExceptionReportSender : ServiceExceptionReportSender {
       public override IExceptionReportSender CreateEmptyClone() {
            return new UWPExceptionReportSender();
        }
    }
}