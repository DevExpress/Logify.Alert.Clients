using System;

namespace DevExpress.Logify.Core {
    public interface IExceptionReportSender {
        string ServiceUrl { get; set; }
        string ApiKey { get; set; }
        //string LogId { get; set; }
        bool ConfirmSendReport { get; set; }
        string MiniDumpServiceUrl { get; set; }

        bool CanSendExceptionReport();
        //bool SendExceptionReport(string report);
        bool SendExceptionReport(LogifyClientExceptionReport report);

        IExceptionReportSender Clone();
        void CopyFrom(IExceptionReportSender instance);
    }
}