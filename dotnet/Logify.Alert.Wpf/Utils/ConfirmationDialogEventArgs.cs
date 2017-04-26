using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevExpress.Logify.WPF {
    public class ConfirmationDialogEventArgs : EventArgs {
        public ConfirmationDialogModel Model { get; private set; }
        public bool Handled { get; set; }

        public ConfirmationDialogEventArgs(ConfirmationDialogModel model) {
            this.Model = model;
            Handled = false;
        }
    }

    public delegate void ConfirmationDialogEventHandler(object sender, ConfirmationDialogEventArgs e);
}
