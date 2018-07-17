#if DEBUG
//using Newtonsoft.Json;
using NUnit.Framework;
using System;
using JsonSerializationException = DevExpress.Logify.Core.Internal.JsonLiteDeserializationException;

namespace DevExpress.Logify.Core.Internal.Tests {
    public class SimpleTestObject {
        public string StringProperty { get; set; }
        public object ObjectProperty { get; set; }
        public int IntProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public float FloatProperty { get; set; }
        public double DoubleProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public TimeSpan TimeSpanProperty { get; set; }
        public bool BoolProperty { get; set; }
    }
    [TestFixture]
    public class JsonLiteDeserializeTests {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueString() {
            Assert.AreEqual(null, DeserializeObject<string>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueObject() {
            Assert.AreEqual(null, DeserializeObject<object>(null));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueIntNullable() {
            Assert.AreEqual(null, DeserializeObject<int?>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueDecimalNullable() {
            Assert.AreEqual(null, DeserializeObject<decimal?>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueFloatNullable() {
            Assert.AreEqual(null, DeserializeObject<float?>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueDoubleNullable() {
            Assert.AreEqual(null, DeserializeObject<double?>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueDateTimeNullable() {
            Assert.AreEqual(null, DeserializeObject<DateTime?>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueTimeSpanNullable() {
            Assert.AreEqual(null, DeserializeObject<TimeSpan?>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueBoolNullable() {
            Assert.AreEqual(null, DeserializeObject<bool?>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueInt() {
            Assert.AreEqual(0, DeserializeObject<int>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueDecimal() {
            Assert.AreEqual(0m, DeserializeObject<decimal>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueFloat() {
            Assert.AreEqual(0f, DeserializeObject<float>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueDouble() {
            Assert.AreEqual(0d, DeserializeObject<double>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueDateTime() {
            Assert.AreEqual(DateTime.MinValue, DeserializeObject<DateTime>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueTimeSpan() {
            Assert.AreEqual(TimeSpan.MinValue, DeserializeObject<TimeSpan>(null));
        }
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void DefaultValueBool() {
            Assert.AreEqual(TimeSpan.MinValue, DeserializeObject<bool>(null));
        }
        [Test]
        public void EmptyValueNullable() {
            Assert.AreEqual(null, DeserializeObject<string>(String.Empty));
            Assert.AreEqual(null, DeserializeObject<object>(String.Empty));
            Assert.AreEqual(null, DeserializeObject<int?>(String.Empty));
            Assert.AreEqual(null, DeserializeObject<decimal?>(String.Empty));
            Assert.AreEqual(null, DeserializeObject<float?>(String.Empty));
            Assert.AreEqual(null, DeserializeObject<double?>(String.Empty));
            Assert.AreEqual(null, DeserializeObject<DateTime?>(String.Empty));
            Assert.AreEqual(null, DeserializeObject<TimeSpan?>(String.Empty));
            Assert.AreEqual(null, DeserializeObject<bool?>(String.Empty));
        }
        [Test, ExpectedException(typeof(JsonSerializationException))]
        public void EmptyValueInt() {
            Assert.AreEqual(0, DeserializeObject<int>(String.Empty));
        }
        [Test, ExpectedException(typeof(JsonSerializationException))]
        public void EmptyValueDecimal() {
            Assert.AreEqual(0m, DeserializeObject<decimal>(String.Empty));
        }
        [Test, ExpectedException(typeof(JsonSerializationException))]
        public void EmptyValueFloat() {
            Assert.AreEqual(0f, DeserializeObject<float>(String.Empty));
        }
        [Test, ExpectedException(typeof(JsonSerializationException))]
        public void EmptyValueDouble() {
            Assert.AreEqual(0d, DeserializeObject<double>(String.Empty));
        }
        [Test, ExpectedException(typeof(JsonSerializationException))]
        public void EmptyValueDateTime() {
            Assert.AreEqual(DateTime.MinValue, DeserializeObject<DateTime>(String.Empty));
        }
        [Test, ExpectedException(typeof(JsonSerializationException))]
        public void EmptyValueTimeSpan() {
            Assert.AreEqual(TimeSpan.MinValue, DeserializeObject<TimeSpan>(String.Empty));
        }
        [Test, ExpectedException(typeof(JsonSerializationException))]
        public void EmptyValueBool() {
            Assert.AreEqual(false, DeserializeObject<bool>(String.Empty));
        }
        /*
        [Test]
        public void CorrectSimpleValue() {
            Assert.AreEqual("test", DeserializeObject<string>("\"test\""));
            Assert.AreEqual(true, DeserializeObject<object>("{}") != null);

            Assert.AreEqual(1, DeserializeObject<int?>("1"));
            Assert.AreEqual(1.3m, DeserializeObject<decimal?>("1.3"));
            Assert.AreEqual(1.4f, DeserializeObject<float?>("1.4"));
            Assert.AreEqual(1.5d, DeserializeObject<double?>("1.5"));
            Assert.AreEqual(new DateTime(2017, 9, 30, 13, 20, 06, 502), DeserializeObject<DateTime?>("\"2017-09-30T13:20:06.502+03:00\""));
            Assert.AreEqual(TimeSpan.FromSeconds(12345), DeserializeObject<TimeSpan?>("\"03:25:45\""));
            Assert.AreEqual(true, DeserializeObject<bool?>("true"));

            Assert.AreEqual(1, DeserializeObject<int>("1"));
            Assert.AreEqual(1.3m, DeserializeObject<decimal>("1.3"));
            Assert.AreEqual(1.4f, DeserializeObject<float>("1.4"));
            Assert.AreEqual(1.5d, DeserializeObject<double>("1.5"));
            Assert.AreEqual(new DateTime(2017, 9, 30, 13, 20, 06, 502), DeserializeObject<DateTime>("\"2017-09-30T13:20:06.502+03:00\""));
            Assert.AreEqual(TimeSpan.FromSeconds(12345), DeserializeObject<TimeSpan>("\"03:25:45\""));
            Assert.AreEqual(true, DeserializeObject<bool>("true"));
        }*/
        [Test]
        public void SimpleObject1() {
            SimpleTestObject instance = DeserializeObject<SimpleTestObject>("{}");
            Assert.AreEqual(true, instance != null);

            //instance = DeserializeObject<SimpleTestObject>("{ \"stringproperty\": \"test\", \"objectProperty\": {}, \"IntProperty\": 2, \"decimalProperty\": \"1.3\", \"floatProperty\": \"1.4\", \"doubleProperty\": \"1.5\", \"dateTimeProperty\": \"2017-09-30T13:20:06.502+03:00\", \"timeSpanProperty\": \"03:25:45\", \"boolProperty\": true }");
            instance = DeserializeObject<SimpleTestObject>("{ \"stringproperty\": \"test\", \"objectProperty\": {}, \"IntProperty\": \"2\", \"decimalProperty\": \"1.3\", \"floatProperty\": \"1.4\", \"doubleProperty\": \"1.5\", \"dateTimeProperty\": \"2017-09-30T13:20:06.502+03:00\", \"timeSpanProperty\": \"03:25:45\", \"boolProperty\": \"true\" }");
            Assert.AreEqual(true, instance != null);
            Assert.AreEqual("test", instance.StringProperty);
            Assert.AreEqual(true, instance.ObjectProperty != null);
            Assert.AreEqual(2, instance.IntProperty);
            Assert.AreEqual(1.3m, instance.DecimalProperty);
            Assert.AreEqual(1.4f, instance.FloatProperty);
            Assert.AreEqual(1.5d, instance.DoubleProperty);
            Assert.AreEqual(new DateTime(2017, 9, 30, 13, 20, 06, 502), instance.DateTimeProperty);
            Assert.AreEqual(TimeSpan.FromSeconds(12345), instance.TimeSpanProperty);
            Assert.AreEqual(true, instance.BoolProperty);
        }
        [Test]
        public void SimpleObject2() {
            SimpleTestObject instance = DeserializeObject<SimpleTestObject>("{}");
            Assert.AreEqual(true, instance != null);

            instance = DeserializeObject<SimpleTestObject>("{ \"stringproperty\": \"test\", \"objectProperty\": {}, \"IntProperty\": 2, \"decimalProperty\": \"1.3\", \"floatProperty\": \"1.4\", \"doubleProperty\": \"1.5\", \"dateTimeProperty\": \"2017-09-30T13:20:06.502+03:00\", \"timeSpanProperty\": \"03:25:45\", \"boolProperty\": true }");
            //instance = DeserializeObject<SimpleTestObject>("{ \"stringproperty\": \"test\", \"objectProperty\": {}, \"IntProperty\": \"2\", \"decimalProperty\": \"1.3\", \"floatProperty\": \"1.4\", \"doubleProperty\": \"1.5\", \"dateTimeProperty\": \"2017-09-30T13:20:06.502+03:00\", \"timeSpanProperty\": \"03:25:45\", \"boolProperty\": \"true\" }");
            Assert.AreEqual(true, instance != null);
            Assert.AreEqual("test", instance.StringProperty);
            Assert.AreEqual(true, instance.ObjectProperty != null);
            Assert.AreEqual(2, instance.IntProperty);
            Assert.AreEqual(1.3m, instance.DecimalProperty);
            Assert.AreEqual(1.4f, instance.FloatProperty);
            Assert.AreEqual(1.5d, instance.DoubleProperty);
            Assert.AreEqual(new DateTime(2017, 9, 30, 13, 20, 06, 502), instance.DateTimeProperty);
            Assert.AreEqual(TimeSpan.FromSeconds(12345), instance.TimeSpanProperty);
            Assert.AreEqual(true, instance.BoolProperty);
        }
        T DeserializeObject<T>(string json) {
            //return JsonConvert.DeserializeObject<T>(json);
            return JsonLite.DeserializeObject<T>(json);
        }
    }

    [TestFixture]
    public class JsonLiteParserTests {
        JsonLiteParser parser = new JsonLiteParser();

        [Test]
        public void ParseNull() {
            Assert.AreEqual(null, parser.TryParse(null));
        }
        [Test]
        public void ParseEmptyString() {
            JsonLiteObjectBase obj = parser.TryParse(String.Empty);
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{}", obj.ToDebugString());
        }
        [Test]
        public void ParseEmptyObject() {
            JsonLiteObjectBase obj = parser.TryParse("{}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{}", obj.ToDebugString());

            obj = parser.TryParse(" { } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{}", obj.ToDebugString());
        }
        [Test]
        public void ParseSimpleValue() {
            JsonLiteObjectBase obj = parser.TryParse("{\"property\":\"value\"}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"value\"}", obj.ToDebugString());

            obj = parser.TryParse(" {  \"property\" : \"value\" } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"value\"}", obj.ToDebugString());

            obj = parser.TryParse("{\"property\":\"va \\\\\\r\\n\\t\\b\\f\\/lue\"}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"va \\\r\n\t\b\f/lue\"}", obj.ToDebugString());

            obj = parser.TryParse("{\"property\":true}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"true\"}", obj.ToDebugString());

            obj = parser.TryParse(" { \"property\" : true } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"true\"}", obj.ToDebugString());

            obj = parser.TryParse("{\"property\":null}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"null\"}", obj.ToDebugString());

            obj = parser.TryParse(" { \"property\" : null } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"null\"}", obj.ToDebugString());

            obj = parser.TryParse("{\"property\":1.5}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"1.5\"}", obj.ToDebugString());

            obj = parser.TryParse(" { \"property\" : 1.5 } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"1.5\"}", obj.ToDebugString());
        }
        [Test]
        public void ParseSeveralStringValues() {
            JsonLiteObjectBase obj = parser.TryParse("{\"property\":\"value\",\"property1\":\"value1\",\"property2\":\"value2\"}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"value\",\r\n\"property1\":\"value1\",\r\n\"property2\":\"value2\"}", obj.ToDebugString());

            obj = parser.TryParse(" { \"property\" : \"value\" , \"property1\" : \"value1\" , \"property2\" : \"value2\" } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"value\",\r\n\"property1\":\"value1\",\r\n\"property2\":\"value2\"}", obj.ToDebugString());

            obj = parser.TryParse("{\"property\":true,\"property1\":false,\"property2\":null}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"true\",\r\n\"property1\":\"false\",\r\n\"property2\":\"null\"}", obj.ToDebugString());

            obj = parser.TryParse(" { \"property\" : true , \"property1\" : false , \"property2\" : null } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":\"true\",\r\n\"property1\":\"false\",\r\n\"property2\":\"null\"}", obj.ToDebugString());
        }
        [Test]
        public void ParseNestedObject() {
            JsonLiteObjectBase obj = parser.TryParse("{\"property\":{\"innerProperty\":\"innerValue\"}}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":{\"innerProperty\":\"innerValue\"}}", obj.ToDebugString());

            obj = parser.TryParse(" { \"property\" : { \"innerProperty\" : \"innerValue\" } } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":{\"innerProperty\":\"innerValue\"}}", obj.ToDebugString());
        }
        [Test]
        public void ParseNestedArray() {
            JsonLiteObjectBase obj = parser.TryParse("{\"property\":[\"innerProperty\",\"innerValue\"]}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":[\"innerProperty\",\r\n\"innerValue\"]}", obj.ToDebugString());

            obj = parser.TryParse(" { \"property\" : [ \"innerProperty\" , \"innerValue\" ] } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":[\"innerProperty\",\r\n\"innerValue\"]}", obj.ToDebugString());
        }
        [Test]
        public void ParseMixed() {
            JsonLiteObjectBase obj = parser.TryParse("{\"property\":[\"innerProperty\",\"innerValue\"],\"property1\":{\"innerProperty\":\"ivalue\"},\"property2\":\"value2\"}");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":[\"innerProperty\",\r\n\"innerValue\"],\r\n\"property1\":{\"innerProperty\":\"ivalue\"},\r\n\"property2\":\"value2\"}", obj.ToDebugString());

            obj = parser.TryParse(" { \"property\" : [ \"innerProperty\" , \"innerValue\" ] , \"property1\" : { \"innerProperty\" : \"ivalue\" } , \"property2\" : \"value2\" } ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("{\"property\":[\"innerProperty\",\r\n\"innerValue\"],\r\n\"property1\":{\"innerProperty\":\"ivalue\"},\r\n\"property2\":\"value2\"}", obj.ToDebugString());
        }
        //[Test]
        //public void ParseSimpleBoolValue() {
        //    JsonLiteObjectBase obj = parser.TryParse("{\"property\":true}");
        //    Assert.AreEqual(true, obj != null);
        //    Assert.AreEqual("{\"property\":\"true\"}", obj.ToDebugString());

        //    obj = parser.TryParse(" { \"property\" : true } ");
        //    Assert.AreEqual(true, obj != null);
        //    Assert.AreEqual("{\"property\":\"true\"}", obj.ToDebugString());
        //}
        [Test]
        public void ParseEmptyArray() {
            JsonLiteObjectBase obj = parser.TryParse("[]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[]", obj.ToDebugString());

            obj = parser.TryParse(" [ ] ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[]", obj.ToDebugString());
        }
        [Test]
        public void ParseSingleItemArray() {
            JsonLiteObjectBase obj = parser.TryParse("[\"test\"]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[\"test\"]", obj.ToDebugString());

            obj = parser.TryParse(" [ \"test\" ] ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[\"test\"]", obj.ToDebugString());

            obj = parser.TryParse("[true]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[\"true\"]", obj.ToDebugString());

            obj = parser.TryParse("[20.3]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[\"20.3\"]", obj.ToDebugString());

            obj = parser.TryParse("[null]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[\"null\"]", obj.ToDebugString());
        }
        [Test]
        public void ParseMultiStringItemArray() {
            JsonLiteObjectBase obj = parser.TryParse("[\"test\",\"passed\",true,20.3,null]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[\"test\",\r\n\"passed\",\r\n\"true\",\r\n\"20.3\",\r\n\"null\"]", obj.ToDebugString());

            obj = parser.TryParse(" [ \"test\" , \"passed\" , true , 20.3 , null ] ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[\"test\",\r\n\"passed\",\r\n\"true\",\r\n\"20.3\",\r\n\"null\"]", obj.ToDebugString());
        }
        [Test]
        public void ParseSingleObjectItemArray() {
            JsonLiteObjectBase obj = parser.TryParse("[{\"property\":\"value\"}]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[{\"property\":\"value\"}]", obj.ToDebugString());

            obj = parser.TryParse(" [ { \"property\" : \"value\" } ] ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[{\"property\":\"value\"}]", obj.ToDebugString());
        }
        [Test]
        public void ParseMultiObjectItemArray() {
            JsonLiteObjectBase obj = parser.TryParse("[{\"property\":\"value\"},{\"property2\":\"value2\"}]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[{\"property\":\"value\"},\r\n{\"property2\":\"value2\"}]", obj.ToDebugString());

            obj = parser.TryParse(" [ { \"property\" : \"value\" } , { \"property2\" : \"value2\" } ] ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[{\"property\":\"value\"},\r\n{\"property2\":\"value2\"}]", obj.ToDebugString());
        }
        [Test]
        public void ParseSingleArrayItemArray() {
            JsonLiteObjectBase obj = parser.TryParse("[[\"test\"]]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[[\"test\"]]", obj.ToDebugString());

            obj = parser.TryParse(" [ [ \"test\" ] ] ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[[\"test\"]]", obj.ToDebugString());
        }
        [Test]
        public void ParseMultiArrayItemArray() {
            JsonLiteObjectBase obj = parser.TryParse("[[\"test\"],[\"passed\"]]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[[\"test\"],\r\n[\"passed\"]]", obj.ToDebugString());

            obj = parser.TryParse(" [ [ \"test\" ] , [ \"passed\" ] ] ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[[\"test\"],\r\n[\"passed\"]]", obj.ToDebugString());
        }
        [Test]
        public void ParseMixedItemArray() {
            JsonLiteObjectBase obj = parser.TryParse("[[\"test\"],{\"property\":\"passed\"},\"value\"]");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[[\"test\"],\r\n{\"property\":\"passed\"},\r\n\"value\"]", obj.ToDebugString());

            obj = parser.TryParse(" [ [ \"test\" ] , { \"property\" : \"passed\" } , \"value\" ] ");
            Assert.AreEqual(true, obj != null);
            Assert.AreEqual("[[\"test\"],\r\n{\"property\":\"passed\"},\r\n\"value\"]", obj.ToDebugString());
        }
    }
}
#endif