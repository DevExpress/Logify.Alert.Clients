using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.WPF {
    public class WPFExceptionReportSender : ServiceExceptionReportSender {
        public override IExceptionReportSender CreateEmptyClone() {
            return new WPFExceptionReportSender();
        }
    }
}
