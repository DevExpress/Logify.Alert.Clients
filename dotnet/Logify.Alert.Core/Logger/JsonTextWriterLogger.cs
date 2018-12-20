using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DevExpress.Office.Utils;

namespace DevExpress.Logify.Core.Internal {
    public class TextWriterLogger : ILogger {
        #region JsonLoggerObject
        class JsonLoggerObject {
            #region static
            static readonly Dictionary<char, string> replaceTable = CreateReplaceTable();
            static Dictionary<char, string> CreateReplaceTable() {
                Dictionary<char, string> result = new Dictionary<char, string>();
                result.Add('\r', @"\r");
                result.Add('\n', @"\n");
                result.Add('\t', @"\t");
                result.Add('\"', @"\""");
                result.Add('\\', @"\\");
                result.Add('\b', @"\b");
                result.Add('\f', @"\f");
                for (int i = 0; i < 32; i++) {
                    char ch = (char)i;
                    if (!result.ContainsKey(ch))
                        result.Add(ch, String.Format(@"\u{0:x4}", i));
                }
                return result;
            }
            static string replaceDot(string name) {
                //MongoDB doesn't support keys with a dot in them
                return name.Replace('.', '\uff0E');
            }
            #endregion
            #region fields
            string name;
            string value;
            List<JsonLoggerObject> objects;
            readonly bool isArray;
            readonly FastCharacterMultiReplacement replacement = new FastCharacterMultiReplacement(new StringBuilder());
            readonly JsonLoggerObject parent;
            #endregion
            #region Properties
            public JsonLoggerObject Parent { get { return parent; } }
            public bool NeedQuotes { get; set; }
            #endregion
            public JsonLoggerObject() : this(string.Empty, null, null) {
            }
            public JsonLoggerObject(string name, string value, JsonLoggerObject parent) : this(name, value, parent, false) {
            }
            public JsonLoggerObject(string name, string value, JsonLoggerObject parent, bool isArray) {
                this.NeedQuotes = false;
                this.name = name;
                this.value = value;
                this.parent = parent;
                this.isArray = isArray;
            }
            public void Add(JsonLoggerObject jsonLoggerObject) {
                if (this.objects == null) {
                    this.objects = new List<JsonLoggerObject>();
                }
                this.objects.Add(jsonLoggerObject);
            }
            public void SaveContent(TextWriter writer, bool lastObject) {
                if (writer != null) {
                    string jsonName = String.IsNullOrEmpty(name) ? string.Empty : "\"" + replaceDot(name) + "\":";
                    if (objects == null) {
                        string text = !NeedQuotes ? value : replacement.PerformReplacements(value, replaceTable);
                        string quote = !NeedQuotes ? string.Empty : "\"";
                        string comma = lastObject ? string.Empty : ",";
                        if (value == null) {
                            text = isArray ? "[]" : "";
                        }
                        writer.WriteLine(jsonName + quote + text + quote + comma);
                    } else {
                        string openedSymbol = isArray ? "[" : "{";
                        string closedSymbol = isArray ? "]" : "}";
                        writer.WriteLine(jsonName + openedSymbol);
                        if (objects != null) {
                            for (int i = 0; i < objects.Count; i++) {
                                objects[i].SaveContent(writer, i == objects.Count - 1);
                            }
                        }
                        writer.WriteLine(closedSymbol + (lastObject ? string.Empty : ","));
                    }
                }

            }
        }
        #endregion
        #region fields
        readonly TextWriter writer;
        readonly Dictionary<string, object> data = new Dictionary<string, object>();
        JsonLoggerObject lastOpenedObject;
        #endregion
        #region Properties
        public TextWriter Writer { get { return writer; } }
        public Dictionary<string, object> Data { get { return data; } }
        #endregion

        public TextWriterLogger(TextWriter writer) {
            this.writer = writer;
            lastOpenedObject = null;
        }

        public void BeginWriteObject(string name) {
            BeginWriteObject(name, false);
        }
        public void EndWriteObject(string name) {
            EndWriteObject();
        }
        public void BeginWriteArray(string name) {
            BeginWriteObject(name, true);
        }
        public void EndWriteArray(string name) {
            EndWriteObject();
        }
        public void WriteValue(string name, string text) {
            JsonLoggerObject jsonLoggerObject = new JsonLoggerObject(name, text, lastOpenedObject);
            jsonLoggerObject.NeedQuotes = true;
            IncludeInObjectModel(jsonLoggerObject);
        }
        public void WriteValue(string name, bool value) {
            IncludeInObjectModel(new JsonLoggerObject(name, (value ? "true" : "false"), lastOpenedObject));
        }
        public void WriteValue(string name, int value) {
            IncludeInObjectModel(new JsonLoggerObject(name, value.ToString(), lastOpenedObject));
        }
        public void WriteValue(string name, Array array) {
            if (array == null || array.Length == 0) return;
            IncludeInObjectModel(new JsonLoggerObject(name, ArrayToString(array), lastOpenedObject));
        }

        void BeginWriteObject(string name, bool isArray) {
            JsonLoggerObject newObject = new JsonLoggerObject(name, null, lastOpenedObject, isArray);
            IncludeInObjectModel(newObject);
            lastOpenedObject = newObject;
        }
        void EndWriteObject() {
            if (lastOpenedObject == null)
                return;
            if (lastOpenedObject.Parent != null) {
                lastOpenedObject = lastOpenedObject.Parent;
            } else {
                lastOpenedObject.SaveContent(writer, true);
                lastOpenedObject = null;
            }
        }
        string ArrayToString(Array array) {
            string result = "[";
            System.Collections.IEnumerator iterator = array.GetEnumerator();
            for (int i = 0; iterator.MoveNext(); i++) {
                result += "\"" + iterator.Current.ToString() + "\"";
                if (i != array.Length - 1) result += ",";
            }
            result += "]";
            return result;
        }
        void IncludeInObjectModel(JsonLoggerObject jsonLoggerObject) {
            if (lastOpenedObject != null)
                lastOpenedObject.Add(jsonLoggerObject);
            else
                jsonLoggerObject.SaveContent(writer, true);
        }
    }
    
}
