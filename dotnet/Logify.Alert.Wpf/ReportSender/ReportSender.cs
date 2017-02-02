using System;
using DevExpress.Logify.Core;
using System.Windows;

namespace DevExpress.Logify.WPF {
    public class WPFExceptionReportSender : ServiceExceptionReportSender {
        public override IExceptionReportSender CreateEmptyClone() {
            return new WPFExceptionReportSender();
        }
        public override bool SendExceptionReport(LogifyClientExceptionReport report) {
            if (ConfirmSendReport) {
                try {
                    ReportConfirmationModel model = new ReportConfirmationModel();
                    model.Comments = String.Empty;
                    model.Details = report.ReportString;
                    model.InformationText = String.Empty;
                    model.WindowCaption = String.Empty;
                    model.OriginalReport = report;
                    model.SendAction = (r) => { return base.SendExceptionReport(r); };
                    ConfirmReportSendForm form = new ConfirmReportSendForm(model);
                    Window activeWindow = GetActiveWindow();
                    if (activeWindow != null)
                        form.Owner = activeWindow;
                    form.ShowDialog();
                    return true; // report successful send
                }
                catch {
                }
            }
            return base.SendExceptionReport(report);
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
#if NET45
        public override async Task<bool> SendExceptionReportAsync(LogifyClientExceptionReport report) {
            //no dialog in async version
            return await base.SendExceptionReportAsync(report);
        }
#endif
    }
}

