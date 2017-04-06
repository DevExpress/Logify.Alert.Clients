using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Win {
    public class WinFormsExceptionReportSender : ServiceWithConfirmationExceptionReportSender {
        public override IExceptionReportSender CreateEmptyClone() {
            return new WinFormsExceptionReportSender();
        }

        protected override bool ShowConfirmSendForm(ReportConfirmationModel model) {
            ConfirmReportSendForm form = new ConfirmReportSendForm(model);

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
            return true;
        }
#if ALLOW_ASYNC
        public override async Task<bool> SendExceptionReportAsync(LogifyClientExceptionReport report) {
            //no dialog in async version
            return await base.SendExceptionReportAsync(report);
        }
#endif
    }
}
