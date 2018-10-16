#if DEBUG
using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Internal.Tests {
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
            Assert.AreEqual(string.Empty, content.ToString());
        }
        [Test]
        public void BeginWriteObjectNoName() {
            logger.BeginWriteObject(String.Empty);
            Assert.AreEqual(string.Empty, content.ToString());
        }
        [Test]
        public void EndWriteObject() {
            logger.EndWriteObject("testObject");
            Assert.AreEqual(string.Empty, content.ToString());
        }
        [Test]
        public void WriteValueString() {
            logger.BeginWriteObject(String.Empty);
            logger.WriteValue("variable", "value");
            logger.EndWriteObject(String.Empty);
            Assert.AreEqual("{\r\n\"variable\":\"value\"\r\n}\r\n", content.ToString());
        }
        [Test]
        public void WriteValueStringWithDotInName() {
            logger.BeginWriteObject(String.Empty);
            logger.WriteValue("devexpress.com", "value");
            logger.EndWriteObject(String.Empty);
            Assert.AreEqual("{\r\n\"devexpress\uff0Ecom\":\"value\"\r\n}\r\n", content.ToString());
        }
        [Test]
        public void WriteValueStringSpecialCharacters() {
            logger.BeginWriteObject(String.Empty);
            logger.WriteValue("variable", "special: \r\n\u0012\t\b\f\"\\;");
            logger.EndWriteObject(String.Empty);
            Assert.AreEqual("{\r\n" + @"""variable"":""special: \r\n\u0012\t\b\f\""\\;""" + "\r\n}\r\n", content.ToString());
        }
        [Test]
        public void WriteValueBool() {
            logger.BeginWriteObject(String.Empty);
            logger.WriteValue("variable", true);
            logger.EndWriteObject(String.Empty);
            Assert.AreEqual("{\r\n\"variable\":true\r\n}\r\n", content.ToString());
        }
        [Test]
        public void BeginWriteArray() {
            logger.BeginWriteArray("testArray");
            Assert.AreEqual(String.Empty, content.ToString());
        }
        [Test]
        public void BeginWriteArrayNoName() {
            logger.BeginWriteArray(String.Empty);
            Assert.AreEqual(String.Empty, content.ToString());
        }
        [Test]
        public void EndWriteArray() {
            logger.EndWriteArray("testArray");
            Assert.AreEqual(String.Empty, content.ToString());
        }

        [Test]
        public void WriteValueArray() {
            int[] testArray = {1, 2, 3};
            logger.BeginWriteObject(String.Empty);
            logger.WriteValue("testArray", testArray);
            logger.EndWriteObject(String.Empty);
            Assert.AreEqual("{\r\n\"testArray\":[\"1\",\"2\",\"3\"]\r\n}\r\n", content.ToString());
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