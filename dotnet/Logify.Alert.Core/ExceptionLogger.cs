using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public static void ReportException(Exception ex, IInfoCollector collector) {
            IExceptionReportSender reportSender = ExceptionLoggerFactory.Instance.PlatformReportSender;
            if (reportSender == null)
                return;

            if (collector == null)
                return;

            reportSender = reportSender.Clone();

            ExceptionLogger logger = new ExceptionLogger();
            logger.ReportSender = reportSender;
            logger.PerformReportException(ex, collector);
        }

        void PerformReportException(Exception ex, IInfoCollector collector) {
            if (ex == null || collector == null)
                return;

            try {
                ReportExceptionCore(ex, collector);
            }
            catch {
            }
        }
        void ReportExceptionCore(Exception ex, IInfoCollector collector) {
            if (!ShouldSendExceptionReport())
                return;

            LogifyClientExceptionReport report = CreateExceptionReport(ex, collector);
            SendExceptionReport(report);
        }
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
        void SendExceptionReport(LogifyClientExceptionReport report) {
            if (ReportSender != null)
                ReportSender.SendExceptionReport(report);
        }
    }
}
