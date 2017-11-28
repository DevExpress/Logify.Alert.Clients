using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.Logify.Core.Internal {
    public class JsonLite {
        public static T DeserializeObject<T>(string json) {
            if (json == null)
                throw new ArgumentNullException();

            if (String.IsNullOrEmpty(json)) {
                if (Nullable.GetUnderlyingType(typeof(T)) != null)
                    return default(T);
                if (typeof(T).IsValueType)
                    throw new JsonLiteDeserializationException();
                return default(T);
            }

            T result;
            if (typeof(T) == typeof(string)) {
                string stringValue = DeserializeString(json);
                result = (T)Activator.CreateInstance(typeof(T), stringValue.ToCharArray());
            }
            else
                result = Activator.CreateInstance<T>();
            JsonLiteObjectBase instance = new JsonLiteParser().TryParse(json);
            ApplyProperties(instance, result);
            return result;
        }

        static void ApplyProperties(JsonLiteObjectBase instance, object result) {
            Type type = result.GetType();
            JsonLiteArray array = instance as JsonLiteArray;
            if (array != null) {
                ApplyPropertiesCore(array, result);
                return;
            }
            JsonLiteObject obj = instance as JsonLiteObject;
            if (obj != null) {
                ApplyPropertiesCore(obj, result);
                return;
            }
        }

        static void ApplyPropertiesCore(JsonLiteObject obj, object result) {
            if (obj == null)
                return;

            if (obj.Properties == null || obj.Properties.Count <= 0)
                return;

            foreach (string key in obj.Properties.Keys) {
                PropertyInfo pi = result.GetType().GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (pi != null)
                    ApplyProperty(result, pi, obj.Properties[key]);
            }
        }
        static void ApplyProperty<T>(T instance, PropertyInfo pi, JsonLiteObjectBase value) {
            if (IsKnownPropertyType(pi.PropertyType))
                pi.SetValue(instance, ConvertSimpleValue(value, pi.PropertyType), null);
            //else if (IsEnumerableProperty(pi.PropertyType))
            //    pi.SetValue(instance, ConvertToArrayOf(value, pi.PropertyType));
            else
                pi.SetValue(instance, ConvertToObject(value, pi.PropertyType), null);
        }

        static bool IsKnownPropertyType(Type propertyType) {
            return propertyType.IsPrimitive ||
                propertyType == typeof(string) ||
                propertyType == typeof(decimal) ||
                propertyType == typeof(DateTime) ||
                propertyType == typeof(TimeSpan);
        }

        static object ConvertToObject(JsonLiteObjectBase value, Type propertyType) {
            JsonLiteObject jsonValue = value as JsonLiteObject;
            if (jsonValue == null) {
                JsonLiteValue val = value as JsonLiteValue;
                if (val != null && val.Value == "null")
                    return null;

                throw new JsonLiteDeserializationException();
            }

            object instance = Activator.CreateInstance(propertyType);
            if (instance != null) {
                ApplyProperties(value, instance);
            }
            return instance;
        }

        static object ConvertSimpleValue(JsonLiteObjectBase value, Type propertyType) {
            JsonLiteValue jsonValue = value as JsonLiteValue;
            if (jsonValue == null)
                throw new JsonLiteDeserializationException();
            if (propertyType == typeof(TimeSpan))
                return TimeSpan.Parse(jsonValue.Value, CultureInfo.InvariantCulture);
            return Convert.ChangeType(jsonValue.Value, propertyType, CultureInfo.InvariantCulture);
        }

        static void ApplyPropertiesCore(JsonLiteArray array, object result) {
            JsonLiteArray jsonValue = array as JsonLiteArray;
            if (jsonValue == null)
                throw new JsonLiteDeserializationException();

            //TODO:
        }

        static string DeserializeString(string json) {
            return String.Empty;
        }
    }
    public class JsonLiteDeserializationException : Exception {

    }
    public class JsonLiteParser {
        Stack<ParserContext> context;
        //JsonLiteObject objectInstance;
        //JsonLiteArray arrayInstance;
        class ParserContext {
            public JsonLiteParserState State { get; set; }
            public JsonLiteObjectBase Result { get; set; }
            public StringBuilder Text { get; set; }
            public char Char { get; set; }
        }

        enum JsonLiteParserState {
            Start,

            ObjectExpectKey,
            ObjectExpectColon,
            ObjectExpectValue,
            ObjectExpectComma,

            ArrayExpectValue,
            ArrayExpectComma,

            ExpectValue,
            ExpectQuotedValue,
            ExpectEscapedValue,

            Finished,
            ParseFailed,
        }
        static Dictionary<JsonLiteParserState, Action<char, Stack<ParserContext>>> stateParsers = CreateStateParsers();
        static Dictionary<JsonLiteParserState, Action<ParserContext, ParserContext>> acceptParsers = CreateAcceptParsers();

        static Dictionary<JsonLiteParserState, Action<char, Stack<ParserContext>>> CreateStateParsers() {
            var result = new Dictionary<JsonLiteParserState, Action<char, Stack<ParserContext>>>();
            result.Add(JsonLiteParserState.Start, Start_ParseChar);
            result.Add(JsonLiteParserState.ObjectExpectKey, ObjectExpectKey_ParseChar);
            result.Add(JsonLiteParserState.ObjectExpectColon, ObjectExpectColon_ParseChar);
            result.Add(JsonLiteParserState.ObjectExpectValue, ObjectExpectValue_ParseChar);
            result.Add(JsonLiteParserState.ObjectExpectComma, ObjectExpectComma_ParseChar);
            result.Add(JsonLiteParserState.ArrayExpectValue, ArrayExpectValue_ParseChar);
            result.Add(JsonLiteParserState.ArrayExpectComma, ArrayExpectComma_ParseChar);
            result.Add(JsonLiteParserState.ExpectValue, ExpectValue_ParseChar);
            result.Add(JsonLiteParserState.ExpectQuotedValue, ExpectQuotedValue_ParseChar);
            result.Add(JsonLiteParserState.ExpectEscapedValue, ExpectEscapedValue_ParseChar);
            return result;
        }
        static Dictionary<JsonLiteParserState, Action<ParserContext, ParserContext>> CreateAcceptParsers() {
            var result = new Dictionary<JsonLiteParserState, Action<ParserContext, ParserContext>>();
            result.Add(JsonLiteParserState.Start, Start_Accept);
            result.Add(JsonLiteParserState.ObjectExpectKey, ObjectExpectKey_Accept);
            result.Add(JsonLiteParserState.ObjectExpectValue, ObjectExpectValue_Accept);
            result.Add(JsonLiteParserState.ArrayExpectValue, ArrayExpectValue_Accept);
            result.Add(JsonLiteParserState.ExpectQuotedValue, ExpectQuotedValue_Accept);
            return result;
        }

        public JsonLiteObjectBase TryParse(string json) {
            if (json == null)
                return null;
            if (String.IsNullOrEmpty(json))
                return new JsonLiteObject();

            BeginParse();
            int count = json.Length;
            for (int i = 0; i < count; i++) {
                char ch = json[i];
                context.Peek().Char = ch;
                ParseChar(ch, context);
                if (context.Count <= 0)
                    return null;

                ParserContext item = context.Peek();
                if (item.State == JsonLiteParserState.ParseFailed)
                    return null;
                if (item.State == JsonLiteParserState.Finished) {
                    return item.Result;
                }
            }

            return null;
        }
        void BeginParse() {
            this.context = new Stack<ParserContext>();

            this.context.Push(new ParserContext() { State = JsonLiteParserState.Start });
        }
        static void ParseChar(char ch, Stack<ParserContext> context) {
            stateParsers[context.Peek().State].Invoke(ch, context);
        }
        static void Accept(Stack<ParserContext> context) {
            ParserContext innerContext = context.Pop();
            ParserContext current = context.Peek();
            acceptParsers[current.State].Invoke(current, innerContext);
        }
        static void Start_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case ' ': break;
                case '\r': break;
                case '\n': break;
                case '\t': break;
                case '[':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ArrayExpectValue, Result = new JsonLiteArray() });
                    break;
                case '{':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ObjectExpectKey, Result = new JsonLiteObject() });
                    break;
                default:
                    context.Peek().State = JsonLiteParserState.ParseFailed;
                    break;
            }
        }
        static void Start_Accept(ParserContext context, ParserContext innerContext) {
            context.State = JsonLiteParserState.Finished;
            context.Result = innerContext.Result;
        }
        static void ObjectExpectKey_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case ' ': break;
                case '\r': break;
                case '\n': break;
                case '\t': break;
                case '}':
                    Accept(context);
                    break;
                case '\"':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ExpectQuotedValue, Result = new JsonLiteArray(), Text = new StringBuilder() });
                    break;
                default:
                    context.Peek().State = JsonLiteParserState.ParseFailed;
                    break;
            }
        }
        static void ObjectExpectKey_Accept(ParserContext context, ParserContext innerContext) {
            JsonLiteObject instance = (JsonLiteObject)context.Result;
            context.Text = innerContext.Text;
            context.State = JsonLiteParserState.ObjectExpectColon;
        }
        static void ObjectExpectColon_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case ' ': break;
                case '\r': break;
                case '\n': break;
                case '\t': break;
                case ':':
                    context.Peek().State = JsonLiteParserState.ObjectExpectValue;
                    break;
                default:
                    context.Peek().State = JsonLiteParserState.ParseFailed;
                    break;
            }
        }
        static void ObjectExpectComma_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case ' ': break;
                case '\r': break;
                case '\n': break;
                case '\t': break;
                case '}':
                    Accept(context);
                    break;
                case ',':
                    ParserContext ctx = context.Peek();
                    ctx.State = JsonLiteParserState.ObjectExpectKey;
                    ctx.Text = new StringBuilder();
                    break;
                default:
                    context.Peek().State = JsonLiteParserState.ParseFailed;
                    break;
            }
        }
        static void ObjectExpectValue_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case ' ': break;
                case '\r': break;
                case '\n': break;
                case '\t': break;
                case '[':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ArrayExpectValue, Result = new JsonLiteArray() });
                    break;
                case '{':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ObjectExpectKey, Result = new JsonLiteObject() });
                    break;
                case '\"':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ExpectQuotedValue, Text = new StringBuilder() });
                    break;
                default:
                    context.Push(new ParserContext() { State = JsonLiteParserState.ExpectValue, Text = new StringBuilder() });
                    ParseChar(ch, context);
                    //context.Peek().State = JsonLiteParserState.ParseFailed;
                    break;
            }
        }
        static void ObjectExpectValue_Accept(ParserContext context, ParserContext innerContext) {
            JsonLiteObject instance = (JsonLiteObject)context.Result;
            if (innerContext.State == JsonLiteParserState.ArrayExpectValue || innerContext.State == JsonLiteParserState.ObjectExpectValue ||
                innerContext.State == JsonLiteParserState.ArrayExpectComma || innerContext.State == JsonLiteParserState.ObjectExpectComma ||
                innerContext.State == JsonLiteParserState.ObjectExpectKey) {
                instance.Properties[context.Text.ToString()] = innerContext.Result;
                context.State = JsonLiteParserState.ObjectExpectComma;
            }
            else if (innerContext.State == JsonLiteParserState.ExpectQuotedValue || innerContext.State == JsonLiteParserState.ExpectValue) {
                instance.Properties[context.Text.ToString()] = new JsonLiteValue() { Value = innerContext.Text.ToString() };
                context.State = JsonLiteParserState.ObjectExpectComma;
            }
            //else if (innerContext.State == JsonLiteParserState.ExpectValue) {
            //    instance.Properties[context.Text.ToString()] = new JsonLiteValue() { Value = innerContext.Text.ToString() };
            //    //if (innerContext.Char == ',') {
            //    //    context.State = JsonLiteParserState.ObjectExpectKey;
            //    //    context.Text = new StringBuilder();
            //    //}
            //    //else
            //        context.State = JsonLiteParserState.ObjectExpectComma;
            //}
            else
                context.State = JsonLiteParserState.ParseFailed;
        }
        static void ArrayExpectValue_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case ' ': break;
                case '\r': break;
                case '\n': break;
                case '\t': break;
                case ']':
                    Accept(context);
                    break;
                case '\"':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ExpectQuotedValue, Text = new StringBuilder() });
                    break;
                case '[':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ArrayExpectValue, Result = new JsonLiteArray(), Text = new StringBuilder() });
                    break;
                case '{':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ObjectExpectKey, Result = new JsonLiteObject(), Text = new StringBuilder() });
                    break;
                default:
                    context.Push(new ParserContext() { State = JsonLiteParserState.ExpectValue, Text = new StringBuilder() });
                    ParseChar(ch, context);
                    //context.Peek().State = JsonLiteParserState.ParseFailed;
                    break;
            }
        }
        static void ArrayExpectComma_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case ' ': break;
                case '\r': break;
                case '\n': break;
                case '\t': break;
                case ']':
                    Accept(context);
                    break;
                case ',':
                    ParserContext ctx = context.Peek();
                    ctx.State = JsonLiteParserState.ArrayExpectValue;
                    ctx.Text = new StringBuilder();
                    break;
                default:
                    context.Peek().State = JsonLiteParserState.ParseFailed;
                    break;
            }
        }
        static void ArrayExpectValue_Accept(ParserContext context, ParserContext innerContext) {
            JsonLiteArray array = (JsonLiteArray)context.Result;
            if (innerContext.State == JsonLiteParserState.ArrayExpectValue || innerContext.State == JsonLiteParserState.ObjectExpectValue ||
                innerContext.State == JsonLiteParserState.ArrayExpectComma || innerContext.State == JsonLiteParserState.ObjectExpectComma ||
                innerContext.State == JsonLiteParserState.ObjectExpectKey) {
                array.Values.Add(innerContext.Result);
                context.State = JsonLiteParserState.ArrayExpectComma;
            }
            else if (innerContext.State == JsonLiteParserState.ExpectQuotedValue || innerContext.State == JsonLiteParserState.ExpectValue) {
                array.Values.Add(new JsonLiteValue() { Value = innerContext.Text.ToString() });
                context.State = JsonLiteParserState.ArrayExpectComma;
            }
            else
                context.State = JsonLiteParserState.ParseFailed;
        }
        static void ExpectQuotedValue_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case '\"':
                    Accept(context);
                    break;
                case '\\':
                    context.Push(new ParserContext() { State = JsonLiteParserState.ExpectEscapedValue, Text = new StringBuilder() });
                    break;
                default:
                    context.Peek().Text.Append(ch);
                    break;
            }
        }
        static HashSet<char> allowedValueChars = CreateAllowedValueChars("null.true.false.0123456789+-E");

        static HashSet<char> CreateAllowedValueChars(string text) {
            HashSet<char> result = new HashSet<char>();
            int count = text.Length;
            for (int i = 0; i < count; i++) {
                if (!result.Contains(text[i]))
                    result.Add(text[i]);
            }
            return result;
        }

        static void ExpectValue_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case ' ':
                    Accept(context);
                    break;
                case ',':
                    Accept(context);
                    ParseChar(ch, context);
                    break;
                case '}':
                    Accept(context);
                    ParseChar(ch, context);
                    break;
                case ']':
                    Accept(context);
                    ParseChar(ch, context);
                    break;
                default:
                    if (allowedValueChars.Contains(ch))
                        context.Peek().Text.Append(ch);
                    else
                        context.Peek().State = JsonLiteParserState.ParseFailed;
                    break;
            }
        }
        static void ExpectQuotedValue_Accept(ParserContext context, ParserContext innerContext) {
            string text = innerContext.Text.ToString();
            context.Text.Append(text);
        }
        static void ExpectEscapedValue_ParseChar(char ch, Stack<ParserContext> context) {
            switch (ch) {
                case 't':
                    context.Peek().Text.Append('\t');
                    Accept(context);
                    break;
                case 'r':
                    context.Peek().Text.Append('\r');
                    Accept(context);
                    break;
                case 'n':
                    context.Peek().Text.Append('\n');
                    Accept(context);
                    break;
                case '\"':
                    context.Peek().Text.Append('\"');
                    Accept(context);
                    break;
                case '\\':
                    context.Peek().Text.Append('\\');
                    Accept(context);
                    break;
                case '/':
                    context.Peek().Text.Append('/');
                    Accept(context);
                    break;
                case 'b':
                    context.Peek().Text.Append('\b');
                    Accept(context);
                    break;
                case 'f':
                    context.Peek().Text.Append('\f');
                    Accept(context);
                    break;
                default:
                    context.Peek().State = JsonLiteParserState.ParseFailed;
                    break;
            }
        }
    }
    public abstract class JsonLiteObjectBase {
        public abstract string ToDebugString();
    }
    public class JsonLiteObject : JsonLiteObjectBase {
        public JsonLiteObject() {
            this.Properties = new Dictionary<string, JsonLiteObjectBase>();
        }
        public Dictionary<string, JsonLiteObjectBase> Properties { get; private set; }

        public override string ToDebugString() {
            StringBuilder result = new StringBuilder();
            result.Append('{');
            int i = 0;
            foreach (string key in Properties.Keys) {
                if (i > 0)
                    result.AppendLine(",");
                result.Append('\"');
                result.Append(key);
                result.Append("\":");
                result.Append(Properties[key].ToDebugString());
                i++;
            }
            result.Append('}');
            return result.ToString();
        }
    }
    public class JsonLiteValue : JsonLiteObjectBase {
        public string Value { get; set; }

        public override string ToDebugString() {
            return "\"" + Value + "\"";
        }
    }
    public class JsonLiteArray : JsonLiteObjectBase {
        public JsonLiteArray() {
            this.Values = new List<JsonLiteObjectBase>();
        }
        public List<JsonLiteObjectBase> Values { get; private set; }

        public override string ToDebugString() {
            StringBuilder result = new StringBuilder();
            result.Append('[');
            int count = Values.Count;
            for (int i = 0; i < count; i++) {
                if (i > 0)
                    result.AppendLine(",");

                result.Append(Values[i].ToDebugString());
            }
            result.Append(']');
            return result.ToString();
        }
    }
}
