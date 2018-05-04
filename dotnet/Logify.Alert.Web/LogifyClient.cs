using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.Web;
using DevExpress.Logify.Core;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Web {
    public class LogifyAlert : LogifyClientBase {
        static volatile LogifyAlert instance;

        [Obsolete("Please use the LogifyAlert.Instance property instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LogifyAlert() {
        }
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
        public string IgnoreServerVariables {
            get { return Config.IgnoreConfig.IgnoreServerVariables; }
            set { Config.IgnoreConfig.IgnoreServerVariables = value; }
        }
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

        protected override LogifyCollectorContext GrabCollectorContext(MethodCallArgumentMap callArgumentsMap, IDictionary<string, string> additionalCustomData = null, AttachmentCollection additionalAttachments = null) {
            LogifyCollectorContext context = base.GrabCollectorContext(callArgumentsMap, additionalCustomData, additionalAttachments);

            WebLogifyCollectorContext webContext = new WebLogifyCollectorContext();
            webContext.CopyFrom(context);
            webContext.HttpContext = HttpContext.Current;
            return webContext;
        }
        protected override RootInfoCollector CreateDefaultCollectorCore(LogifyCollectorContext context) {
            return new WebExceptionCollector(context, Platform.ASP);
        }
        protected override ILogifyAppInfo CreateAppInfo(LogifyCollectorContext context) {
            WebLogifyCollectorContext webContext = context as WebLogifyCollectorContext;
            return new WebApplicationCollector(webContext != null ? webContext.HttpContext : null);
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
            WebLogifyConfigSection section = ConfigurationManager.GetSection("logifyAlert") as WebLogifyConfigSection;
            LogifyAlertConfiguration configuration = ClientConfigurationLoader.LoadCommonConfiguration(section);
            LoadWebConfiguration(section, configuration);
            return configuration;
        }
        static void LoadWebConfiguration(WebLogifyConfigSection section, LogifyAlertConfiguration config) {
            if (section == null)
                return;

            if (section.IgnoreFormFields != null)
                config.IgnoreFormFields = section.IgnoreFormFields.Value;
            if (section.IgnoreHeaders != null)
                config.IgnoreHeaders = section.IgnoreHeaders.Value;
            if (section.IgnoreCookies != null)
                config.IgnoreCookies = section.IgnoreCookies.Value;
            if (section.IgnoreServerVariables != null)
                config.IgnoreServerVariables = section.IgnoreServerVariables.Value;
            if (section.IgnoreRequestBody != null)
                config.IgnoreRequestBody = section.IgnoreRequestBody.ValueAsBool;
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

namespace DevExpress.Logify.Core.Internal {
    public class WebLogifyCollectorContext : LogifyCollectorContext {
        public HttpContext HttpContext { get; set; }
    }
}