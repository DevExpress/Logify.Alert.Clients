using System;
using System.Collections.Generic;

namespace DevExpress.Logify.Core {
    public class CompositeExceptionReportSender : ExceptionReportSenderSkeleton {
        readonly List<IExceptionReportSender> senders = new List<IExceptionReportSender>();
        public IList<IExceptionReportSender> Senders { get { return senders; } }

        public bool StopWhenFirstSuccess { get; set; }
        public override string ApiKey {
            get { return base.ApiKey; }
            set {
                base.ApiKey = value;
                foreach (IExceptionReportSender sender in Senders)
                    sender.ApiKey = value;
            }
        }
        public override string ServiceUrl {
            get { return base.ServiceUrl; }
            set {
                base.ServiceUrl = value;
                foreach (IExceptionReportSender sender in Senders)
                    sender.ServiceUrl = value;
            }
        }
        public override int RetryCount {
            get { return base.RetryCount; }
            set {
                base.RetryCount = value;
                foreach (IExceptionReportSender sender in Senders) {
                    ExceptionReportSenderSkeleton instance = sender as ExceptionReportSenderSkeleton;
                    if (instance != null)
                        instance.RetryCount = value;
                }
            }
        }
        public override string MiniDumpServiceUrl {
            get { return base.MiniDumpServiceUrl; }
            set {
                base.MiniDumpServiceUrl = value;
                foreach (IExceptionReportSender sender in Senders) {
                    ExceptionReportSenderSkeleton instance = sender as ExceptionReportSenderSkeleton;
                    if (instance != null)
                        instance.MiniDumpServiceUrl = value;
                }
            }
        }
        public override int ReportTimeoutMilliseconds {
            get { return base.ReportTimeoutMilliseconds; }
            set {
                base.ReportTimeoutMilliseconds = value;
                foreach (IExceptionReportSender sender in Senders) {
                    ExceptionReportSenderSkeleton instance = sender as ExceptionReportSenderSkeleton;
                    if (instance != null)
                        instance.ReportTimeoutMilliseconds = value;
                }
            }
        }

        public override bool CanSendExceptionReport() {
            int count = Senders.Count;
            for (int i = 0; i < count; i++)
                if (Senders[i].CanSendExceptionReport())
                    return true;
            return false;
        }

        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            bool result = false;
            int count = Senders.Count;
            for (int i = 0; i < count; i++) {
                try {
                    if (Senders[i].CanSendExceptionReport()) {
                        bool success = Senders[i].SendExceptionReport(report);
                        result = true;
                        if (success && StopWhenFirstSuccess)
                            break;

                    }
                } catch {
                }
            }
            return result;
        }

        public override IExceptionReportSender CreateEmptyClone() {
            return new CompositeExceptionReportSender();
        }
        public override void CopyFrom(IExceptionReportSender instance) {
            base.CopyFrom(instance);


            CompositeExceptionReportSender other = instance as CompositeExceptionReportSender;
            if (other == null)
                return;

            for (int i = 0; i < other.Senders.Count; i++)
                this.Senders.Add(other.Senders[i].Clone());

            this.StopWhenFirstSuccess = other.StopWhenFirstSuccess;
        }
    }

    public class FirstSuccessfullExceptionReportSender : CompositeExceptionReportSender {
        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            bool result = false;
            int count = Senders.Count;
            for (int i = 0; i < count; i++) {
                try {
                    if (Senders[i].CanSendExceptionReport()) {
                        Senders[i].SendExceptionReport(report);
                        result = true;
                    }
                } catch {
                }
            }
            return result;
        }
    }
}