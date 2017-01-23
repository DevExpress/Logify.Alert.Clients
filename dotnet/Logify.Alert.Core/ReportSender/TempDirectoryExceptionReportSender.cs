using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevExpress.Logify.Core {
    public class OfflineDirectoryExceptionReportSender : ExceptionReportSenderSkeleton, IOfflineDirectoryExceptionReportSender {
        internal const string TempFileNamePrefix = "LogifyR_";
        internal const string TempFileNameExtension = "bin";
        public string DirectoryName { get; set; }
        public int ReportCount { get; set; }
        public bool IsEnabled { get; set; }
        public Encoding Encoding { get; set; }

        public OfflineDirectoryExceptionReportSender() {
            ReportCount = 100;
            DirectoryName = "offline_reports";
        }

        public override bool CanSendExceptionReport() {
            return IsEnabled && !String.IsNullOrEmpty(DirectoryName) && base.CanSendExceptionReport();
        }

        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            try {
                if (!Directory.Exists(DirectoryName))
                    Directory.CreateDirectory(DirectoryName);

                if (!Directory.Exists(DirectoryName))
                    return false;

                EnsureHaveSpace();
                string fileName = CreateTempFileName(DirectoryName);
                if (String.IsNullOrEmpty(fileName))
                    return false;

                Encoding encoding = this.Encoding;
                if (encoding == null)
                    encoding = Encoding.UTF8;

                File.WriteAllText(fileName, report.ReportString, encoding);
                return true;
            }
            catch {
                return false;
            }
        }

        void EnsureHaveSpace() {
            try {
                int reportCount = Math.Max(0, ReportCount - 1);
                string[] fileNames = Directory.GetFiles(DirectoryName, TempFileNamePrefix + "*." + TempFileNameExtension);
                if (fileNames == null || fileNames.Length <= reportCount)
                    return;

                List<FileInfo> files = new List<FileInfo>();
                foreach (string fileName in fileNames)
                    files.Add(new FileInfo(fileName));

                files.Sort(new FileInfoDateTimeComparer());

                int limit = files.Count - reportCount;
                for (int i = 0; i < limit; i++) {
                    try {
                        files[i].Delete();
                    }
                    catch {
                    }
                }
            }
            catch {
            }
        }

        class FileInfoDateTimeComparer : IComparer<FileInfo> {
            public int Compare(FileInfo x, FileInfo y) {
                return Comparer<DateTime>.Default.Compare(x.LastWriteTime, y.LastWriteTime);
            }
        }

        string CreateTempFileName(string directoryName) {
            DateTime now = DateTime.Now;
            string fileNameTemplate = String.Format(TempFileNamePrefix + "{0}_{1}_{2}_{3}_{4}_{5}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            for (int i = 0; i < 100; i++) {
                string fileName = fileNameTemplate;
                if (i != 0)
                    fileName += "_" + i.ToString();
                fileName += "." + TempFileNameExtension;
                string fullPath = Path.Combine(directoryName, fileName);
                if (!File.Exists(fullPath))
                    return fullPath;
            }
            return String.Empty;
        }

        public override IExceptionReportSender CreateEmptyClone() {
            return new OfflineDirectoryExceptionReportSender();
        }
        public override void CopyFrom(IExceptionReportSender instance) {
            base.CopyFrom(instance);
            OfflineDirectoryExceptionReportSender other = instance as OfflineDirectoryExceptionReportSender;
            if (other == null)
                return;

            this.DirectoryName = other.DirectoryName;
            this.Encoding = other.Encoding;
            this.ReportCount = other.ReportCount;
            this.IsEnabled = other.IsEnabled;
        }
    }
}