#if DEBUG
using System;
using DevExpress.Logify.Win;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Tests {
    [TestFixture]
    public class DxLogifyClientTests {
        DxLogifyClient client;

        [SetUp]
        public void Setup() {
            this.client = new DxLogifyClient();
        }
        [TearDown]
        public void TearDown() {
            this.client = null;
        }
        [Test]
        public void ReportToDevExpressFileNameSpecified() {
            client.ReportToDevExpress("myLog", "exception.log", GetType().Assembly);

            Assert.AreEqual("12345678FEE1DEADBEEF4B1DBABEFACE", client.ApiKey);
            Assert.AreEqual("https://logify.devexpress.com/api/report/", client.ServiceUrl);
            CheckReportToDevExpressFileNameSpecified(ExceptionLoggerFactory.Instance.PlatformReportSender);
            CheckReportToDevExpressFileNameSpecified(ExceptionLoggerFactory.Instance.PlatformReportSender.Clone());
        }
        void CheckReportToDevExpressFileNameSpecified(IExceptionReportSender rootSender) {
            const string apiKey = "12345678FEE1DEADBEEF4B1DBABEFACE";
            const string serviceUrl = "https://logify.devexpress.com/api/report/";
            EmptyBackgroundExceptionReportSender root = rootSender as EmptyBackgroundExceptionReportSender;
            Assert.AreEqual(true, root != null);
            Assert.AreEqual(apiKey, root.ApiKey);
            Assert.AreEqual(serviceUrl, root.ServiceUrl);

            CompositeExceptionReportSender composite = root.InnerSender as CompositeExceptionReportSender;
            Assert.AreEqual(true, composite != null);
            Assert.AreEqual(apiKey, composite.ApiKey);
            Assert.AreEqual(serviceUrl, composite.ServiceUrl);

            ExternalProcessExceptionReportSender externalSender = composite.Senders[0] as ExternalProcessExceptionReportSender;
            Assert.AreEqual(true, externalSender != null);
            Assert.AreEqual(apiKey, externalSender.ApiKey);
            Assert.AreEqual(serviceUrl, externalSender.ServiceUrl);

            WinFormsExceptionReportSender winFormsSender = composite.Senders[1] as WinFormsExceptionReportSender;
            Assert.AreEqual(true, winFormsSender != null);
            Assert.AreEqual(apiKey, winFormsSender.ApiKey);
            Assert.AreEqual(serviceUrl, winFormsSender.ServiceUrl);

            //FileExceptionReportSender file = composite.Senders[2] as FileExceptionReportSender;
            //Assert.AreEqual(true, file != null);
            //Assert.AreEqual(apiKey, file.ApiKey);
            //Assert.AreEqual(serviceUrl, file.ServiceUrl);
            //Assert.AreEqual("exception.log", file.FileName);
        }
    }
}
#endif