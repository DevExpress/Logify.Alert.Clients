using System;
using System.Net;
using System.Threading.Tasks;

namespace DevExpress.Logify.Core.Internal {
    public abstract class ExceptionReportSenderSkeleton : IExceptionReportSender {
        public virtual string ServiceUrl { get; set; }
        public virtual string ApiKey { get; set; }
        public virtual bool ConfirmSendReport { get; set; }
        //public virtual string LogId { get; set; }
        public virtual int RetryCount { get; set; }
        public virtual int ReportTimeoutMilliseconds { get; set; }

        public virtual ICredentials ProxyCredentials { get; set; }
        public virtual IWebProxy Proxy { get; set; }
        //public virtual string MiniDumpServiceUrl { get; set; }

        protected ExceptionReportSenderSkeleton() {
            this.RetryCount = 3;
            this.ReportTimeoutMilliseconds = 5000;
        }

        public virtual bool CanSendExceptionReport() {
            return !String.IsNullOrEmpty(ServiceUrl) &&
                !String.IsNullOrEmpty(ApiKey)/* &&
                !String.IsNullOrEmpty(LogId)*/;
        }

        public virtual bool SendExceptionReport(LogifyClientExceptionReport report) {
            //TODO: maybe put report to disk, for further sending in case of unrecoverable error during send (no net or so on)
            for (int i = 0; i < RetryCount; i++) {
                try {
                    if (SendExceptionReportCore(report))
                        return true;
                } catch {
                }
            }
            return false;
        }
#if ALLOW_ASYNC
        public virtual async Task<bool> SendExceptionReportAsync(LogifyClientExceptionReport report) {
            //TODO: maybe put report to disk, for further sending in case of unrecoverable error during send (no net or so on)
            for (int i = 0; i < RetryCount; i++) {
                try {
                    if (await SendExceptionReportCoreAsync(report))
                        return true;
                }
                catch {
                }
            }
            return false;
        }
        protected abstract Task<bool> SendExceptionReportCoreAsync(LogifyClientExceptionReport report);
#endif

        protected abstract bool SendExceptionReportCore(LogifyClientExceptionReport report);
        public abstract IExceptionReportSender CreateEmptyClone();
        public virtual IExceptionReportSender Clone() {
            IExceptionReportSender clone = CreateEmptyClone();
            clone.CopyFrom(this);
            return clone;
        }
        public virtual void CopyFrom(IExceptionReportSender instance) {
            this.ServiceUrl = instance.ServiceUrl;
            this.ApiKey = instance.ApiKey;
            this.ConfirmSendReport = instance.ConfirmSendReport;
            this.Proxy = instance.Proxy;
            this.ProxyCredentials = instance.ProxyCredentials;
            //this.MiniDumpServiceUrl = instance.MiniDumpServiceUrl;
            //this.LogId = instance.LogId;
        }
    }
}