using System;
using System.IO;
using System.Text;
using System.Threading;

namespace DevExpress.Logify.Core {
    public class SavedExceptionReportSender {
        public string DirectoryName { get; set; }
        public IExceptionReportSender Sender { get; set; }

        public void TrySendSavedReports() {
            try {
                if (String.IsNullOrEmpty(DirectoryName))
                    return;
                if (Sender != null && !Sender.CanSendExceptionReport())
                    return;
                if (!Directory.Exists(DirectoryName))
                    return;

                Thread thread = new Thread(TrySendSavedReportsWorker);
                //thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
            catch {
            }
        }

        void TrySendSavedReportsWorker() {
            try {
                TrySendSavedReportsCore();
            }
            catch {
            }
        }

        void TrySendSavedReportsCore() {
            if (String.IsNullOrEmpty(DirectoryName))
                return;
            if (Sender != null && !Sender.CanSendExceptionReport())
                return;
            if (!Directory.Exists(DirectoryName))
                return;

            string[] fileNames = Directory.GetFiles(DirectoryName, TempDirectoryExceptionReportSender.TempFileNamePrefix + "*." + TempDirectoryExceptionReportSender.TempFileNameExtension);
            foreach (string fileName in fileNames)
                TrySendSavedReport(fileName);
        }

        void TrySendSavedReport(string fileName) {
            try {
                FileInfo info = new FileInfo(fileName);
                if (info.Length > 1024 * 1024) { // file size if more than 1Mb
                    File.Delete(fileName);
                    return;
                }

                LogifyClientExceptionReport report = new LogifyClientExceptionReport();
                StringBuilder content = new StringBuilder(File.ReadAllText(fileName));
                report.ReportContent = content;
                if (Sender.SendExceptionReport(report))
                    File.Delete(fileName);
            }
            catch {
            }
        }
    }
}