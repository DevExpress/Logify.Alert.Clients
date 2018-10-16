#if DEBUG
using System;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Internal.Tests {
    [TestFixture]
    public class OperatingSystemCollectorTests : CollectorTestsBase {
        OperatingSystemCollector collector;
        [SetUp]
        public void Setup() {
            SetupCore();
            this.collector = new OperatingSystemCollector();
        }
        [TearDown]
        public void TearDown() {
            TearDownCore();
        }

        [Test]
        public void BeginWriteObject() {
            collector.Process(null, Logger);
            string expected = String.Format(
@"""os"":{{" + "\r\n" +
@"""platform"":""{0}""," + "\r\n" +
@"""servicePack"":""{1}""," + "\r\n" +
@"""version"":""{2}""," + "\r\n" +
@"""is64bit"":{3}" + "\r\n" +
@"}}" + "\r\n",
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