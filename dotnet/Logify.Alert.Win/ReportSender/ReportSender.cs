using System;
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
                    form.ShowDialog(Form.ActiveForm);
                    return true; // report successful send
                }
                catch {
                }
            }
            return base.SendExceptionReport(report);
        }
    }
}
