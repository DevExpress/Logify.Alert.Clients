using System;
using System.Collections.Generic;
using System.IO;

namespace DevExpress.Logify.Core.Internal {
    public interface ILogger {
        TextWriter Writer { get; }
        Dictionary<string, object> Data { get; }
        void BeginWriteObject(string name);
        void EndWriteObject(string name);
        void BeginWriteArray(string name);
        void EndWriteArray(string name);
        void WriteValue(string name, string text);
        void WriteValue(string name, bool value);
        void WriteValue(string name, int value);
        void WriteValue(string name, Array array);
    }
}