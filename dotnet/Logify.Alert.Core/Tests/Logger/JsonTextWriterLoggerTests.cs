#if DEBUG
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Tests {
    [TestFixture]
    public class JsonTextWriterLoggerTests {
        TextWriter writer;
        StringBuilder content;
        ILogger logger;

        [SetUp]
        public void Setup() {
            this.content = new StringBuilder();
            this.writer = new StringWriter(content);
            this.logger = new TextWriterLogger(writer);
        }
        [TearDown]
        public void TearDown() {
            this.content = null;
            this.writer.Dispose();
            this.logger = null;
        }
        [Test]
        public void BeginWriteObject() {
            logger.BeginWriteObject("testObject");
            Assert.AreEqual("\"testObject\": {\r\n", content.ToString());
        }
        [Test]
        public void BeginWriteObjectNoName() {
            logger.BeginWriteObject(String.Empty);
            Assert.AreEqual("{\r\n", content.ToString());
        }
        [Test]
        public void EndWriteObject() {
            logger.EndWriteObject("testObject");
            Assert.AreEqual("},\r\n", content.ToString());
        }
        [Test]
        public void WriteValueString() {
            logger.WriteValue("variable", "value");
            Assert.AreEqual("\"variable\":\"value\",\r\n", content.ToString());
        }
        [Test]
        public void WriteValueStringWithDotInName() {
            logger.WriteValue("devexpress.com", "value");
            Assert.AreEqual("\"devexpress\uff0Ecom\":\"value\",\r\n", content.ToString());
        }
        [Test]
        public void WriteValueStringSpecialCharacters() {
            logger.WriteValue("variable", "special: \r\n\u0012\t\b\f\"\\;");
            Assert.AreEqual(@"""variable"":""special: \r\n\u0012\t\b\f\""\\;""," + "\r\n", content.ToString());
        }
        [Test]
        public void WriteValueBool() {
            logger.WriteValue("variable", true);
            Assert.AreEqual("\"variable\":true,\r\n", content.ToString());
        }
        [Test]
        public void BeginWriteArray() {
            logger.BeginWriteArray("testArray");
            Assert.AreEqual("\"testArray\": [\r\n", content.ToString());
        }
        [Test]
        public void BeginWriteArrayNoName() {
            logger.BeginWriteArray(String.Empty);
            Assert.AreEqual("[\r\n", content.ToString());
        }
        [Test]
        public void EndWriteArray() {
            logger.EndWriteArray("testArray");
            Assert.AreEqual("],\r\n", content.ToString());
        }

        [Test]
        public void WriteValueArray() {
            int[] testArray = {1, 2, 3};
            logger.WriteValue("testArray", testArray);
            Assert.AreEqual("\"testArray\": [\r\n\"1\",\r\n\"2\",\r\n\"3\"\r\n],\r\n", content.ToString());
        }

        [Test]
        public void WriteValueEmptyArray() {
            int[] testArray = {};
            logger.WriteValue("testArray", testArray);
            Assert.AreEqual("", content.ToString());
        }
    }
}
#endif