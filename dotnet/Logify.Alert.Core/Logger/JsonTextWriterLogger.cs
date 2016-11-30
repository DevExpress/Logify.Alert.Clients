using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DevExpress.Office.Utils;

namespace DevExpress.Logify.Core {
    public class TextWriterLogger : ILogger {
        static readonly Dictionary<char, string> replaceTable = CreateReplaceTable();
        readonly Dictionary<string, object> data = new Dictionary<string, object>();

        readonly FastCharacterMultiReplacement replacement = new FastCharacterMultiReplacement(new StringBuilder());
        readonly TextWriter writer;


        public Dictionary<string, object> Data { get { return data; } }

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
        public TextWriterLogger(TextWriter writer) {
            this.writer = writer;
        }

        public TextWriter Writer { get { return writer; } }



        public void BeginWriteObject(string name) {
            if (writer != null) {
                if (String.IsNullOrEmpty(name))
                    writer.WriteLine("{");
                else
                    writer.WriteLine("\"" + replaceDot(name) + "\": {");
            }
        }

        public void WriteValue(string name, string text) {
            if (writer != null) {
                text = replacement.PerformReplacements(text, replaceTable);
                //if (text.Contains(@"\"))
                //    text = text.Replace(@"\", @"\\");
                writer.WriteLine("\"" + replaceDot(name) + "\":\"" + text + "\",");
            }
        }
        
        public void WriteValue(string name, bool value) {
            if (writer != null)
                writer.WriteLine("\"" + replaceDot(name) + "\":" + (value ? "true" : "false") + ",");
        }
        
        public void WriteValue(string name, int value) {
            if (writer != null) {
                writer.WriteLine("\"" + replaceDot(name) + "\":" + value.ToString() + ",");
            }
        }

        public void WriteValue(string name, Array array) {
            if (array == null || array.Length == 0) return;
            this.BeginWriteArray(replaceDot(name));
            if (writer != null) {
                System.Collections.IEnumerator iterator = array.GetEnumerator();
                for (int i = 0; iterator.MoveNext(); i++) {
                    string value = "\"" + iterator.Current.ToString() + "\"";
                    if (i != array.Length - 1) value += ",";
                    writer.WriteLine(value);
                }
            }
            this.EndWriteArray(name);
        }

        public void EndWriteObject(string name) {
            if (writer != null)
                writer.WriteLine("},");
        }

        public void BeginWriteArray(string name) {
            if (writer != null) {
                if (String.IsNullOrEmpty(name))
                    writer.WriteLine("[");
                else
                    writer.WriteLine("\"" + replaceDot(name) + "\": [");
            }
        }
        
        public void EndWriteArray(string name) {
            if (writer != null)
                writer.WriteLine("],");
        }


        string replaceDot(string name) {
            //MongoDB doesn't support keys with a dot in them
            return name.Replace('.', '\uff0E'); 
        }
    }
}
