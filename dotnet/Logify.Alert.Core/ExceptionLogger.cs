using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.Logify.Core {
    public class LogifyClientExceptionReport {
        string report;
        StringBuilder reportContent;
        public StringBuilder ReportContent {
            get { return reportContent; }
            set {
                reportContent = value;
                report = null;
            }
        }
        public string ReportString {
            get {
                if (report == null) {
                    if (ReportContent != null)
                        report = ReportContent.ToString();
                }
                return report;
            }
        }
        public Dictionary<string, object> Data { get; set; }
        public void ResetReportString() {
            report = null;
        }

        public LogifyClientExceptionReport Clone() {
            LogifyClientExceptionReport clone = new LogifyClientExceptionReport();
            clone.CopyFrom(this);
            return clone;
        }

        void CopyFrom(LogifyClientExceptionReport value) {
            if (value.ReportContent != null)
                this.reportContent = new StringBuilder(value.ReportContent.ToString());
            this.report = value.report;

            if (value.Data != null) {
                this.Data = new Dictionary<string, object>();
                foreach (string key in value.Data.Keys)
                    this.Data[key] = value.Data[key];
            }
        }
    }
}
namespace DevExpress.Logify.Core.Internal {
    public class ExceptionLogger {
        public IExceptionReportSender ReportSender { get; set; }

        public static void ReportException(Exception ex) {
            IInfoCollectorFactory factory = ExceptionLoggerFactory.Instance.PlatformCollectorFactory;
            if (factory == null)
                return;

            IInfoCollector collector = factory.CreateDefaultCollector(new DefaultClientConfiguration());
            if (collector == null)
                return;

            ReportException(ex, collector);
        }
        public static bool ReportException(Exception ex, IInfoCollector collector) {
            IExceptionReportSender reportSender = ExceptionLoggerFactory.Instance.PlatformReportSender;
            if (reportSender == null)
                return false;

            if (collector == null)
                return false;

            reportSender = reportSender.Clone();

            ExceptionLogger logger = new ExceptionLogger();
            logger.ReportSender = reportSender;
            return logger.PerformReportException(ex, collector);
        }
#if ALLOW_ASYNC
        public static async Task<bool> ReportExceptionAsync(Exception ex, IInfoCollector collector) {
            IExceptionReportSender reportSender = ExceptionLoggerFactory.Instance.PlatformReportSender;
            if (reportSender == null)
                return false;

            if (collector == null)
                return false;

            reportSender = reportSender.Clone();

            ExceptionLogger logger = new ExceptionLogger();
            logger.ReportSender = reportSender;
            return await logger.PerformReportExceptionAsync(ex, collector);
        }
#endif
        bool PerformReportException(Exception ex, IInfoCollector collector) {
            if (ex == null || collector == null)
                return false;

            try {
                return ReportExceptionCore(ex, collector);
            }
            catch {
                return false;
            }
        }
#if ALLOW_ASYNC
        async Task<bool> PerformReportExceptionAsync(Exception ex, IInfoCollector collector) {
            if (ex == null || collector == null)
                return false;

            try {
                return await ReportExceptionCoreAsync(ex, collector);
            }
            catch {
                return false;
            }
        }
#endif
        bool ReportExceptionCore(Exception ex, IInfoCollector collector) {
            if (!ShouldSendExceptionReport())
                return false;

            LogifyClientExceptionReport report = CreateExceptionReport(ex, collector);
            return SendExceptionReport(report);
        }
#if ALLOW_ASYNC
        async Task<bool> ReportExceptionCoreAsync(Exception ex, IInfoCollector collector) {
            if (!ShouldSendExceptionReport())
                return false;

            LogifyClientExceptionReport report = CreateExceptionReport(ex, collector);
            return await SendExceptionReportAsync(report);
        }
#endif
        LogifyClientExceptionReport CreateExceptionReport(Exception ex, IInfoCollector collector) {
            StringBuilder content = new StringBuilder();
            StringWriter writer = new StringWriter(content);
            TextWriterLogger logger = new TextWriterLogger(writer);

            logger.BeginWriteObject(String.Empty);
            try {
                collector.Process(ex, logger);
            }
            finally {
                logger.EndWriteObject(String.Empty);
            }

            LogifyClientExceptionReport report = new LogifyClientExceptionReport();
            report.ReportContent = content;
            report.Data = logger.Data;
            return report;
        }
        bool ShouldSendExceptionReport() {
            return ReportSender != null && ReportSender.CanSendExceptionReport();
        }
        bool SendExceptionReport(LogifyClientExceptionReport report) {
            if (ReportSender != null)
                return ReportSender.SendExceptionReport(report);
            else
                return true;
        }
#if ALLOW_ASYNC
        async Task<bool> SendExceptionReportAsync(LogifyClientExceptionReport report) {
            if (ReportSender != null)
                return await ReportSender.SendExceptionReportAsync(report);
            else
                return true;
        }
#endif
    }
}
