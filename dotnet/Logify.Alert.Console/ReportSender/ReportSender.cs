using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Console {
    public class ConsoleExceptionReportSender : ServiceExceptionReportSender {
        public override IExceptionReportSender CreateEmptyClone() {
            return new ConsoleExceptionReportSender();
        }
    }
}
