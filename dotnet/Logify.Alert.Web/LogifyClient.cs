using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using DevExpress.Logify.Core;
using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;
using DevExpress.Logify.Web;

namespace DevExpress.Logify.Web {
    public class LogifyAlert : LogifyClientBase {
        static volatile LogifyAlert instance;

        public LogifyAlert() {
        }
        protected LogifyAlert(string apiKey) : base(apiKey) {
        }

        public static new LogifyAlert Instance {
            get {
                if (instance != null)
                    return instance;

                lock (typeof(LogifyAlert)) {
                    if (instance != null)
                        return instance;

                    instance = new LogifyAlert();
                    LogifyClientBase.Instance = instance;
                }
                return instance;
            }
        }

        //public bool SendReportInSeparateProcess { get; set; }

        protected internal LogifyAlert(Dictionary<string, string> config) : base(config) {
        }

        protected override IInfoCollectorFactory CreateCollectorFactory() {
            //return new WinFormsExceptionCollectorFactory();
            return new WebDefaultExceptionCollectorFactory(Platform.ASP);
        }
        protected override IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config, IDictionary<string, string> additionalCustomData, AttachmentCollection additionalAttachments) {
            WebExceptionCollector result = new WebExceptionCollector(config, Platform.ASP);
            result.AppName = this.AppName;
            result.AppVersion = this.AppVersion;
            result.UserId = this.UserId;
            result.Collectors.Add(new CustomDataCollector(this.CustomData, additionalCustomData));
            result.Collectors.Add(new AttachmentsCollector(this.Attachments, additionalAttachments));
            return result;
        }
        protected override IExceptionReportSender CreateExceptionReportSender() {
            WebExceptionReportSender defaultSender = new WebExceptionReportSender();
            defaultSender.ConfirmSendReport = ConfirmSendReport;
            if (ConfirmSendReport)
                return defaultSender;

            //IExceptionReportSender winDefaultSender = base.CreateExceptionReportSender();
            CompositeExceptionReportSender sender = new CompositeExceptionReportSender();
            sender.StopWhenFirstSuccess = true;
            //sender.Senders.Add(new ExternalProcessExceptionReportSender());
            sender.Senders.Add(defaultSender);
            sender.Senders.Add(new OfflineDirectoryExceptionReportSender());
            return sender;
        }
        protected override IExceptionReportSender CreateEmptyPlatformExceptionReportSender() {
            return new WebExceptionReportSender();
        }
        protected override ISavedReportSender CreateSavedReportsSender() {
            return new SavedExceptionReportSender();
        }
        protected override BackgroundExceptionReportSender CreateBackgroundExceptionReportSender(IExceptionReportSender reportSender) {
            return new EmptyBackgroundExceptionReportSender(reportSender);
        }

        protected override string GetAssemblyVersionString(Assembly asm) {
            return asm.GetName().Version.ToString();
        }
        protected override IExceptionIgnoreDetection CreateIgnoreDetection() {
            return new StackBasedExceptionIgnoreDetection();
        }
        protected override void Configure() {
            ClientConfigurationLoader.ApplyClientConfiguration(this);
        }

        public override void Run() {
            //do nothing
            //SendOfflineReports();
        }
        public override void Stop() {
            //do nothing
        }
   }
}