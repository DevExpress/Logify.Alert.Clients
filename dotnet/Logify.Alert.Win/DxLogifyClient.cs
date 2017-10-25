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


        protected override IInfoCollector CreateDefaultCollector(IDictionary<string, string> additionalCustomData, AttachmentCollection additionalAttachments) {
            IInfoCollector result = base.CreateDefaultCollector(additionalCustomData, additionalAttachments);
            CompositeInfoCollector collector = result as CompositeInfoCollector;
            if (collector != null) {
                collector.Collectors.Add(new DevExpressVersionsInStackCollector());
            }
            return result;
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
