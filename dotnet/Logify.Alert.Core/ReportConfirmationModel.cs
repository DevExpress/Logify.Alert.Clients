using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Core {
    public class ReportConfirmationModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        string comments = "";
        public string Comments {
            get { return comments; }
            set {
                if (comments != value) {
                    comments = value;
                    OnPropertyChanged("Comments");
                }
            }
        }

        string details = null;
        public string Details {
            get {
                if (details == null) {
                    details = OriginalReport != null ? OriginalReport.ReportString : "";
                }
                return details;
            }
            set {
                if (details != value) {
                    details = value;
                    OnPropertyChanged("Details");
                }
            }
        }
        internal Func<LogifyClientExceptionReport, bool> SendAction { get; set; }
        internal LogifyClientExceptionReport OriginalReport { get; set; }

        protected ReportConfirmationModel(LogifyClientExceptionReport report, Func<LogifyClientExceptionReport, bool> sendAction) {
            this.OriginalReport = report;
            this.SendAction = sendAction;
        }

        public bool SendReport() {
            LogifyClientExceptionReport resultReport = ReportCommentAppender.CreateReportWithUserComments(OriginalReport, Comments);
            if (SendAction != null)
                return SendAction(resultReport);

            return false;
        }

        void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
namespace DevExpress.Logify.Core.Internal {
    public class ReportCommentAppender {
        public static LogifyClientExceptionReport CreateReportWithUserComments(LogifyClientExceptionReport originalReport, string userComments) {
            if(originalReport == null)
                return null;

            LogifyClientExceptionReport report = originalReport;
            if(!String.IsNullOrEmpty(userComments)) {
                report = report.Clone();
                AppendUserComments(report, userComments);
            }
            return report;
        }

        static void AppendUserComments(LogifyClientExceptionReport report, string comments) {
            if(report == null || report.ReportContent == null)
                return;

            if(String.IsNullOrEmpty(comments))
                return;
            comments = comments.Trim();

            if(String.IsNullOrEmpty(comments))
                return;

            StringBuilder reportContent = report.ReportContent;
            int lastBraceIndex = -1;
            for(int i = reportContent.Length - 1; i >= 0; i--) {
                if(reportContent[i] == '}') {
                    lastBraceIndex = i;
                    break;
                }
            }
            if(lastBraceIndex < 0)
                return;

            string commentsContent = GenerateCommentsContent(comments);
            if(String.IsNullOrEmpty(commentsContent))
                return;

            report.ReportContent = reportContent.Insert(lastBraceIndex, commentsContent);
            report.ResetReportString();
        }

        static string GenerateCommentsContent(string value) {
            StringBuilder content = new StringBuilder();
            StringWriter writer = new StringWriter(content);
            TextWriterLogger logger = new TextWriterLogger(writer);

            logger.WriteValue("userComments", value);
            return content.ToString();
        }
    }
}