using System;
using System.Threading;

namespace DevExpress.Logify.Core {
    public class BackgroundThreadExceptionReportSender : BackgroundExceptionReportSender {
        public BackgroundThreadExceptionReportSender(IExceptionReportSender innerSender)
            : base(innerSender) {
        }
        protected override void SendExceptionReportInBackground(IExceptionReportSender innerSender, LogifyClientExceptionReport report) {
            Thread thread = new Thread(() => {
                if (innerSender != null)
                    innerSender.SendExceptionReport(report);

            });
            //thread.Priority = ThreadPriority.Highest;
            thread.Start();
            //Thread.Sleep(3000);
        }

        public override IExceptionReportSender CreateEmptyClone() {
            return new BackgroundThreadExceptionReportSender(InnerSender.Clone());
        }
    }
    public class EmptyBackgroundExceptionReportSender : BackgroundExceptionReportSender {
        public EmptyBackgroundExceptionReportSender(IExceptionReportSender innerSender)
            : base(innerSender) {
        }
        protected override void SendExceptionReportInBackground(IExceptionReportSender innerSender, LogifyClientExceptionReport report) {
            if (innerSender != null)
                innerSender.SendExceptionReport(report);
        }

        public override IExceptionReportSender CreateEmptyClone() {
            return new EmptyBackgroundExceptionReportSender(InnerSender.Clone());
        }
    }
}