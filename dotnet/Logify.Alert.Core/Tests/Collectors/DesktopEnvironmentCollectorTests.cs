#if DEBUG
using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Internal.Tests {
    class DefaultTestClientConfiguration : ILogifyClientConfiguration {
        public bool CollectScreenshot { get; set; }
        public bool CollectMiniDump { get; set; }
        public bool CollectBreadcrumbs { get; set; }
        public int BreadcrumbsMaxCount { get; set; }

        public IgnorePropertiesInfoConfig IgnoreConfig { get; set; }
    }
    [TestFixture]
    public class DesktopEnvironmentCollectorTests : CollectorTestsBase {
        DesktopEnvironmentCollector collector;
        CultureInfo culture;
        CultureInfo uiCulture;
        [SetUp]
        public void Setup() {
            SetupCore();
            this.culture = CultureInfo.CurrentCulture;
            this.uiCulture = CultureInfo.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", false);
            this.collector = new DesktopEnvironmentCollector(new LogifyCollectorContext() { Config = new DefaultTestClientConfiguration() });
            Assert.AreEqual(10, this.collector.Collectors.Count);
            Assert.AreEqual(typeof(FrameworkVersionsCollector), this.collector.Collectors[8].GetType());
            this.collector.Collectors.RemoveAt(8);
            Assert.AreEqual(typeof(Win32GuiResourcesCollector), this.collector.Collectors[5].GetType());
            this.collector.Collectors.RemoveAt(5);
            Assert.AreEqual(typeof(DisplayCollector), this.collector.Collectors[4].GetType());
            this.collector.Collectors.RemoveAt(4);
            Assert.AreEqual(typeof(MemoryCollector), this.collector.Collectors[3].GetType());
            this.collector.Collectors.RemoveAt(3);
            Assert.AreEqual(typeof(DebuggerCollector), this.collector.Collectors[2].GetType());
            this.collector.Collectors.RemoveAt(2);
            Assert.AreEqual(typeof(VirtualMachineCollector), this.collector.Collectors[1].GetType());
            this.collector.Collectors.RemoveAt(1);
        }
        [TearDown]
        public void TearDown() {
            TearDownCore();
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = uiCulture;
            this.collector = null;
        }

        [Test]
        public void BeginWriteObject() {
            collector.Process(null, Logger);
            string expected = String.Format(
@"""os"": {{" + "\r\n" +
    @"""platform"":""{0}""," + "\r\n" +
    @"""servicePack"":""{1}""," + "\r\n" +
    @"""version"":""{2}""," + "\r\n" +
    @"""is64bit"":{3}," + "\r\n" +
@"}}," + "\r\n" +
@"""currentCulture"": {{" + "\r\n" +
    @"""name"":""en-US""," + "\r\n" +
    @"""englishName"":""English (United States)""," + "\r\n" +
    @"""displayName"":""English (United States)""," + "\r\n" +
    @"""isCultureCustomize"":false," + "\r\n" +
@"}}," + "\r\n" +
@"""currentUICulture"": {{" + "\r\n" +
    @"""name"":""en-US""," + "\r\n" +
    @"""englishName"":""English (United States)""," + "\r\n" +
    @"""displayName"":""English (United States)""," + "\r\n" +
    @"""isCultureCustomize"":false," + "\r\n" +
@"}}," + "\r\n",
 Environment.OSVersion.Platform,
 Environment.OSVersion.ServicePack,
 Environment.OSVersion.Version,
 Environment.Is64BitOperatingSystem.ToString().ToLowerInvariant()
);
            Assert.AreEqual(expected, Content);
        }
    }
}
#endif