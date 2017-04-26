using DevExpress.Logify.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DevExpress.Logify.WPF {
    public class ConfirmationDialogModel : ReportConfirmationModel {
        public ICommand Send { get; private set; }
        internal ConfirmationDialogModel(LogifyClientExceptionReport report, Func<LogifyClientExceptionReport, bool> sendAction) : base(report, sendAction) {
            Send = new SendReportCommand(SendCommandExecute);
        }
                
        void SendCommandExecute(object parameter) {
            SendReport();
        }
    }

    public class SendReportCommand : ICommand {
        private Action<object> _execute;

        EventHandler onCanExecuteChanged;
        public event EventHandler CanExecuteChanged { add { onCanExecuteChanged += value; } remove { onCanExecuteChanged -= value; } }

        public SendReportCommand(Action<object> execute) {
            this._execute = execute;
        }
        
        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            _execute(parameter);
        }
    }
}
