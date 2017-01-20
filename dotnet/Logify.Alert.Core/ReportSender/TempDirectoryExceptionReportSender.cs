using System;
using System.IO;
using System.Text;

namespace DevExpress.Logify.Core {
    public class TempDirectoryExceptionReportSender : ExceptionReportSenderSkeleton {
        public const string TempFileNamePrefix = "LogifyR_";
        public const string TempFileNameExtension = "bin";
        public string DirectoryName { get; set; }
        public Encoding Encoding { get; set; }

        public override bool CanSendExceptionReport() {
            return !String.IsNullOrEmpty(DirectoryName);
        }

        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            try {
                if (!Directory.Exists(DirectoryName))
                    Directory.CreateDirectory(DirectoryName);

                if (!Directory.Exists(DirectoryName))
                    return false;

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
            return new TempDirectoryExceptionReportSender();
        }
        public override void CopyFrom(IExceptionReportSender instance) {
            base.CopyFrom(instance);
            TempDirectoryExceptionReportSender other = instance as TempDirectoryExceptionReportSender;
            if (other == null)
                return;

            this.DirectoryName = other.DirectoryName;
            this.Encoding = other.Encoding;
        }
    }
}