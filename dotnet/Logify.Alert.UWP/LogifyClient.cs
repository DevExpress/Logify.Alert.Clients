using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Security;
using DevExpress.Logify.Core;
using System.ComponentModel;
using DevExpress.Logify.Core.Internal;
using System.Threading.Tasks;
using WinUI = Windows.UI.Xaml;

namespace DevExpress.Logify.UWP {
    public class LogifyAlert : LogifyClientBase {
        static volatile LogifyAlert instance;

        internal LogifyAlert(bool b) {
        }
        protected LogifyAlert(string apiKey) : base(apiKey) {
        }

        internal bool CollectMiniDump { get { return Config.CollectMiniDump; } set { Config.CollectMiniDump = value; } }
        internal bool CollectBreadcrumbs { get { return CollectBreadcrumbsCore; } set { CollectBreadcrumbsCore = value; } }
        public int BreadcrumbsMaxCount { get { return BreadcrumbsMaxCountCore; } set { BreadcrumbsMaxCountCore = value; } }
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

        protected override RootInfoCollector CreateDefaultCollectorCore(LogifyCollectorContext context) {
            return new UWPExceptionCollector(context);
        }
        protected override ILogifyAppInfo CreateAppInfo(LogifyCollectorContext context) {
            return new UWPApplicationCollector();
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
            return new UWPExceptionReportSender();
        }

        protected override LogifyAlertConfiguration LoadConfiguration() {
            return new LogifyAlertConfiguration();
        }
        public override void Run()
        {
            if (!IsSecondaryInstance) {
                if (WinUI.Application.Current != null) {
                    WinUI.Application.Current.UnhandledException += OnCurrentAppUnhandledException;
                }
                AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            }
        }

        public override void Stop() {
            if (!IsSecondaryInstance) {
                if (WinUI.Application.Current != null) {
                    WinUI.Application.Current.UnhandledException -= OnCurrentAppUnhandledException;
                }
                AppDomain.CurrentDomain.UnhandledException -= OnCurrentDomainUnhandledException;
                TaskScheduler.UnobservedTaskException -= TaskSchedulerOnUnobservedTaskException;
            }
        }
        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        [IgnoreCallTracking]
        private void OnCurrentAppUnhandledException(object sender, WinUI.UnhandledExceptionEventArgs e)
        {
            if (e == null)
                return;
            HandleExceptionCore(e.Exception as Exception);
        }

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        [IgnoreCallTracking]
        void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (e == null)
                return;
            HandleExceptionCore(e.ExceptionObject as Exception);
        }

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        [IgnoreCallTracking]
        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
            if (e == null)
                return;
            HandleExceptionCore(e.Exception);
        }

        private void HandleExceptionCore(Exception ex) {
            var callArgumentsMap = this.MethodArgumentsMap; // this call should be done before any inner calls
            ResetTrackArguments();
            if (ex != null) {
                LogifyCollectorContext context = GrabCollectorContext(callArgumentsMap);
                ReportException(ex, context);
            }
        }
        protected override ReportConfirmationModel CreateConfirmationModel(LogifyClientExceptionReport report, Func<LogifyClientExceptionReport, bool> sendAction) {
            return null;
        }
        protected override bool RaiseConfirmationDialogShowing(ReportConfirmationModel model) {
            return false;
        }
        protected override IInfoCollector CreateNormalizedStackCollector() {
            return new RegexpExceptionNormalizedStackCollector();
        }
        protected override IStackTraceNormalizer CreateStackNormalizer() {
            return new RegexpExceptionNormalizedStackCollector();
        }
    }
}