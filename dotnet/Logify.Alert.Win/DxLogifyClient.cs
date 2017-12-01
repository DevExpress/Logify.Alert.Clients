using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using DevExpress.Logify.Core;
using DevExpress.Logify.Core.Internal;
using System.ComponentModel;

namespace DevExpress.Logify.Win {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DxLogifyClient : LogifyAlert {
        public DxLogifyClient() : base(true) {
        }
        public void ReportToDevExpress2(string logId, string lastExceptionReportFileName, Assembly asm, IDictionary<string, string> customData) {
            ReportToDevExpressCore(logId, lastExceptionReportFileName, asm, customData);
        }
        
        public void ReportToDevExpress(string logId, string lastExceptionReportFileName, Assembly asm) {
            ReportToDevExpressCore(logId, lastExceptionReportFileName, asm, new Dictionary<string, string>());
        }

        protected override void AddCollectors(RootInfoCollector root) {
            base.AddCollectors(root);
            root.Collectors.Add(new DevExpressVersionsInStackCollector());
        }

        //protected override IExceptionReportSender CreateExceptionReportSender() {
        //    IExceptionReportSender winDefaultSender = base.CreateExceptionReportSender();
        //    CompositeExceptionReportSender sender = new CompositeExceptionReportSender();
        //    sender.StopWhenFirstSuccess = true;
        //    sender.Senders.Add(new ExternalProcessExceptionReportSender());
        //    sender.Senders.Add(winDefaultSender);
        //    return sender;
        //}
    }
}
