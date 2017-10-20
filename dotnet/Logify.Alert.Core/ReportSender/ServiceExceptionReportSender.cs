using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Logify.Core;
using System.Threading;
using System.ComponentModel;
using DevExpress.Logify.Core.Internal;

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
            SetupProxy(request);

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
        void SetupProxy(WebRequest request) {
            if (this.ProxyCredentials != null) {
                Uri proxyUri = WebRequest.DefaultWebProxy.GetProxy(new Uri(ServiceUrl));
                if (proxyUri != null) {
                    request.Proxy = new WebProxy(proxyUri, false);

                    request.UseDefaultCredentials = false;
                    request.Proxy.Credentials = ProxyCredentials;
                }
            }
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
        async Task<bool> SendViaHttpClientAsync(LogifyClientExceptionReport report) {
            using (HttpClient client = CreateAndSetupHttpClient()) {
                HttpRequestMessage request = CreateHttpRequest(report);
                HttpResponseMessage message = await client.SendAsync(request);
                return message != null && message.StatusCode == HttpStatusCode.OK;
            }
        }
        HttpClient CreateAndSetupHttpClient() {
            HttpClient client = CreateClientWithProxy();
            client.BaseAddress = new Uri(ServiceUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", this.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        HttpClient CreateClientWithProxy() {
            if (this.Proxy == null)
                return new HttpClient();
            
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            
            httpClientHandler.Proxy = this.Proxy;
            httpClientHandler.UseProxy = true;
            httpClientHandler.PreAuthenticate = true;
            httpClientHandler.UseDefaultCredentials = false;
            
            return new HttpClient(httpClientHandler);
        }

        HttpRequestMessage CreateHttpRequest(LogifyClientExceptionReport report) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "newreport") {
                Content = new StringContent(report.ReportString, Encoding.UTF8, "application/json")
            };
            return request;
        }
        
#endif
    }

    public class BackgroundSendModel {
        public Func<bool> SendAction { get; set; }
        //public LogifyClientExceptionReport Report { get; set; }
        public Thread Thread { get; set; }
        public bool SendComplete { get; set; }
        public bool SendResult { get; set; }

        internal static BackgroundSendModel SendReportInBackgroundThread(Func<bool> sendAction) {
            Thread thread = new Thread(BackgroundSend);
            BackgroundSendModel sendModel = new BackgroundSendModel();
            sendModel.SendAction = sendAction;
            sendModel.Thread = thread;
            thread.Start(sendModel);
            return sendModel;
        }

        static void BackgroundSend(object obj) {
            BackgroundSendModel model = obj as BackgroundSendModel;
            if(model == null)
                return;

            try {
                if(model.SendAction == null) {
                    model.SendResult = false;
                    //model.SendComplete = true;
                    return;
                }

                model.SendResult = model.SendAction();
                //model.SendComplete = true;
            } finally {
                model.SendComplete = true;
            }
        }
    }

    public abstract class ServiceWithConfirmationExceptionReportSender : ServiceExceptionReportSender {
        static bool isFormShown;
        public override bool SendExceptionReport(LogifyClientExceptionReport report) {
            if(ConfirmSendReport && !isFormShown && LogifyClientBase.Instance != null) {
                try {
                    ReportConfirmationModel model = LogifyClientAccessor.CreateConfirmationModel(report, (r) => { return base.SendExceptionReport(r); });
                    if(model == null)
                        return false;
                    isFormShown = true;
                    if(ShowCustomConfirmSendForm(model))
                        return true;
                    return ShowBuiltInConfirmSendForm(model);
                } catch {
                    return false;
                } finally {
                    isFormShown = false;
                }
            } else
                return base.SendExceptionReport(report);
        }
        bool ShowCustomConfirmSendForm(ReportConfirmationModel model) {
            if((LogifyClientBase.Instance != null) && LogifyClientAccessor.RaiseConfirmationDialogShowing(model)) {
                return true;
            }
            return false;
        }
        protected abstract bool ShowBuiltInConfirmSendForm(ReportConfirmationModel model);
    }
}
