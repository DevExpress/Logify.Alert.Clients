using DevExpress.Logify.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DevExpress.Logify.WPF {

    public enum ReportSendProgressDialogResult {
        OK,
        Retry,
        Cancel
    }
    /// <summary>
    /// Interaction logic for ReportSendProgressForm.xaml
    /// </summary>
    public partial class ReportSendProgressForm : Window {
        BackgroundSendModel Model { get; set; }
        System.Windows.Forms.Timer timer;

        public ReportSendProgressForm() {
            InitializeComponent();
        }

        public ReportSendProgressForm(BackgroundSendModel model) {
            InitializeComponent();

            Model = model;
            if (Model != null) {
                this.timer = new System.Windows.Forms.Timer();
                this.timer.Tick += OnTimerTick;
                this.timer.Interval = 100;
                this.timer.Start();
            }
        }

        public ReportSendProgressDialogResult Result { get; set; }

        void OnTimerTick(object sender, EventArgs e) {
            if (Model == null)
                return;

            try {
                if (Model.SendComplete || (Model.Thread != null && (Model.Thread.ThreadState == ThreadState.AbortRequested || Model.Thread.ThreadState == ThreadState.Aborted))) {
                    timer.Stop();
                    if (Model.SendResult) {
                        this.Result = ReportSendProgressDialogResult.OK;
                        this.DialogResult = true;
                    }
                    else {
                        this.Result = ReportSendProgressDialogResult.Retry;
                        this.DialogResult = false;
                    }
                }
            }
            catch {
            }
        }

        void OnCancelClick(object sender, RoutedEventArgs e) {
            if (Model == null)
                return;

            try {
                if (Model.Thread != null)
                    Model.Thread.Abort();
            }
            catch {
            }
            this.Result = ReportSendProgressDialogResult.Cancel;
            this.DialogResult = false;
        }
    }
}
