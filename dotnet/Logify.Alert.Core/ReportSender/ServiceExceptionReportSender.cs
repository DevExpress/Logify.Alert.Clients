using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Logify.Core;
using System.Threading;
using System.ComponentModel;
using DevExpress.Logify.Core.Internal;
using System.Diagnostics;

#if NETSTANDARD
using System.Net.Http;
using System.Net.Http.Headers;
#endif

namespace DevExpress.Logify.Core {
    public class BackgroundSendModel {
        public Func<bool> SendAction { get; set; }
        //public LogifyClientExceptionReport Report { get; set; }
        public Thread Thread { get; set; }
        public bool SendComplete { get; set; }
        public bool SendResult { get; set; }
    }
}

namespace DevExpress.Logify.Core.Internal {
    public class BackgroundSendModelAccessor {
        public static BackgroundSendModel SendReportInBackgroundThread(Func<bool> sendAction) {
            FixWebRequestDeadlock();
            Thread thread = new Thread(BackgroundSend);
            BackgroundSendModel sendModel = new BackgroundSendModel();
            sendModel.SendAction = sendAction;
            sendModel.Thread = thread;
            thread.Start(sendModel);
            return sendModel;
        }

        internal static void FixWebRequestDeadlock() {
            WebRequest.Create("http://logify.devexpress.com/"); //T605959: deadlock in System.Net.Logging read configuration
        }

        static void BackgroundSend(object obj) {
            BackgroundSendModel model = obj as BackgroundSendModel;
            if (model == null)
                return;

            try {
                if (model.SendAction == null) {
                    model.SendResult = false;
                    //model.SendComplete = true;
                    return;
                }

                model.SendResult = model.SendAction();
                //model.SendComplete = true;
            }
            finally {
                model.SendComplete = true;
            }
        }
    }
    public abstract class ServiceExceptionReportSender : ExceptionReportSenderSkeleton, IRemoteConfigurationProvider {
        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            return SendViaHttpWebRequest(report);
        }
#if ALLOW_ASYNC
        protected override async Task<bool> SendExceptionReportCoreAsync(LogifyClientExceptionReport report) {
            return await SendViaHttpWebRequestAsync(report);
        }
#endif
        static string CreateEndPointUrl(string serviceUrl, string queryString) {
            string url = serviceUrl;
            if (!string.IsNullOrEmpty(url)) {
                if (url[url.Length - 1] != '/')
                    url += '/';
                url += queryString;
            }
            return url;
        }

        WebRequest CreateAndSetupHttpWebRequest(LogifyClientExceptionReport report) {
            Uri serUri = new Uri(CreateEndPointUrl(ServiceUrl, "api/report/newreport"), UriKind.Absolute);    
            WebRequest request = WebRequest.Create(serUri);
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
            IWebProxy proxy = null;
            if (this.Proxy != null)
                proxy = this.Proxy;
            if (this.ProxyCredentials != null)
                proxy = SetupProxyByCredentials();
            if (proxy != null) { 
                request.Proxy = proxy;
            }
        }
        IWebProxy SetupProxyByCredentials() {
            if (WebRequest.DefaultWebProxy != null) {
                WebProxy proxy = GetProxy();
                if (proxy != null) {
                    proxy.UseDefaultCredentials = false;
                    if (this.ProxyCredentials != null) {
                        proxy.Credentials = ProxyCredentials;
                    }
                    return proxy;
                }
            }
            return null;
        }
        WebProxy GetProxy() {
#if NETSTANDARD
            return new WebProxy();
#else
            Uri proxyUri = WebRequest.DefaultWebProxy.GetProxy(new Uri(ServiceUrl));
            if (proxyUri == null)
                return null;
            return new WebProxy(proxyUri, false);
#endif
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
        public LogifyAlertRemoteConfiguration GetConfiguration(string serviceUrl, string apiKey) {
            try {
                string json = GetConfigurationJson(serviceUrl, apiKey);
                if (String.IsNullOrEmpty(json))
                    return null;
                return JsonLite.DeserializeObject<LogifyAlertRemoteConfiguration>(json);
            }
            catch {
                return null;
            }
        }
        string GetConfigurationJson(string serviceUrl, string apiKey) {
            try {
                string instanceId = HardwareId.Get();
                WebRequest request = WebRequest.Create(CreateEndPointUrl(serviceUrl, String.Format("api/config/get?instanceId={0}", instanceId)));
                SetupProxy(request);
                request.Method = "GET";
                request.Headers.Add("Authorization", "amx " + apiKey);
                //request.ContentType = "application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                    return String.Empty;
                using (Stream stream = response.GetResponseStream()) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch {
                return String.Empty;
            }
        }
    }


    public abstract class ServiceWithConfirmationExceptionReportSender : ServiceExceptionReportSender {
        static bool isFormShown;
        public override bool SendExceptionReport(LogifyClientExceptionReport report) {
            if (ConfirmSendReport && !isFormShown && LogifyClientBase.Instance != null) {
                try {
                    ReportConfirmationModel model = LogifyClientAccessor.CreateConfirmationModel(report, (r) => { return base.SendExceptionReport(r); });
                    if (model == null)
                        return false;
                    isFormShown = true;
                    if (ShowCustomConfirmSendForm(model))
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
            if ((LogifyClientBase.Instance != null) && LogifyClientAccessor.RaiseConfirmationDialogShowing(model)) {
                return true;
            }
            return false;
        }
        protected abstract bool ShowBuiltInConfirmSendForm(ReportConfirmationModel model);
    }
}
