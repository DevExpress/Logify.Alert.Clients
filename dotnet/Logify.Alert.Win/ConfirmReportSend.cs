using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Win {
    public partial class ConfirmReportSendForm : Form {
        protected ReportConfirmationModel Model { get; set; }

        public ConfirmReportSendForm() {
            InitializeComponent();
        }
        public ConfirmReportSendForm(ReportConfirmationModel model) {
            InitializeComponent();

            this.Model = model;

            Initialize();
        }

        void Initialize() {
            if (Model == null)
                return;

            if (Model.Comments != null)
                this.txtComments.Text = Model.Comments;
            if (Model.Details != null)
                this.txtProblemDetails.Text = Model.Details;

            if (String.IsNullOrEmpty(Model.WindowCaption))
                this.Text += " \"" + Application.ProductName + "\"";
            else
                this.Text = Model.WindowCaption;

            if (!String.IsNullOrEmpty(Model.InformationText))
                lblInfo.Text = Model.InformationText;
        }

        void btnSend_Click(object sender, EventArgs e) {
            try {
                if (Model == null)
                    return;
                if (Model.OriginalReport == null)
                    return;

                LogifyClientExceptionReport report = Model.CreateReportWithUserComments(txtComments.Text);

                if (Model.SendAction != null) {
                    BackgroundSendModel sendModel = BackgroundSendModel.SendReportInBackgroundThread(report, Model.SendAction);

                    ReportSendProgressForm progressForm = new ReportSendProgressForm(sendModel);
                    DialogResult result = progressForm.ShowDialog(this);
                    if (result == DialogResult.OK)
                        this.DialogResult = DialogResult.OK;
                    else if (result == DialogResult.Retry)
                        MessageBox.Show(this, "Unable to send, please try again", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch {
            }
        }

        void btnDontSend_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
