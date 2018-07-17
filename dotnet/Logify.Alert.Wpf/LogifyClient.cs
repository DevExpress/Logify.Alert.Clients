using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using System.Windows.Threading;
using DevExpress.Logify.Core;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Windows;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.WPF {
    public class LogifyAlert : LogifyClientBase {
        static volatile LogifyAlert instance;

        public event ConfirmationDialogEventHandler ConfirmationDialogShowing;

        [Obsolete("Please use the LogifyAlert.Instance property instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LogifyAlert() {
        }
        internal LogifyAlert(bool b) {
        }
        protected LogifyAlert(string apiKey) : base(apiKey) {
        }
        protected internal LogifyAlert(Dictionary<string, string> config)
            : base(config) {
        }

        public bool CollectMiniDump { get { return Config.CollectMiniDump; } set { Config.CollectMiniDump = value; } }
        public bool CollectBreadcrumbs { get { return CollectBreadcrumbsCore; } set { CollectBreadcrumbsCore = value; } }
        public int BreadcrumbsMaxCount { get { return BreadcrumbsMaxCountCore; } set { BreadcrumbsMaxCountCore = value; } }
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
        protected override void BeginCollectBreadcrumbsCore() {
            WPFBreadcrumbsRecorder.Instance.BeginCollect();
        }
        protected override void EndCollectBreadcrumbsCore() {
            WPFBreadcrumbsRecorder.Instance.EndCollect();
        }

        protected override RootInfoCollector CreateDefaultCollectorCore(LogifyCollectorContext context) {
            return new WPFExceptionCollector(context);
        }
        protected override ILogifyAppInfo CreateAppInfo(LogifyCollectorContext context) {
            return new WpfApplicationCollector();
        }
        protected override IExceptionReportSender CreateExceptionReportSender() {
            IExceptionReportSender defaultSender = CreateConfiguredPlatformExceptionReportSender();
            if (ConfirmSendReport)
                return defaultSender;

            CompositeExceptionReportSender sender = new CompositeExceptionReportSender();
            sender.StopWhenFirstSuccess = true;
            sender.Senders.Add(new ExternalProcessExceptionReportSender());
            sender.Senders.Add(defaultSender);
            sender.Senders.Add(new OfflineDirectoryExceptionReportSender());
            return sender;
        }
        protected override IExceptionReportSender CreateEmptyPlatformExceptionReportSender() {
            return new WPFExceptionReportSender();
        }
        protected override LogifyAlertConfiguration LoadConfiguration() {
            LogifyConfigSection section = ConfigurationManager.GetSection("logifyAlert") as LogifyConfigSection;
            return ClientConfigurationLoader.LoadCommonConfiguration(section);
        }
        public override void Run() {
            if (!IsSecondaryInstance) {
                //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
                Dispatcher.CurrentDispatcher.UnhandledException += OnCurrentDispatcherUnhandledException;
                if (Application.Current != null)
                    Application.Current.DispatcherUnhandledException += OnCurrentDispatcherUnhandledException;
                //AppDomain.CurrentDomain.FirstChanceException
                //SendOfflineReports();
            }

        }
        public override void Stop() {
            if (!IsSecondaryInstance) {
                //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
                AppDomain.CurrentDomain.UnhandledException -= OnCurrentDomainUnhandledException;
                Dispatcher.CurrentDispatcher.UnhandledException -= OnCurrentDispatcherUnhandledException;
                if (Application.Current != null)
                    Application.Current.DispatcherUnhandledException -= OnCurrentDispatcherUnhandledException;
            }
        }

        Exception lastReportedException;

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        [IgnoreCallTracking]
        void OnCurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            var callArgumentsMap = this.MethodArgumentsMap; // this call should be done before any inner calls
            ResetTrackArguments();

            if (e != null && e.Exception != null) {
                if (!Object.ReferenceEquals(e.Exception, lastReportedException)) {
                    lastReportedException = e.Exception;
                    LogifyCollectorContext context = GrabCollectorContext(callArgumentsMap);
                    ReportException(e.Exception, context);
                }
            }
        }

        protected override ReportConfirmationModel CreateConfirmationModel(LogifyClientExceptionReport report, Func<LogifyClientExceptionReport, bool> sendAction) {
            return new ConfirmationDialogModel(report, sendAction);
        }
        protected override bool RaiseConfirmationDialogShowing(ReportConfirmationModel model) {
            ConfirmationDialogEventHandler handler = ConfirmationDialogShowing;
            if (handler != null) {
                ConfirmationDialogModel actualModel = model as ConfirmationDialogModel;
                if(actualModel == null)
                    return false;
                ConfirmationDialogEventArgs args = new ConfirmationDialogEventArgs(actualModel);
                handler(this, args);
                return args.Handled;
            }
            return false;
        }


        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        [IgnoreCallTracking]
        void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (e == null)
                return;

            var callArgumentsMap = this.MethodArgumentsMap; // this call should be done before any inner calls
            ResetTrackArguments();
            
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null) {
                if (!Object.ReferenceEquals(ex, lastReportedException)) {
                    lastReportedException = ex;
                    LogifyCollectorContext context = GrabCollectorContext(callArgumentsMap);
                    ReportException(ex, context);
                }
            }
        }

        Mutex mutex;
        protected override bool DetectIfSecondaryInstance() {
            try {
                int pid = Process.GetCurrentProcess().Id;
                string name = "Global\\logifypid" + pid.ToString();
                if (mutex == null) {
                    mutex = OpenExistingMutex(name);
                    if (mutex != null)
                        return true;
                }

                this.mutex = new Mutex(false, name);
                return false;
            }
            catch {
                return false;
            }
        }
        Mutex OpenExistingMutex(string name) {
            try {
#if ALLOW_ASYNC
                Mutex result;
                if (Mutex.TryOpenExisting(name, out result))
                    return result;
                return null;
#else
                try {
                    //AM: TryOpenExisting exists only in .NET4.5
                    MethodInfo tryGetMutex = typeof(Mutex).GetMethod("TryOpenExisting", new Type[] { typeof(string), typeof(Mutex).MakeByRefType() });
                    if (tryGetMutex != null) {

                        object[] parameters = new object[] { name, null };
                        object mutexExists = tryGetMutex.Invoke(null, parameters);
                        if ((bool)mutexExists) {
                            Mutex result = parameters[1] as Mutex;
                            if (result != null)
                                return result;
                        }
                        else
                            return null;
                    }
                }
                catch {
                }
                return Mutex.OpenExisting(name);
#endif
            }
            catch {
                return null;
            }
        }
    }
}