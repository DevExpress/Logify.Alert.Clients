using DevExpress.Logify.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DevExpress.Logify.WPF {
    /// <summary>
    /// Interaction logic for ConfirmReportSendForm.xaml
    /// </summary>
    public partial class ConfirmReportSendForm : Window {
        ReportConfirmationModel Model { get; set; }

        public ConfirmReportSendForm() {
            InitializeComponent();
        }

        public ConfirmReportSendForm(ReportConfirmationModel model) {
            InitializeComponent();

            DataContext = model;
            Model = model;

            Initialize();
        }

        void Initialize() {
            if (Model == null)
                return;

            if (Model.Comments != null)
                this.txtComments.Text = Model.Comments;
        }

        void OnSendClick(object sender, RoutedEventArgs e) {
            try {
                if (Model == null)
                    return;

                Model.Comments = txtComments.Text;
                BackgroundSendModel sendModel = BackgroundSendModel.SendReportInBackgroundThread(Model.SendReport);

                ReportSendProgressForm progressForm = new ReportSendProgressForm(sendModel);
                progressForm.Owner = this;
                progressForm.ShowDialog();
                ReportSendProgressDialogResult result = progressForm.Result;
                if (result == ReportSendProgressDialogResult.OK)
                    this.DialogResult = true;
                else if (result == ReportSendProgressDialogResult.Retry)
                    MessageBox.Show(this, "Unable to send, please try again", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch {
            }
        }

        void OnCancelClick(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }
    }
}
