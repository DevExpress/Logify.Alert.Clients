#if DEBUG
using System;
using DevExpress.Logify.Core.Internal;
using DevExpress.Logify.Core.Internal.Tests;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Tests.Collectors {
    [TestFixture]
    public class ExceptionObjectCollectorTests : CollectorTestsBase {
        ExceptionObjectInfoCollector collector;
        LogifyCollectorContext context;

        const string excpetionsPrefix = @"""exception"": " + "[\r\n";
        const string excpetionsSuffix = "],\r\n";
        [SetUp]
        public void Setup() {
            SetupCore();
            context = new LogifyCollectorContext() { CallArgumentsMap = null };
            this.collector = new ExceptionObjectInfoCollector(context, new LegacyExceptionNormalizedStackCollector());
        }
        [TearDown]
        public void TearDown() {
            TearDownCore();
        }
        string GetExceptionJson(Exception ex, string stack = null, string normalizedStack = null, string additionalData = null) {
            return GetExceptionJson(ex.GetType().FullName, ex.Message, stack, normalizedStack, additionalData);
        }
        string GetExceptionJson(string exTypeName, string exMessage, string stack = null, string normalizedStack = null, string additionalData = null) {
            
            const string singleExceptionObjectFormat = "{{\r\n" +
    @"""type"":""{0}""," + "\r\n" +
    @"""message"":""{1}""," + "\r\n" +
    @"""stackTrace"":""{2}""," + "\r\n" +
    "{4}" +
    @"""normalizedStackTrace"":""{3}""," + "\r\n" +
    @"}}," + "\r\n";
            return String.Format(
                singleExceptionObjectFormat,
                exTypeName,
                exMessage,
                stack == null ? String.Empty : stack,
                normalizedStack == null ? String.Empty : normalizedStack,
                additionalData == null ? String.Empty : additionalData);
        }
        string ExceptionIdString(string value) {
            if (String.IsNullOrEmpty(value))
                return String.Empty;
            return String.Format(@"""groupId"":""{0}""," + "\r\n", value);
        }

        [Test]
        public void BeginWriteObject_SimpleException() {
            const string message = "TestMessage";
            collector.Process(new NullReferenceException(message), Logger);

            string expected = excpetionsPrefix + 
                GetExceptionJson(typeof(NullReferenceException).FullName, message) + 
                excpetionsSuffix;

            Assert.AreEqual(expected, Content);
        }

        [Test]
        public void BeginWriteObject_SimpleException_WithInner() {
            const string message = "TestMessage";
            var innerEx = new DivideByZeroException(message + "_Inner");
            var ex = new NullReferenceException(message, innerEx);
            collector.Process(ex, Logger);
            string expected = excpetionsPrefix;
            expected += GetExceptionJson(ex);
            expected += GetExceptionJson(innerEx.GetType().FullName, message + "_Inner");
            expected += excpetionsSuffix;

            Assert.AreEqual(expected, Content);
        }

        [Test]
        public void BeginWriteObject_AggregateException() {
            const string message = "TestMessage";
            var firstEx = new DivideByZeroException(message + "_First");
            var secondEx = new NullReferenceException(message + "_Second");
            var ex = new AggregateException(message, firstEx, secondEx);
            collector.Process(ex, Logger);
            string expected = excpetionsPrefix;
            expected += GetExceptionJson(ex.GetType().FullName, message, additionalData: ExceptionIdString("0"));
            expected += GetExceptionJson(firstEx.GetType().FullName, firstEx.Message, additionalData: ExceptionIdString("0"));
            expected += GetExceptionJson(secondEx.GetType().FullName, secondEx.Message, additionalData: ExceptionIdString("1"));
            expected += excpetionsSuffix;

            Assert.AreEqual(expected, Content);
        }
        [Test]
        public void BeginWriteObject_AggregateException_WithInners() {
            const string message = "TestMessage";
            var firstExInner = new DllNotFoundException(message + "_First_Inner");
            var firstEx = new DivideByZeroException(message + "_First", firstExInner);

            var secondEx = new NullReferenceException(message + "_Second");
            var ex = new AggregateException(message, firstEx, secondEx);
            collector.Process(ex, Logger);
            string expected = excpetionsPrefix;
            expected += GetExceptionJson(ex.GetType().FullName, message, additionalData: ExceptionIdString("0"));

            expected += GetExceptionJson(firstEx.GetType().FullName, firstEx.Message, additionalData: ExceptionIdString("0"));
            expected += GetExceptionJson(firstExInner.GetType().FullName, firstExInner.Message, additionalData: ExceptionIdString("0"));

            expected += GetExceptionJson(secondEx.GetType().FullName, secondEx.Message, additionalData: ExceptionIdString("1"));
            expected += excpetionsSuffix;

            Assert.AreEqual(expected, Content);
        }

        [Test]
        public void BeginWriteObject_AggregateException_WithAggregateInners() {
            const string message = "TestMessage";
            var firstExInner = new DllNotFoundException(message + "_First_Inner");
            var firstEx = new DivideByZeroException(message + "_First", firstExInner);

            var secondExInner1 = new NullReferenceException(message + "_Second");
            var secondExInner2 = new DataMisalignedException(message + "_Second");
            var secondEx = new AggregateException(message + "_Second", secondExInner1, secondExInner2);

            var ex = new AggregateException(message, firstEx, secondEx);
            collector.Process(ex, Logger);
            string expected = excpetionsPrefix;
            expected += GetExceptionJson(ex.GetType().FullName, message, additionalData: ExceptionIdString("0"));

            expected += GetExceptionJson(firstEx.GetType().FullName, firstEx.Message, additionalData: ExceptionIdString("0"));
            expected += GetExceptionJson(firstExInner.GetType().FullName, firstExInner.Message, additionalData: ExceptionIdString("0"));

            //expected += GetExceptionJson(secondEx.GetType().FullName, secondEx.Message, additionalData: @"""threadId"":""""," + "\r\n");
            expected += GetExceptionJson(secondExInner1.GetType().FullName, secondExInner1.Message, additionalData: ExceptionIdString("1"));
            expected += GetExceptionJson(secondExInner2.GetType().FullName, secondExInner2.Message, additionalData: ExceptionIdString("2"));

            expected += excpetionsSuffix;

            Assert.AreEqual(expected, Content);
        }
    }
}
#endif
