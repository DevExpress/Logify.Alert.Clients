using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Logify.Core;
using DevExpress.Logify.Core.Internal;
using Microsoft.Extensions.Configuration;

namespace DevExpress.Logify.Web {
    public class LogifyAlert : LogifyClientBase {
        static volatile LogifyAlert instance;

        internal LogifyAlert(bool b) {
        }
        protected LogifyAlert(string apiKey) : base(apiKey) {
        }

        public bool CollectBreadcrumbs { get { return base.CollectBreadcrumbsCore; } set { base.CollectBreadcrumbsCore = value; } }
        public string IgnoreFormFields {
            get { return Config.IgnoreConfig.IgnoreFormFields; }
            set { Config.IgnoreConfig.IgnoreFormFields = value; }
        }
        public string IgnoreHeaders {
            get { return Config.IgnoreConfig.IgnoreHeaders; }
            set { Config.IgnoreConfig.IgnoreHeaders = value; }
        }
        public string IgnoreCookies {
            get { return Config.IgnoreConfig.IgnoreCookies; }
            set { Config.IgnoreConfig.IgnoreCookies = value; }
        }
        //public string IgnoreServerVariables {
        //    get { return Config.IgnoreConfig.IgnoreServerVariables; }
        //    set { Config.IgnoreConfig.IgnoreServerVariables = value; }
        //}
        public bool IgnoreRequestBody {
            get { return Config.IgnoreConfig.IgnoreRequestBody; }
            set { Config.IgnoreConfig.IgnoreRequestBody = value; }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool ConfirmSendReport { get { return ConfirmSendReportCore; } set { ConfirmSendReportCore = value; } }

        public static new LogifyAlert Instance {
            get {
                if (instance != null)
                    return instance;

                InitializeInstance();
                return instance;
            }
        }
        internal static void InitializeInstance() {
            lock (typeof(LogifyAlert)) {
                if (instance != null)
                    return;

                instance = new LogifyAlert(true);
                LogifyClientBase.Instance = instance;
            }
        }

        protected internal LogifyAlert(Dictionary<string, string> config) : base(config) {
        }

        protected override RootInfoCollector CreateDefaultCollectorCore() {
            return new NetCoreWebExceptionCollector(Config, Platform.NETCORE_ASP);
        }
        protected override ILogifyAppInfo CreateAppInfo() {
            return new NetCoreWebApplicationCollector();
        }
        protected override IExceptionReportSender CreateExceptionReportSender() {
            IExceptionReportSender defaultSender = CreateConfiguredPlatformExceptionReportSender();
            if (ConfirmSendReport)
                return defaultSender;

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
        protected override LogifyAlertConfiguration LoadConfiguration() {
            return new LogifyAlertConfiguration();
        }
        internal void Configure(IConfigurationSection section) {
            Configure(ClientConfigurationLoader.LoadConfiguration(section));
        }

        public override void Run() {
            //do nothing
            //SendOfflineReports();
        }
        public override void Stop() {
            //do nothing
        }
        protected override ReportConfirmationModel CreateConfirmationModel(LogifyClientExceptionReport report, Func<LogifyClientExceptionReport, bool> sendAction) {
            return null;
        }
        protected override bool RaiseConfirmationDialogShowing(ReportConfirmationModel model) {
            return false;
        }
    }
}