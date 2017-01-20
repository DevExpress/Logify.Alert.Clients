using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Logify.Core;

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
    }
}
