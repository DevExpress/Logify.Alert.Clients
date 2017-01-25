using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.Logify.Core {
    public class FileExceptionReportSender : ExceptionReportSenderSkeleton {
        public string FileName { get; set; }
        public bool Append { get; set; }
        public Encoding Encoding { get; set; }

        public override bool CanSendExceptionReport() {
            return !String.IsNullOrEmpty(FileName);
        }

        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            try {
                Encoding encoding = this.Encoding;
                if (encoding == null)
                    encoding = Encoding.UTF8;

                if (Append)
                    File.AppendAllText(FileName, report.ReportString, encoding);
                else
                    File.WriteAllText(FileName, report.ReportString, encoding);
                return true;
            }
            catch {
                return false;
            }
        }
#if NET45
        protected override Task<bool> SendExceptionReportCoreAsync(LogifyClientExceptionReport report) {
            return Task.FromResult(SendExceptionReportCore(report));
        }
#endif

        public override IExceptionReportSender CreateEmptyClone() {
            return new FileExceptionReportSender();
        }
        public override void CopyFrom(IExceptionReportSender instance) {
            base.CopyFrom(instance);
            FileExceptionReportSender other = instance as FileExceptionReportSender;
            if (other == null)
                return;

            this.FileName = other.FileName;
            this.Append = other.Append;
            this.Encoding = other.Encoding;
        }
    }
}