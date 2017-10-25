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
        internal bool CollectBreadcrumbs { get { return CollectBreadcrumbsCore; } set { CollectBreadcrumbsCore = value; } }
        internal int BreadcrumbsMaxCount { get { return BreadcrumbsMaxCountCore; } set { BreadcrumbsMaxCountCore = value; } }

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

        protected override IInfoCollectorFactory CreateCollectorFactory() {
            return new WPFExceptionCollectorFactory();
        }
        protected override IInfoCollector CreateDefaultCollector(IDictionary<string, string> additionalCustomData, AttachmentCollection additionalAttachments) {
            WPFExceptionCollector result = new WPFExceptionCollector(Config);
            result.AppName = this.AppName;
            result.AppVersion = this.AppVersion;
            result.UserId = this.UserId;
            result.Collectors.Add(new CustomDataCollector(this.CustomData, additionalCustomData));
            result.Collectors.Add(new BreadcrumbsCollector(this.Breadcrumbs));
            result.Collectors.Add(new AttachmentsCollector(this.Attachments, additionalAttachments));
            return result;
        }
        protected override IExceptionReportSender CreateExceptionReportSender() {
            WPFExceptionReportSender defaultSender = new WPFExceptionReportSender();
            defaultSender.ConfirmSendReport = ConfirmSendReport;
            defaultSender.ProxyCredentials = ProxyCredentials;
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
            ForceUpdateBreadcrumbsMaxCount();
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
        protected override IStackTraceHelper CreateStackTraceHelper() {
            return new StackTraceHelper();
        }

        void OnCurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            if (e != null && e.Exception != null) {
                ReportException(e.Exception, null, null);
            }
        }

        protected override ReportConfirmationModel CreateConfirmationModel(LogifyClientExceptionReport report, Func<LogifyClientExceptionReport, bool> sendAction) {
            return new ConfirmationDialogModel(report, sendAction);
        }
        protected override bool RaiseConfirmationDialogShowing(ReportConfirmationModel model) {
            if(ConfirmationDialogShowing != null) {
                ConfirmationDialogModel actualModel = model as ConfirmationDialogModel;
                if(actualModel == null)
                    return false;
                ConfirmationDialogEventArgs args = new ConfirmationDialogEventArgs(actualModel);
                ConfirmationDialogShowing(this, args);
                return args.Handled;
            }
            return false;
        }


        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (e == null)
                return;
            Exception ex = e.ExceptionObject as Exception;

            if (ex != null)
                ReportException(ex, null, null);
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