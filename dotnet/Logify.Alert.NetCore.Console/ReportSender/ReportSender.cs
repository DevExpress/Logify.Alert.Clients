using DevExpress.Logify.Core;

namespace DevExpress.Logify.Console {
    class NetCoreConsoleExceptionReportSender : ServiceExceptionReportSender {
       public override IExceptionReportSender CreateEmptyClone() {
            return new NetCoreConsoleExceptionReportSender();
        }
    }
}