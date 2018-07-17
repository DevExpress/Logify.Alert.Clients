using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Logify.Core;
using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;
using System.ComponentModel;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Console {
    public class LogifyAlert : LogifyClientBase {
        static volatile LogifyAlert instance;

        internal LogifyAlert(bool b) {
        }
        protected LogifyAlert(string apiKey) : base(apiKey) {
        }

        public bool CollectMiniDump { get { return Config.CollectMiniDump; } set { Config.CollectMiniDump = value; } }
        internal bool CollectBreadcrumbs { get { return CollectBreadcrumbsCore; } set { CollectBreadcrumbsCore = value; } }
        internal int BreadcrumbsMaxCount { get { return BreadcrumbsMaxCountCore; } set { BreadcrumbsMaxCountCore = value; } }
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
            return new ConsoleExceptionCollector(context);
        }
        protected override ILogifyAppInfo CreateAppInfo(LogifyCollectorContext context) {
            return new ConsoleApplicationCollector();
        }
        protected override IExceptionReportSender CreateExceptionReportSender() {
            IExceptionReportSender defaultSender = CreateEmptyPlatformExceptionReportSender();
            defaultSender.ConfirmSendReport = ConfirmSendReport;
            defaultSender.Proxy = Proxy;
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
            return new ConsoleExceptionReportSender();
        }
        protected override LogifyAlertConfiguration LoadConfiguration() {
            LogifyConfigSection section = ConfigurationManager.GetSection("logifyAlert") as LogifyConfigSection;
            return ClientConfigurationLoader.LoadCommonConfiguration(section);
        }
        public override void Run() {
            if (!IsSecondaryInstance) {
                //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
                Application.ThreadException += OnApplicationThreadException;
                //AppDomain.CurrentDomain.FirstChanceException
                //SendOfflineReports();
            }
        }
        public override void Stop() {
            if (!IsSecondaryInstance) {
                //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
                AppDomain.CurrentDomain.UnhandledException -= OnCurrentDomainUnhandledException;
                Application.ThreadException -= OnApplicationThreadException;
            }
        }

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (e == null)
                return;

            var callArgumentsMap = this.MethodArgumentsMap; // this call should be done before any inner calls
            ResetTrackArguments();
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null) {
                LogifyCollectorContext context = GrabCollectorContext(callArgumentsMap);
                ReportException(ex, context);
            }
        }
        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e) {
            var callArgumentsMap = this.MethodArgumentsMap; // this call should be done before any inner calls
            ResetTrackArguments();
            
            if (e != null && e.Exception != null) {
                LogifyCollectorContext context = GrabCollectorContext(callArgumentsMap);
                ReportException(e.Exception, context);
            }
        }

        protected override ReportConfirmationModel CreateConfirmationModel(LogifyClientExceptionReport report, Func<LogifyClientExceptionReport, bool> sendAction) {
            return null;
        }
        protected override bool RaiseConfirmationDialogShowing(ReportConfirmationModel model) {
            return false;
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