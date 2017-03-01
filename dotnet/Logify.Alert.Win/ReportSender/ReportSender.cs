using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Win {
    public class WinFormsExceptionReportSender : ServiceExceptionReportSender {
        public override IExceptionReportSender CreateEmptyClone() {
            return new WinFormsExceptionReportSender();
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
                    ShowConfirmSendForm(form);
                    return true; // report successful send
                }
                catch {
                }
            }
            return base.SendExceptionReport(report);
        }
        void ShowConfirmSendForm(ConfirmReportSendForm form) {
            Form activeForm = null;
            try {
                activeForm = Form.ActiveForm;
                if (activeForm != null) {
                    IntPtr handle = activeForm.Handle;
                    handle = IntPtr.Zero;
                }
            }
            catch {
                activeForm = null;
            }

            if (activeForm != null)
                form.ShowDialog(activeForm);
            else
                form.ShowDialog();
        }
#if ALLOW_ASYNC
        public override async Task<bool> SendExceptionReportAsync(LogifyClientExceptionReport report) {
            //no dialog in async version
            return await base.SendExceptionReportAsync(report);
        }
#endif
    }
}
