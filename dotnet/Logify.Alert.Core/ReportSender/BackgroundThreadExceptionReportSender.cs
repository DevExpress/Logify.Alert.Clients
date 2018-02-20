using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevExpress.Logify.Core.Internal {
    public class BackgroundThreadExceptionReportSender : BackgroundExceptionReportSender {
        public BackgroundThreadExceptionReportSender(IExceptionReportSender innerSender)
            : base(innerSender) {
        }
        protected override bool SendExceptionReportInBackground(IExceptionReportSender innerSender, LogifyClientExceptionReport report) {
            BackgroundSendModelAccessor.FixWebRequestDeadlock();
            Thread thread = new Thread(() => {
                if (innerSender != null)
                    innerSender.SendExceptionReport(report);

            });
            //thread.Priority = ThreadPriority.Highest;
            thread.Start();
            return true;
            //Thread.Sleep(3000);
        }
#if ALLOW_ASYNC
        protected override Task<bool> SendExceptionReportInBackgroundAsync(IExceptionReportSender innerSender, LogifyClientExceptionReport report) {
            BackgroundSendModelAccessor.FixWebRequestDeadlock();
            Thread thread = new Thread(() => {
                if (innerSender != null)
                    innerSender.SendExceptionReport(report);

            });
            //thread.Priority = ThreadPriority.Highest;
            thread.Start();
            //Thread.Sleep(3000);
            return Task.FromResult(true);
        }
#endif
        public override IExceptionReportSender CreateEmptyClone() {
            return new BackgroundThreadExceptionReportSender(InnerSender.Clone());
        }
    }
    public class EmptyBackgroundExceptionReportSender : BackgroundExceptionReportSender {
        public EmptyBackgroundExceptionReportSender(IExceptionReportSender innerSender)
            : base(innerSender) {
        }
        protected override bool SendExceptionReportInBackground(IExceptionReportSender innerSender, LogifyClientExceptionReport report) {
            if (innerSender != null)
                return innerSender.SendExceptionReport(report);
            else
                return false;
        }
#if ALLOW_ASYNC
        protected override async Task<bool> SendExceptionReportInBackgroundAsync(IExceptionReportSender innerSender, LogifyClientExceptionReport report) {
            if (innerSender != null)
                return await innerSender.SendExceptionReportAsync(report);
            else
                return false;
        }
#endif
        public override IExceptionReportSender CreateEmptyClone() {
            return new EmptyBackgroundExceptionReportSender(InnerSender.Clone());
        }
    }
}