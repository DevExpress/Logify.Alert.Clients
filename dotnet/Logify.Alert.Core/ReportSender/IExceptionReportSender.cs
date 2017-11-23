using System;
using System.Net;
using System.Threading.Tasks;

namespace DevExpress.Logify.Core.Internal {
    public interface IExceptionReportSender {
        string ServiceUrl { get; set; }
        string ApiKey { get; set; }
        //string LogId { get; set; }
        bool ConfirmSendReport { get; set; }
        ICredentials ProxyCredentials { get; set; }
#if NETSTANDARD
        IWebProxy Proxy { get; set; }
#endif
        //string MiniDumpServiceUrl { get; set; }

        bool CanSendExceptionReport();
        //bool SendExceptionReport(string report);
        bool SendExceptionReport(LogifyClientExceptionReport report);
#if ALLOW_ASYNC
        Task<bool> SendExceptionReportAsync(LogifyClientExceptionReport report);
#endif

        IExceptionReportSender Clone();
        void CopyFrom(IExceptionReportSender instance);
    }
    public interface IRemoteConfigurationProvider {
        LogifyAlertRemoteConfiguration GetConfiguration(string serviceUrl, string apiKey);
    }
    public interface IExceptionReportSenderWrapper {
        IExceptionReportSender InnerSender { get; }
    }
    public interface IOfflineDirectoryExceptionReportSender {
        string DirectoryName { get; set; }
        int ReportCount { get; set; }
        bool IsEnabled { get; set; }
    }

    public interface ISavedReportSender {
        IExceptionReportSender Sender { get; set; }
        string DirectoryName { get; set; }
        void TrySendOfflineReports();
    }
}