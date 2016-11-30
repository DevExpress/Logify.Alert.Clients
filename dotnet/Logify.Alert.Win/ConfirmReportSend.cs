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

                LogifyClientExceptionReport report = Model.OriginalReport;
                Model.Comments = txtComments.Text;
                if (!String.IsNullOrEmpty(Model.Comments)) {
                    report = report.Clone();
                    AppendUserComments(report, Model.Comments);
                }

                if (Model.SendAction != null) {
                    Thread thread = new Thread(BackgroundSend);
                    BackgroundSendModel sendModel = new BackgroundSendModel();
                    sendModel.SendAction = Model.SendAction;
                    sendModel.Report = report;
                    sendModel.Thread = thread;
                    thread.Start(sendModel);

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


        void BackgroundSend(object obj) {
            BackgroundSendModel model = obj as BackgroundSendModel;
            if (model == null)
                return;

            try {
                if (model.SendAction == null || model.Report == null) {
                    model.SendResult = false;
                    //model.SendComplete = true;
                    return;
                }

                model.SendResult = model.SendAction(model.Report);
                //model.SendComplete = true;
            }
            finally {
                model.SendComplete = true;
            }
        }

        void btnDontSend_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }

        void AppendUserComments(LogifyClientExceptionReport report, string comments) {
            if (report == null || report.ReportContent == null)
                return;

            if (String.IsNullOrEmpty(comments))
                return;
            comments = comments.Trim();

            if (String.IsNullOrEmpty(comments))
                return;

            StringBuilder reportContent = report.ReportContent;
            int lastBraceIndex = -1;
            for (int i = reportContent.Length - 1; i >= 0; i--) {
                if (reportContent[i] == '}') {
                    lastBraceIndex = i;
                    break;
                }
            }
            if (lastBraceIndex < 0)
                return;

            string commentsContent = GenerateCommentsContent(comments);
            if (String.IsNullOrEmpty(commentsContent))
                return;

            report.ReportContent = reportContent.Insert(lastBraceIndex, commentsContent);
            report.ResetReportString();
        }

        string GenerateCommentsContent(string value) {
            StringBuilder content = new StringBuilder();
            StringWriter writer = new StringWriter(content);
            TextWriterLogger logger = new TextWriterLogger(writer);

            logger.WriteValue("userComments", value);
            return content.ToString();
        }
    }

    public class ReportConfirmationModel {
        public string Comments { get; set; }
        public string Details { get; set; }

        public string WindowCaption { get; set; }
        public string InformationText { get; set; }

        internal Func<LogifyClientExceptionReport, bool> SendAction { get; set; }
        internal LogifyClientExceptionReport OriginalReport { get; set; }
    }

    public class BackgroundSendModel {
        public Func<LogifyClientExceptionReport, bool> SendAction { get; set; }
        public LogifyClientExceptionReport Report { get; set; }
        public Thread Thread { get; set; }
        public bool SendComplete { get; set; }
        public bool SendResult { get; set; }
    }
}
