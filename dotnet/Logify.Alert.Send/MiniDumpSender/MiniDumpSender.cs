using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Timers;

namespace DevExpress.Logify.Send {
    public class MiniDumpSender {
        public MiniDumpSender(string serviceUrl) {
            ServiceUrl = new Uri(serviceUrl);
        }

        Uri ServiceUrl { get; set; }
        TimeSpan MinutesToEnd = TimeSpan.FromMinutes(5);
        TimeSpan IntervalSeconds = TimeSpan.FromSeconds(30);
        DateTime EndTime;
        string ApiKey;
        string MiniDumpGuid;
        string DumpFileName;
        Timer Timer;

        System.Threading.CancellationTokenSource TokenSource = new System.Threading.CancellationTokenSource();
        System.Threading.CancellationToken Token {
            get {
                return TokenSource.Token;
            }
        }

        void TimeElapsed(object sender, ElapsedEventArgs e) {
            string reportId = CheckReport();
            if (!string.IsNullOrEmpty(reportId)) {
                Console.WriteLine("Checking report success...");
                UploadReport(reportId);
            }
                
        }

        public void UploadReport(string reportId) {
            Console.WriteLine("Uploading dump...");
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = ServiceUrl;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", ApiKey);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format("api/dump/uploadminidump?reportId={0}", reportId));
                request.Headers.ExpectContinue = false;
                MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----boundary");
                ByteArrayContent byteArrayContent = new ByteArrayContent(File.ReadAllBytes(DumpFileName));
                byteArrayContent.Headers.Add("Content-Type", "application/vnd.tcpdump.pcap");
                multiPartContent.Add(byteArrayContent, "file", MiniDumpGuid + ".dmp");
                request.Content = multiPartContent;
                HttpResponseMessage httpResponse = client.SendAsync(request, HttpCompletionOption.ResponseContentRead, System.Threading.CancellationToken.None).Result;

                TokenSource.Cancel();
            }
        }

        string CheckReport() {
            Console.WriteLine("Checking report...");
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = ServiceUrl;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", ApiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("api/dump/checkduplicate?dumpId={0}", MiniDumpGuid));
                HttpResponseMessage message = client.SendAsync(request).Result;
                if (message.StatusCode == System.Net.HttpStatusCode.Conflict
                    || message.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    || DateTime.Now.CompareTo(EndTime) >= 0
                    ) {
                    Timer.Stop();
                    TokenSource.Cancel();
                } else if (message.StatusCode == System.Net.HttpStatusCode.OK)
                    return message.Content.ReadAsStringAsync().Result.Trim('"');
            }
            return string.Empty;
        }

        public void UploadWithConfirmation(string apiKey, string miniDumpGuid, string fileName) {
            ApiKey = apiKey;
            MiniDumpGuid = miniDumpGuid;
            DumpFileName = fileName;
            EndTime = DateTime.Now.Add(MinutesToEnd);
            Timer = new Timer(IntervalSeconds.TotalMilliseconds);

            Timer.Elapsed += new ElapsedEventHandler(TimeElapsed);
            Timer.Enabled = true;
            Timer.Start();

            Console.WriteLine("Wait answer from dump recieve service...");
            Token.WaitHandle.WaitOne(MinutesToEnd);
        }
    }
}
