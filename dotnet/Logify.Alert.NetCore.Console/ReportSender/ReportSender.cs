using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    class NetCoreConsoleExceptionReportSender : ServiceExceptionReportSender {
       public override IExceptionReportSender CreateEmptyClone() {
            return new NetCoreConsoleExceptionReportSender();
        }
    }
}