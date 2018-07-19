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
            this.collector = new ExceptionObjectInfoCollector(context);
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
    @"""normalizedStackTrace"":""{3}""," + "\r\n" +
    "{4}" +
    @"}}," + "\r\n";
            return String.Format(
                singleExceptionObjectFormat,
                exTypeName,
                exMessage,
                stack == null ? String.Empty : stack,
                normalizedStack == null ? String.Empty : normalizedStack,
                additionalData == null ? String.Empty : additionalData);
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
            expected += GetExceptionJson(ex.GetType().FullName, message, additionalData: @"""threadId"":""""," + "\r\n" );
            expected += GetExceptionJson(firstEx.GetType().FullName, firstEx.Message);
            expected += GetExceptionJson(secondEx.GetType().FullName, secondEx.Message);
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
            expected += GetExceptionJson(ex.GetType().FullName, message, additionalData: @"""threadId"":""""," + "\r\n");

            expected += GetExceptionJson(firstEx.GetType().FullName, firstEx.Message);
            expected += GetExceptionJson(firstExInner.GetType().FullName, firstExInner.Message);

            expected += GetExceptionJson(secondEx.GetType().FullName, secondEx.Message);
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
            expected += GetExceptionJson(ex.GetType().FullName, message, additionalData: @"""threadId"":""""," + "\r\n");

            expected += GetExceptionJson(firstEx.GetType().FullName, firstEx.Message);
            expected += GetExceptionJson(firstExInner.GetType().FullName, firstExInner.Message);

            //expected += GetExceptionJson(secondEx.GetType().FullName, secondEx.Message, additionalData: @"""threadId"":""""," + "\r\n");
            expected += GetExceptionJson(secondExInner1.GetType().FullName, secondExInner1.Message);
            expected += GetExceptionJson(secondExInner2.GetType().FullName, secondExInner2.Message);

            expected += excpetionsSuffix;

            Assert.AreEqual(expected, Content);
        }
    }
}
#endif
