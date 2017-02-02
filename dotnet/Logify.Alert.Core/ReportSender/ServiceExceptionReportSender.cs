using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Logify.Core;
using System.Threading;

namespace DevExpress.Logify.Core {
    public abstract class ServiceExceptionReportSender : ExceptionReportSenderSkeleton {
        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            //    return false;
            //}
            //protected bool SendExceptionReportCore2(LogifyClientExceptionReport report) {
#if DEBUG
            try {
                System.IO.File.WriteAllText(@"C:\exception.log", report.ReportString);
            } catch(Exception) { }

#endif
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(ServiceUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", this.ApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "newreport") {
                    Content =
                        new StringContent(report.ReportString, Encoding.UTF8,
                            "application/json")
                };
                //client.DefaultRequestHeaders.ProxyAuthorization = new AuthenticationHeaderValue("amx", this.ApiKey);

                HttpResponseMessage message = client.SendAsync(request).Result;
                //client.SendAsync(request).Wait();
                return message != null && message.StatusCode == HttpStatusCode.OK;
                /*
                Task<HttpResponseMessage> task = client.SendAsync(request);
                if (task.Wait(ReportTimeoutMilliseconds) && task.IsCompleted) {
                    HttpResponseMessage message = task.Result;
                    if (message == null)
                        return false;
                    return message.StatusCode == HttpStatusCode.OK;
                }
                else
                    return false;
                */
                //Debug.WriteLine(client.SendAsync(request).Result);
            }
        }
#if NET45
        protected override async Task<bool> SendExceptionReportCoreAsync(LogifyClientExceptionReport report) {
#if DEBUG
            try {
                System.IO.File.WriteAllText(@"C:\exception.log", report.ReportString);
            } catch(Exception) { }
            
#endif
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(ServiceUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", this.ApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "newreport") {
                    Content =
                        new StringContent(report.ReportString, Encoding.UTF8,
                            "application/json")
                };
                //client.DefaultRequestHeaders.ProxyAuthorization = new AuthenticationHeaderValue("amx", this.ApiKey);

                HttpResponseMessage message = await client.SendAsync(request);
                return message != null && message.StatusCode == HttpStatusCode.OK;
            }
        }
#endif
    }

    public class ReportConfirmationModel {
        public string Comments { get; set; }
        public string Details { get; set; }

        public string WindowCaption { get; set; }
        public string InformationText { get; set; }

        internal Func<LogifyClientExceptionReport, bool> SendAction { get; set; }
        internal LogifyClientExceptionReport OriginalReport { get; set; }

        internal LogifyClientExceptionReport CreateReportWithUserComments(string userComments) {
            if (OriginalReport == null)
                return null;

            LogifyClientExceptionReport report = OriginalReport;
            Comments = userComments;
            if (!String.IsNullOrEmpty(Comments)) {
                report = report.Clone();
                AppendUserComments(report, Comments);
            }
            return report;
        }
        void AppendUserComments(LogifyClientExceptionReport report, string comments) {
            if (report == null || report.ReportContent == null)
                return;

            if (String.IsNullOrEmpty(comments))
                return;
            comments = comments.Trim();

            if (String.IsNullOrEmpty(comments))
                return;

            StringBuilder reportContent = report.ReportContent;
            int lastBraceIndex = -1;
            for (int i = reportContent.Length - 1; i >= 0; i--) {
                if (reportContent[i] == '}') {
                    lastBraceIndex = i;
                    break;
                }
            }
            if (lastBraceIndex < 0)
                return;

            string commentsContent = GenerateCommentsContent(comments);
            if (String.IsNullOrEmpty(commentsContent))
                return;

            report.ReportContent = reportContent.Insert(lastBraceIndex, commentsContent);
            report.ResetReportString();
        }

        string GenerateCommentsContent(string value) {
            StringBuilder content = new StringBuilder();
            StringWriter writer = new StringWriter(content);
            TextWriterLogger logger = new TextWriterLogger(writer);

            logger.WriteValue("userComments", value);
            return content.ToString();
        }

    }

    public class BackgroundSendModel {
        public Func<LogifyClientExceptionReport, bool> SendAction { get; set; }
        public LogifyClientExceptionReport Report { get; set; }
        public Thread Thread { get; set; }
        public bool SendComplete { get; set; }
        public bool SendResult { get; set; }

        internal static BackgroundSendModel SendReportInBackgroundThread(LogifyClientExceptionReport report, Func<LogifyClientExceptionReport, bool> sendAction) {
            Thread thread = new Thread(BackgroundSend);
            BackgroundSendModel sendModel = new BackgroundSendModel();
            sendModel.SendAction = sendAction;
            sendModel.Report = report;
            sendModel.Thread = thread;
            thread.Start(sendModel);
            return sendModel;
        }

        static void BackgroundSend(object obj) {
            BackgroundSendModel model = obj as BackgroundSendModel;
            if (model == null)
                return;

            try {
                if (model.SendAction == null || model.Report == null) {
                    model.SendResult = false;
                    //model.SendComplete = true;
                    return;
                }

                model.SendResult = model.SendAction(model.Report);
                //model.SendComplete = true;
            }
            finally {
                model.SendComplete = true;
            }
        }
    }
}
