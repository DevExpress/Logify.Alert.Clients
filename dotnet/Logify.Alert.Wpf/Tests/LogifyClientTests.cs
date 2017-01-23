#if DEBUG
using System;
using DevExpress.Logify.WPF;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Tests {
    [TestFixture]
    public class LogifyClientTests {
        LogifyAlert client;

        [SetUp]
        public void Setup() {
            this.client = new LogifyAlert();
        }
        [TearDown]
        public void TearDown() {
            this.client = null;
        }

        [Test]
        public void Defaults() {
            Assert.AreEqual(null, client.ApiKey);
            Assert.AreEqual(null, client.AppName);
            Assert.AreEqual(null, client.AppVersion);
            Assert.AreEqual(false, client.ConfirmSendReport);
            Assert.AreEqual(null, client.MiniDumpServiceUrl);
            Assert.AreEqual("https://logify.devexpress.com/api/report/", client.ServiceUrl);
            Assert.AreEqual(null, client.UserId);
            Assert.AreEqual(true, client.CustomData != null);
            Assert.AreEqual(0, client.CustomData.Count);
            Assert.AreEqual("offline_reports", client.OfflineReportsDirectory);
            Assert.AreEqual(100, client.OfflineReportsCount);
            Assert.AreEqual(false, client.OfflineReportsEnabled);

            Predicate<IExceptionReportSender> predicate = (s) => {
                OfflineDirectoryExceptionReportSender sender = s as OfflineDirectoryExceptionReportSender;
                if (sender != null) {
                    Assert.AreEqual("offline_reports", sender.DirectoryName);
                    Assert.AreEqual(100, sender.ReportCount);
                    Assert.AreEqual(false, sender.IsEnabled);
                }
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);
        }

        [Test]
        public void ApiKey() {
            Assert.AreEqual(null, client.ApiKey);
            client.ApiKey = "<my-api-key>";
            Assert.AreEqual("<my-api-key>", client.ApiKey);
            Predicate<IExceptionReportSender> predicate = (s) => {
                Assert.AreEqual("<my-api-key>", s.ApiKey);
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);
        }
        [Test]
        public void ServiceUrl() {
            Assert.AreEqual("https://logify.devexpress.com/api/report/", client.ServiceUrl);
            client.ServiceUrl = "<my-service>";
            Assert.AreEqual("<my-service>", client.ServiceUrl);
            Predicate<IExceptionReportSender> predicate = (s) => {
                Assert.AreEqual("<my-service>", s.ServiceUrl);
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);
        }
        [Test]
        public void MiniDumpServiceUrl() {
            Assert.AreEqual(null, client.MiniDumpServiceUrl);
            client.MiniDumpServiceUrl = "<my-minidump-service>";
            Assert.AreEqual("<my-minidump-service>", client.MiniDumpServiceUrl);
            Predicate<IExceptionReportSender> predicate = (s) => {
                Assert.AreEqual("<my-minidump-service>", s.MiniDumpServiceUrl);
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);
        }
        [Test]
        public void ConfirmSendReport() {
            Assert.AreEqual(false, client.ConfirmSendReport);

            client.ConfirmSendReport = true;
            Assert.AreEqual(true, client.ConfirmSendReport);
            Predicate<IExceptionReportSender> predicate = (s) => {
                Assert.AreEqual(true, s.ConfirmSendReport);
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);

            client.ConfirmSendReport = false;
            Assert.AreEqual(false, client.ConfirmSendReport);
            predicate = (s) => {
                Assert.AreEqual(false, s.ConfirmSendReport);
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);
        }
        [Test]
        public void OfflineReportsDirectory() {
            Assert.AreEqual("offline_reports", client.OfflineReportsDirectory);

            client.OfflineReportsDirectory = "offline_reports2";
            Assert.AreEqual("offline_reports2", client.OfflineReportsDirectory);
            Predicate<IExceptionReportSender> predicate = (s) => {
                OfflineDirectoryExceptionReportSender sender = s as OfflineDirectoryExceptionReportSender;
                if (sender != null)
                    Assert.AreEqual("offline_reports2", sender.DirectoryName);
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);
        }
        [Test]
        public void OfflineReportsCount() {
            Assert.AreEqual(100, client.OfflineReportsCount);

            client.OfflineReportsCount = 20;
            Assert.AreEqual(20, client.OfflineReportsCount);
            Predicate<IExceptionReportSender> predicate = (s) => {
                OfflineDirectoryExceptionReportSender sender = s as OfflineDirectoryExceptionReportSender;
                if (sender != null)
                    Assert.AreEqual(20, sender.ReportCount);
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);
        }
        [Test]
        public void OfflineReportsEnabled() {
            Assert.AreEqual(false, client.OfflineReportsEnabled);

            client.OfflineReportsEnabled = true;
            Assert.AreEqual(true, client.OfflineReportsEnabled);
            Predicate<IExceptionReportSender> predicate = (s) => {
                OfflineDirectoryExceptionReportSender sender = s as OfflineDirectoryExceptionReportSender;
                if (sender != null)
                    Assert.AreEqual(true, sender.IsEnabled);
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);

            client.OfflineReportsEnabled = false;
            Assert.AreEqual(false, client.OfflineReportsEnabled);
            predicate = (s) => {
                OfflineDirectoryExceptionReportSender sender = s as OfflineDirectoryExceptionReportSender;
                if (sender != null)
                    Assert.AreEqual(false, sender.IsEnabled);
                return true;
            };
            CheckDefaultStructureAndPredicate(client, predicate);
        }

        static void CheckDefaultStructureAndPredicate(LogifyAlert client, Predicate<IExceptionReportSender> predicate) {
            IExceptionReportSender sender = ExceptionLoggerFactory.Instance.PlatformReportSender;
            Assert.AreEqual(true, sender != null);
            Assert.AreEqual(typeof(EmptyBackgroundExceptionReportSender), sender.GetType());
            CheckSenderConsistency(client, sender);
            predicate(sender);

            sender = ((EmptyBackgroundExceptionReportSender)sender).InnerSender;
            Assert.AreEqual(true, sender != null);
            Assert.AreEqual(typeof(CompositeExceptionReportSender), sender.GetType());
            CheckSenderConsistency(client, sender);
            predicate(sender);

            CompositeExceptionReportSender compositeSender = (CompositeExceptionReportSender)sender;
            CheckSenderConsistency(client, compositeSender);
            predicate(compositeSender);
            Assert.AreEqual(true, compositeSender.Senders != null);
            Assert.AreEqual(3, compositeSender.Senders.Count);

            sender = compositeSender.Senders[0];
            Assert.AreEqual(true, sender != null);
            Assert.AreEqual(typeof(ExternalProcessExceptionReportSender), sender.GetType());
            CheckSenderConsistency(client, sender);
            predicate(sender);

            sender = compositeSender.Senders[1];
            Assert.AreEqual(true, sender != null);
            Assert.AreEqual(typeof(WPFExceptionReportSender), sender.GetType());
            CheckSenderConsistency(client, sender);
            predicate(sender);

            sender = compositeSender.Senders[2];
            Assert.AreEqual(true, sender != null);
            Assert.AreEqual(typeof(OfflineDirectoryExceptionReportSender), sender.GetType());
            CheckSenderConsistency(client, sender);
            predicate(sender);
        }

        static void CheckSenderConsistency(LogifyClientBase client, IExceptionReportSender sender) {
            Assert.AreEqual(client.MiniDumpServiceUrl, sender.MiniDumpServiceUrl);
            Assert.AreEqual(client.ServiceUrl, sender.ServiceUrl);
            Assert.AreEqual(client.ConfirmSendReport, sender.ConfirmSendReport);
            Assert.AreEqual(client.ApiKey, sender.ApiKey);
        }
    }
}
#endif