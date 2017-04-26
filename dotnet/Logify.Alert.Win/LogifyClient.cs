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

namespace DevExpress.Logify.Win {
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

        public static new LogifyAlert Instance {
            get {
                if (instance != null)
                    return instance;

                lock (typeof(LogifyAlert)) {
                    if (instance != null)
                        return instance;

                    instance = new LogifyAlert(true);
                    LogifyClientBase.Instance = instance;
                }
                return instance;
            }
        }

        public event ConfirmationDialogEventHandler ConfirmationDialogShowing;
        //public bool SendReportInSeparateProcess { get; set; }

        protected internal LogifyAlert(Dictionary<string, string> config) : base(config) {
        }

        protected override IInfoCollectorFactory CreateCollectorFactory() {
            return new WinFormsExceptionCollectorFactory();
        }
        protected override IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config, IDictionary<string, string> additionalCustomData, AttachmentCollection additionalAttachments) {
            WinFormsExceptionCollector result = new WinFormsExceptionCollector(config);
            result.AppName = this.AppName;
            result.AppVersion = this.AppVersion;
            result.UserId = this.UserId;
            result.Collectors.Add(new CustomDataCollector(this.CustomData, additionalCustomData));
            result.Collectors.Add(new AttachmentsCollector(this.Attachments, additionalAttachments));
            return result;
        }
        protected override IExceptionReportSender CreateExceptionReportSender() {
            WinFormsExceptionReportSender defaultSender = new WinFormsExceptionReportSender();
            defaultSender.ConfirmSendReport = ConfirmSendReport;
            if (ConfirmSendReport)
                return defaultSender;

            //IExceptionReportSender winDefaultSender = base.CreateExceptionReportSender();
            CompositeExceptionReportSender sender = new CompositeExceptionReportSender();
            sender.StopWhenFirstSuccess = true;
            sender.Senders.Add(new ExternalProcessExceptionReportSender());
            sender.Senders.Add(defaultSender);
            sender.Senders.Add(new OfflineDirectoryExceptionReportSender());
            return sender;
        }
        protected override IExceptionReportSender CreateEmptyPlatformExceptionReportSender() {
            return new WinFormsExceptionReportSender();
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
            Exception ex = e.ExceptionObject as Exception;

            if (ex != null)
                ReportException(ex, null, null);
        }
        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e) {
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

        Mutex mutex;
        protected override bool DetectIfSecondaryInstance() {
            try {
                int pid = Process.GetCurrentProcess().Id;
                string name = "logifypid" + pid.ToString();
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

    public class ConfirmationDialogModel : ReportConfirmationModel {
        internal ConfirmationDialogModel(LogifyClientExceptionReport report, Func<LogifyClientExceptionReport, bool> sendAction) : base(report, sendAction) {
        }
    }
    public class ConfirmationDialogEventArgs : EventArgs {
        public ConfirmationDialogModel Model { get; private set; }
        public bool Handled { get; set; }

        public ConfirmationDialogEventArgs(ConfirmationDialogModel model) {
            this.Model = model;
            Handled = false;
        }
    }

    public delegate void ConfirmationDialogEventHandler(object sender, ConfirmationDialogEventArgs e);
}