using System;
using System.Windows.Forms;
using DevExpress.Logify.Core;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Win {
    public partial class ConfirmReportSendForm : Form {
        protected ReportConfirmationModel Model { get; set; }

        public ConfirmReportSendForm() {
            InitializeComponent();
        }
        public ConfirmReportSendForm(ReportConfirmationModel model) {
            InitializeComponent();
            Model = model;
            this.InitializeContent();
        }

        void btnSend_Click(object sender, EventArgs e) {
            this.Send();
        }
        void btnDontSend_Click(object sender, EventArgs e) {
            this.Cancel();
        }

        protected virtual void InitializeContent() {
            if (Model == null)
                return;

            if (Model.Comments != null)
                this.txtComments.Text = Model.Comments;
            if (Model.Details != null)
                this.txtProblemDetails.Text = Model.Details;
            this.Text += " \"" + Application.ProductName + "\"";
        }
        protected virtual void Send() {
            try {
                if (Model == null)
                    return;

                Model.Comments = txtComments.Text;

                BackgroundSendModel sendModel = BackgroundSendModelAccessor.SendReportInBackgroundThread(Model.SendReport);

                ReportSendProgressForm progressForm = new ReportSendProgressForm(sendModel);
                DialogResult result = progressForm.ShowDialog(this);
                if (result == DialogResult.OK)
                    this.DialogResult = DialogResult.OK;
                else if (result == DialogResult.Retry)
                    MessageBox.Show(this, "Unable to send, please try again", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch { }
        }
        protected virtual void Cancel() {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
