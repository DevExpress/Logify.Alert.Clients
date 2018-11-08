#if DEBUG
using System;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Internal.Tests {
    [TestFixture]
    public class NormalizedStackCollectorTests : CollectorTestsBase {
        IStackTraceNormalizer normalizer;
        IStackTraceNormalizer legacyNormalizer;
        [SetUp]
        public void Setup() {
            SetupCore();
            legacyNormalizer = new LegacyExceptionNormalizedStackCollector();
            normalizer = new RegexpExceptionNormalizedStackCollector();
        }
        [TearDown]
        public void TearDown() {
            TearDownCore();
        }

        [Test]
        public void SingleLine() {
            string stack = "   at Test.MyApp.Program.Main() in E:\\E\\Test.MyApp\\Test.MyApp\\Test.MyApp\\Program.cs:line 31";
            string expected = legacyNormalizer.NormalizeStackTrace(stack);//"Test.MyApp.Program.Main()\r\n";
            string normalized = normalizer.NormalizeStackTrace(stack);

            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void WithoutR() {
            string stack = "   at System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath) in C:\\Projects\\Sandbox\\UserSettings.cs:line 62\n" +
                           "   at Test.MyApp.Program.Main() in E:\\E\\Test.MyApp\\Test.MyApp\\Test.MyApp\\Program.cs:line 31";

            //string expected = "System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)\n" +
            //                  "   at Test.MyApp.Program.Main()\r\n";
            string expected = legacyNormalizer.NormalizeStackTrace(stack);

            string normalized = normalizer.NormalizeStackTrace(stack);

            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void MultiLine() {
            string stack = "   at System.Xml.Serialization.XmlSerializer.Deserialize(XmlReader xmlReader, String encodingStyle, XmlDeserializationEvents events) in C:\\Projects\\Sandbox\\UserSettings.cs:line 62\r\n" +
                "   at System.Xml.Serialization.XmlSerializer.Deserialize(Stream stream) in C:\\Projects\\Sandbox\\UserSettings.cs:line 62\r\n" +
                "   at MyApp.UserSettings.Load() in C:\\Projects\\Sandbox\\UserSettings.cs:line 62\r\n";

            //string expected = "System.Xml.Serialization.XmlSerializer.Deserialize(XmlReader xmlReader, String encodingStyle, XmlDeserializationEvents events)\r\n" +
            //    "System.Xml.Serialization.XmlSerializer.Deserialize(Stream stream)\r\n" +
            //    "MyApp.UserSettings.Load()\r\n";
            string expected = legacyNormalizer.NormalizeStackTrace(stack);

            string normalized = normalizer.NormalizeStackTrace(stack);

            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void WithoutIN() {
            string stack = "   at System.Xml.Serialization.XmlSerializer.Deserialize(XmlReader xmlReader, String encodingStyle, XmlDeserializationEvents events)\r\n" +
                "   at System.Xml.Serialization.XmlSerializer.Deserialize(Stream stream)\r\n";

            string expected = legacyNormalizer.NormalizeStackTrace(stack);

            string normalized = normalizer.NormalizeStackTrace(stack);

            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void WithAddressesIOS() {
            //Real iOS stacks contains \n in the end because of Environment.NewLine == "\n"
            //Here is used Win-style line breaks in test purposes

            string stack = "  at Test.MainPage.Handle_Clicked (System.Object sender, System.EventArgs e) [0x00001] in /Projects/Test/MainPage.xaml.cs:18 \r\n" +
                "  at Xamarin.Forms.Button.SendClicked () [0x0001f] in D:\\a\\1\\s\\Xamarin.Forms.Core\\Button.cs:133 \r\n" +
                "  at Xamarin.Forms.Platform.iOS.ButtonRenderer.OnButtonTouchUpInside (System.Object sender, System.EventArgs eventArgs) [0x0001b] in <2a59efab866341818ab4748ebe270f0a>:0";
            string expected = "Test.MainPage.Handle_Clicked (System.Object sender, System.EventArgs e)\r\n" +
                "Xamarin.Forms.Button.SendClicked ()\r\n" +
                "Xamarin.Forms.Platform.iOS.ButtonRenderer.OnButtonTouchUpInside (System.Object sender, System.EventArgs eventArgs)\r\n";

            string normalized = normalizer.NormalizeStackTrace(stack);

            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void WithAngleBrackets() {
            string stack = "   at BPanelCollector.CreateAssemblies.<>c.<GetLines>b__55_1(Instance x)\r\n" +
                "   at System.Linq.Enumerable.<OfTypeIterator>d__92`1.MoveNext()";

            string expected = legacyNormalizer.NormalizeStackTrace(stack);

            string normalized = normalizer.NormalizeStackTrace(stack);

            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void WithAngleBracketsAndAddress() {
            string stack = "   at BPanelCollector.CreateAssemblies.<>c.<GetLines>b__55_1(Instance x) [0x00001]\r\n" +
                "   at System.Linq.Enumerable.<OfTypeIterator>d__92`1.MoveNext() [0x0001f]";

            string expected = "BPanelCollector.CreateAssemblies.<>c.<GetLines>b__55_1(Instance x)\r\n" +
                "System.Linq.Enumerable.<OfTypeIterator>d__92`1.MoveNext()\r\n";

            string normalized = normalizer.NormalizeStackTrace(stack);

            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void WithSquareBrackets() {
            string stack = "   at System.Linq.Enumerable.ToList[TSource](IEnumerable`1 source) [0x00001]";

            string expected = "System.Linq.Enumerable.ToList[TSource](IEnumerable`1 source)\r\n";

            string normalized = normalizer.NormalizeStackTrace(stack);

            Assert.AreEqual(expected, normalized);
        }

        [Test]
        public void WithSquareBracketsAndAddress() {
            string stack = "   at System.Linq.Enumerable.ToList[TSource](IEnumerable`1 source) [0x00001] in /Projects/Test/MainPage.xaml.cs:18";

            string expected = "System.Linq.Enumerable.ToList[TSource](IEnumerable`1 source)\r\n";

            string normalized = normalizer.NormalizeStackTrace(stack);

            Assert.AreEqual(expected, normalized);
        }
    }
}
#endif