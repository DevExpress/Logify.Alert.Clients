using DevExpress.Logify.Core;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DevExpress.Logify.Win {
    public partial class ReportSendProgressForm : Form {
        
        readonly System.Windows.Forms.Timer timer;

        public ReportSendProgressForm() {
            InitializeComponent();
        }
        public ReportSendProgressForm(BackgroundSendModel model) {
            InitializeComponent();

            this.Model = model;
            if (Model != null) {
                this.timer = new System.Windows.Forms.Timer();
                this.timer.Tick += OnTimerTick;
                this.timer.Interval = 100;
                this.timer.Start();
            }
        }

        BackgroundSendModel Model { get; set; }

        void OnTimerTick(object sender, EventArgs e) {
            if (Model == null)
                return;

            try {
                if (Model.SendComplete || (Model.Thread != null && (Model.Thread.ThreadState == ThreadState.AbortRequested || Model.Thread.ThreadState == ThreadState.Aborted))) {
                    timer.Stop();
                    if (Model.SendResult)
                        this.DialogResult = DialogResult.OK;
                    else
                        this.DialogResult = DialogResult.Retry;
                }
            }
            catch {
            }
        }

        void btnCancel_Click(object sender, EventArgs e) {
            if (Model == null)
                return;

            try {
                if (Model.Thread != null)
                    Model.Thread.Abort();
            }
            catch {
            }
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
