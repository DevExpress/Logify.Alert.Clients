using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.Logify.Core {
    public class ExternalProcessExceptionReportSender : ExceptionReportSenderSkeleton {
        public override int RetryCount { get { return 1; } set {} }
        
        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            try {
                string exePath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "LogifySend.exe");
                if (!File.Exists(exePath))
                    return false;

                string reportPath = SaveReportToTemp(report.ReportString);
                if (String.IsNullOrEmpty(reportPath))
                    return false;
                string configPath = SaveConfigToTemp(reportPath, report.Data);
                if (String.IsNullOrEmpty(configPath))
                    return false;
                Process process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = configPath;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                if (!process.Start())
                    return false;
                if (process.WaitForExit(500) && process.HasExited)
                    return process.ExitCode == 0;
                return true;
            }
            catch {
                return false;
            }
        }
#if ALLOW_ASYNC
        protected override Task<bool> SendExceptionReportCoreAsync(LogifyClientExceptionReport report) {
            return Task.FromResult(SendExceptionReportCore(report));
        }
#endif

        string SaveConfigToTemp(string reportPath, Dictionary<string, object> reportData) {
            StringBuilder content = new StringBuilder();
            content.AppendLine("ServiceUrl=" + this.ServiceUrl);
            content.AppendLine("ApiKey=" + this.ApiKey);
            content.AppendLine("ReportFileName=" + reportPath);
            content.AppendLine("DeleteReportFile=1");
            content.AppendLine("DeleteConfigFile=1");
            content.AppendLine("Update=1");

            object dumpGuidObject;
            reportData.TryGetValue(MiniDumpCollector.DumpGuidKey, out dumpGuidObject);
            object dumpFileNameObject;
            reportData.TryGetValue(MiniDumpCollector.DumpFileNameKey, out dumpFileNameObject);
            string dumpGuid = dumpGuidObject as string;
            string dumpFileName = dumpFileNameObject as string;
            if (!String.IsNullOrEmpty(dumpGuid) && !String.IsNullOrEmpty(dumpFileName)) {
                content.AppendLine("MiniDumpGuid=" + dumpGuid);
                content.AppendLine("MiniDumpFileName=" + dumpFileName);
                content.AppendLine("MiniDumpServiceUrl=" + this.MiniDumpServiceUrl);
            }

            string fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, content.ToString(), Encoding.UTF8);
            return fileName;
        }

        string SaveReportToTemp(string report) {
            string fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, report, Encoding.UTF8);
            return fileName;
        }

        public override IExceptionReportSender CreateEmptyClone() {
            return new ExternalProcessExceptionReportSender();
        }
    }

}