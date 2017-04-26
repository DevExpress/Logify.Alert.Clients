using System;
using DevExpress.Logify.Core;
using System.Windows;
using System.Threading.Tasks;

namespace DevExpress.Logify.WPF {
    public class WPFExceptionReportSender : ServiceWithConfirmationExceptionReportSender {
        public override IExceptionReportSender CreateEmptyClone() {
            return new WPFExceptionReportSender();
        }
        protected override bool ShowBuiltInConfirmSendForm(ReportConfirmationModel model) {
            ConfirmReportSendForm form = new ConfirmReportSendForm(model);
            Window activeWindow = GetActiveWindow();
            if (activeWindow != null)
                form.Owner = activeWindow;
            form.ShowDialog();
            return true;
        }
        Window GetActiveWindow() {
            try {
                Application app = Application.Current;
                if (app == null)
                    return null;

                WindowCollection windows = app.Windows;
                if (windows == null || windows.Count <= 0)
                    return null;

                int count = windows.Count;
                for (int i = 0; i < count; i++)
                    if (windows[i].IsActive)
                        return windows[i];
            }
            catch {
            }
            return null;
        }
#if ALLOW_ASYNC
        public override async Task<bool> SendExceptionReportAsync(LogifyClientExceptionReport report) {
            //no dialog in async version
            return await base.SendExceptionReportAsync(report);
        }
#endif
    }
}

