#if DEBUG
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;
using System.Collections.Generic;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Core.Internal.Tests {
    [TestFixture]
    public class BreadcrumbsCollectorTests : CollectorTestsBase {
        BreadcrumbsCollector collector;
        BreadcrumbCollection breadcrumbs;
        [SetUp]
        public void Setup() {
            this.breadcrumbs = new BreadcrumbCollection();
            this.collector = new BreadcrumbsCollector(breadcrumbs);
            SetupCore();

        }
        [TearDown]
        public void TearDown() {
            TearDownCore();
        }
        [Test]
        public void Default() {
            collector.Process(null, Logger);
            string expected = "";
            Assert.AreEqual(expected, Content);
        }
        [Test]
        public void SimpleEmptyBreadcrumb() {
            breadcrumbs.AddSimple(new Breadcrumb() {
                DateTime = new DateTime(2017, 9, 1, 20, 18, 36, DateTimeKind.Utc),
            });
            this.collector = new BreadcrumbsCollector(breadcrumbs);
            collector.Process(null, Logger);
            string expected = @"""breadcrumbs"": [
{
""dateTime"":""2017-09-01T20:18:36.0000000Z"",
},
],
";
            Assert.AreEqual(expected, Content);
        }
        [Test]
        public void SimpleFullBreadcrumb() {
            var customFields = new Dictionary<string, string>();
            customFields.Add("my_custom_field", "testvalue");
            breadcrumbs.AddSimple(new Breadcrumb() {
                DateTime = new DateTime(2017, 9, 1, 20, 18, 36, DateTimeKind.Utc),
                Event = BreadcrumbEvent.MouseClick,
                Level = BreadcrumbLevel.Info,
                Category = "test",
                ClassName = "BreadcrumbsCollectorTests",
                MethodName = "SimpleBreadcrumb",
                Message = "simple test breadcrumb",
                Line = 15,
                ThreadId = "789",
                CustomData = customFields
            });
            this.collector = new BreadcrumbsCollector(breadcrumbs);
            collector.Process(null, Logger);
            string expected = @"""breadcrumbs"": [
{
""dateTime"":""2017-09-01T20:18:36.0000000Z"",
""level"":""Info"",
""event"":""mouseClick"",
""category"":""test"",
""message"":""simple test breadcrumb"",
""className"":""BreadcrumbsCollectorTests"",
""methodName"":""SimpleBreadcrumb"",
""line"":15,
""threadId"":""789"",
""customData"": {
""my_custom_field"":""testvalue"",
},
},
],
";
            Assert.AreEqual(expected, Content);
        }
        [Test]
        public void SeveralBreadcrumbs_BackwardOrder() {
            breadcrumbs.AddSimple(new Breadcrumb() {
                DateTime = new DateTime(2017, 9, 1, 20, 18, 36, DateTimeKind.Utc),
            });
            breadcrumbs.AddSimple(new Breadcrumb() {
                DateTime = new DateTime(2018, 9, 1, 20, 18, 36, DateTimeKind.Utc),
            });
            this.collector = new BreadcrumbsCollector(breadcrumbs);
            collector.Process(null, Logger);
            string expected = @"""breadcrumbs"": [
{
""dateTime"":""2018-09-01T20:18:36.0000000Z"",
},
{
""dateTime"":""2017-09-01T20:18:36.0000000Z"",
},
],
";
            Assert.AreEqual(expected, Content);
        }
    }
}
#endif