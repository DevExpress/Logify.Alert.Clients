using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Logify.Core;
using System.Threading;

#if NETSTANDARD
using System.Net.Http;
using System.Net.Http.Headers;
#endif

namespace DevExpress.Logify.Core {
    public abstract class ServiceExceptionReportSender : ExceptionReportSenderSkeleton {
        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
#if NETSTANDARD
            return SendViaHttpClient(report);
#else
            return SendViaHttpWebRequest(report);
#endif
        }
#if ALLOW_ASYNC
        protected override async Task<bool> SendExceptionReportCoreAsync(LogifyClientExceptionReport report) {
#if NETSTANDARD
            return await SendViaHttpClientAsync(report);
#else
            return await SendViaHttpWebRequestAsync(report);
#endif
        }
#endif

#if !NETSTANDARD
        WebRequest CreateAndSetupHttpWebRequest(LogifyClientExceptionReport report) {
            string url = ServiceUrl;
            if (!string.IsNullOrEmpty(url)) {
                if (url[url.Length - 1] != '/')
                    url += '/';
                url += "newreport";
            }
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.Headers.Add("Authorization", "amx " + this.ApiKey);
            request.ContentType = "application/json";

            byte[] buffer = Encoding.UTF8.GetBytes(report.ReportString);
            request.ContentLength = buffer.Length;
            using (Stream content = request.GetRequestStream()) {
                content.Write(buffer, 0, buffer.Length);
                content.Flush();
            }
            return request;
        }
        bool SendViaHttpWebRequest(LogifyClientExceptionReport report) {
            WebRequest request = CreateAndSetupHttpWebRequest(report);
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            return response != null && response.StatusCode == HttpStatusCode.OK;
        }
#if ALLOW_ASYNC
        async Task<bool> SendViaHttpWebRequestAsync(LogifyClientExceptionReport report) {
            WebRequest request = CreateAndSetupHttpWebRequest(report);
            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            return response != null && response.StatusCode == HttpStatusCode.OK;
        }
#endif
#endif
#if NETSTANDARD
        bool SendViaHttpClient(LogifyClientExceptionReport report) {
            using (HttpClient client = CreateAndSetupHttpClient()) {
                HttpRequestMessage request = CreateHttpRequest(report);
                HttpResponseMessage message = client.SendAsync(request).Result;
                return message != null && message.StatusCode == HttpStatusCode.OK;
            }
        }
        HttpClient CreateAndSetupHttpClient() {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ServiceUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", this.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        HttpRequestMessage CreateHttpRequest(LogifyClientExceptionReport report) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "newreport") {
                Content = new StringContent(report.ReportString, Encoding.UTF8, "application/json")
            };
            return request;
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

    public abstract class ServiceWithConfirmationExceptionReportSender : ServiceExceptionReportSender {
        static bool isFormShown;

        public override bool SendExceptionReport(LogifyClientExceptionReport report) {
            if (ConfirmSendReport && !isFormShown) {
                try {
                    ReportConfirmationModel model = new ReportConfirmationModel();
                    model.Comments = String.Empty;
                    model.Details = report.ReportString;
                    model.InformationText = String.Empty;
                    model.WindowCaption = String.Empty;
                    model.OriginalReport = report;
                    model.SendAction = (r) => { return base.SendExceptionReport(r); };

                    isFormShown = true;
                    return ShowConfirmSendForm(model);
                }
                catch {
                }
                finally {
                    isFormShown = false;
                }
            }
            return base.SendExceptionReport(report);
        }

        protected abstract bool ShowConfirmSendForm(ReportConfirmationModel model);
    }
}
