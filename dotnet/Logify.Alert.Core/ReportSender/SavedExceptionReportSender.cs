using System;
using System.IO;
using System.Text;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    public class SavedExceptionReportSender : ISavedReportSender {
        public string DirectoryName { get; set; }
        public IExceptionReportSender Sender { get; set; }

        public void TrySendOfflineReports() {
            try {
                if (String.IsNullOrEmpty(DirectoryName))
                    return;
                if (Sender != null && !Sender.CanSendExceptionReport())
                    return;
                if (!Directory.Exists(DirectoryName))
                    return;

                string[] files = GetSavedFiles();
                if (files == null || files.Length <= 0)
                    return;
                BackgroundSendModelAccessor.FixWebRequestDeadlock();
                Thread thread = new Thread(TrySendSavedReportsWorker);
                try {
                    thread.Priority = ThreadPriority.Lowest;
                }
                catch {
                }
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

            string[] fileNames = GetSavedFiles();
            foreach (string fileName in fileNames)
                TrySendSavedReport(fileName);
        }

        string[] GetSavedFiles() {
            return Directory.GetFiles(DirectoryName, OfflineDirectoryExceptionReportSender.TempFileNamePrefix + "*." + OfflineDirectoryExceptionReportSender.TempFileNameExtension);
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