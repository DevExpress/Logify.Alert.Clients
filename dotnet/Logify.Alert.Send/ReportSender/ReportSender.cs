using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Send {
    class InnerExceptionReportSender : ServiceExceptionReportSender {
        public bool SendExceptionReport(string reportContent) {
            LogifyClientExceptionReport report = new LogifyClientExceptionReport();
            report.ReportContent = new StringBuilder(reportContent);
            report.Data = new Dictionary<string, object>();
            return SendExceptionReport(report);
        }
        public override IExceptionReportSender CreateEmptyClone() {
            return new InnerExceptionReportSender();
        }
    }
}
