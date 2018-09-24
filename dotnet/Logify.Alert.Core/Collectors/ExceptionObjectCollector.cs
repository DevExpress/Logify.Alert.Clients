using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;
using DevExpress.Logify.Core.Internal;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace DevExpress.Logify.Core.Internal {
    public class ExceptionObjectInfoCollector : CompositeInfoCollector {
        public ExceptionObjectInfoCollector(LogifyCollectorContext context)
            : base(context) {
            Collectors.Add(new ExceptionMethodCallArgumentsCollector(context.CallArgumentsMap));
        }

        public override void Process(Exception ex, ILogger logger) {
            ExceptionProcessors processor = new ExceptionProcessors(ex);
            logger.BeginWriteArray("exception");
            try {
                for (; ; ) {
                    logger.BeginWriteObject(String.Empty);
                    try {
                        ex = processor.PreProcess(ex);
                        base.Process(ex, logger);
                    } finally {
                        logger.EndWriteObject(String.Empty);
                    }
                    ex = processor.GetNextException(ex);
                    if (ex == null)
                        return;
                }
            } finally {
                logger.EndWriteArray("exception");
            }
        }

        protected override void RegisterCollectors(LogifyCollectorContext context) {
            Collectors.Add(new ExceptionTypeCollector());
            Collectors.Add(new ExceptionMessageCollector());
            Collectors.Add(new ExceptionStackCollector());
            Collectors.Add(new ExceptionNormalizedStackCollector());
            Collectors.Add(new InnerExceptionIdCollector());
            Collectors.Add(new ExceptionDataCollector());
            //etc
        }
    }    
    //TODO: move to platform specific assembly
    public class ExceptionTypeCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.WriteValue("type", ex.GetType().ToString());
        }
    }
    //TODO: move to platform specific assembly
    public class ExceptionMessageCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.WriteValue("message", ex.Message);
        }
    }
    //TODO: move to platform specific assembly
    public class ExceptionStackCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.WriteValue("stackTrace", GetFullStackTrace(ex, ex.StackTrace, OuterStackKeys.Stack));
        }

        internal static string GetFullStackTrace(Exception ex, string innerStackTrace, string outerStackKey) {
            string fullStackTrace = innerStackTrace;

            if (ex.Data != null && ex.Data.Contains(outerStackKey)) {
                string outerStack = ex.Data[outerStackKey] as string;
                if (!String.IsNullOrEmpty(outerStack))
                    fullStackTrace += outerStack;
            }
            return fullStackTrace;
        }
    }
    public class InnerExceptionIdCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            if (ex.Data != null && ex.Data.Contains(ExceptionObjectKeys.InnerExceptionNumber) && ex.Data[ExceptionObjectKeys.InnerExceptionNumber] != null) { 
                logger.WriteValue("groupId", ex.Data[ExceptionObjectKeys.InnerExceptionNumber].ToString());
                ex.Data.Remove(ExceptionObjectKeys.InnerExceptionNumber);
            }
        }
    }
    //TODO: move to platform specific assembly
    public class ExceptionDataCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            IDictionary data = ex.Data;
            if (data == null)
                return;

            int count = data.Count;
            if (data.Contains(OuterStackKeys.Stack))
                count--;
            if (data.Contains(OuterStackKeys.StackNormalized))
                count--;

            if (count <= 0)
                return;

            logger.BeginWriteArray("data");
            try {

                foreach (object key in data.Keys) {
                    if (Object.Equals(key, OuterStackKeys.Stack) || Object.Equals(key, OuterStackKeys.StackNormalized))
                        continue;
                    object value = data[key];
                    logger.BeginWriteObject(String.Empty);
                    try {
                        string keyName = key != null ? key.ToString() : "null";
                        logger.WriteValue("key", keyName);
                        if (String.Compare(keyName, "SmartStackFrames", StringComparison.InvariantCultureIgnoreCase) == 0)
                            logger.WriteValue("value", SerializeSmartStackFramesValue(value));
                        else
                            logger.WriteValue("value", value != null ? value.ToString() : "null");
                    } finally {
                        logger.EndWriteObject(String.Empty);
                    }

                }
            } finally {
                logger.EndWriteArray("data");
            }
        }
        string SerializeSmartStackFramesValue(object value) {
            if (value == null)
                return null;

            ICollection collection = value as ICollection;
            if (collection == null)
                return value.ToString();

            return SerializeSmartStackFramesCollection(collection);
        }
        string SerializeSmartStackFramesCollection(ICollection collection) {
            if (collection == null || collection.Count <= 0)
                return String.Empty;

            StringBuilder content = new StringBuilder();
            StringWriter writer = new StringWriter(content);
            TextWriterLogger logger = new TextWriterLogger(writer);

            logger.BeginWriteObject(String.Empty);
            try {
                SerializeSmartStackFramesCollection(collection, logger);
            } finally {
                logger.EndWriteObject(String.Empty);
            }

            return content.ToString();
        }
        void SerializeSmartStackFramesCollection(ICollection collection, ILogger logger) {
            logger.BeginWriteArray("frames");
            try {
                foreach (object item in collection) {
                    logger.BeginWriteObject(String.Empty);
                    try {
                        dynamic obj = item;
                        //int ilOffset = obj.ILOffset;
                        //object[] objects = obj.Objects;
                        logger.WriteValue("exceptionStackDepth", obj.ExceptionStackDepth);
                        logger.WriteValue("methodId", obj.MethodID);
                    } finally {
                        logger.EndWriteObject(String.Empty);
                    }
                }
            } catch {
            } finally {
                logger.EndWriteArray("frames");
            }
        }
    }
    //TODO: move to platform specific assembly
    public class ExceptionNormalizedStackCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            CultureInfo prevCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo prevUICulture = Thread.CurrentThread.CurrentUICulture;
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                string normalizedStackTrace = ExceptionStackCollector.GetFullStackTrace(ex, NormalizeStackTraceSmokers(ex.StackTrace), OuterStackKeys.StackNormalized);
                logger.WriteValue("normalizedStackTrace", normalizedStackTrace);
            } finally {
                Thread.CurrentThread.CurrentCulture = prevCulture;
                Thread.CurrentThread.CurrentUICulture = prevUICulture;
            }
        }

        public static string NormalizeStackTrace(string stackTrace) {
            if (String.IsNullOrEmpty(stackTrace))
                return String.Empty;
            string[] frames = stackTrace.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < frames.Length; i++)
                result.AppendLine(NormalizeStackFrame(frames[i]));
            return result.ToString();
        }

        static string NormalizeStackFrame(string frame) {
            const string prefix = "   at ";
            const string suffix = ") in ";
            if (frame.StartsWith(prefix))
                frame = frame.Substring(prefix.Length);
            int suffixIndex = frame.LastIndexOf(suffix);
            if (suffixIndex >= 0)
                frame = frame.Substring(0, suffixIndex + 1);
            return frame;
        }
        static Regex normalizator = new Regex(@"^(\s*(?:at)\s*)?(.+[\)|-])\s*((\[.+\])?\s+in\s+.*)?$", RegexOptions.Multiline);
        static string NormalizeStackTraceSmokers(string stackTrace) {
            if (String.IsNullOrEmpty(stackTrace))
                return String.Empty;
            const string replacement = "$2";
            return normalizator.Replace(stackTrace, replacement);
        }
    }

    public class ExceptionMethodCallArgumentsCollector : IInfoCollector {
        readonly MethodCallArgumentMap callArgumentsMap;
        public ExceptionMethodCallArgumentsCollector(MethodCallArgumentMap callArgumentsMap) {
            this.callArgumentsMap = callArgumentsMap;
        }
        public virtual void Process(Exception ex, ILogger logger) {
            CultureInfo prevCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo prevUICulture = Thread.CurrentThread.CurrentUICulture;
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                WriteCallParameters(ex, logger);
            } finally {
                Thread.CurrentThread.CurrentCulture = prevCulture;
                Thread.CurrentThread.CurrentUICulture = prevUICulture;
            }
        }

        void WriteCallParameters(Exception ex, ILogger logger) {
            if (callArgumentsMap == null || callArgumentsMap.Count <= 0)
                return;

            MethodCallStackArgumentMap map;
            if (!callArgumentsMap.TryGetValue(ex, out map) || map == null)
                return;
            logger.BeginWriteArray("callParams");
            try {
                foreach (int frame in map.Keys) {
                    MethodCallInfo methodCallInfo = map[frame];
                    if (methodCallInfo == null || methodCallInfo.Arguments == null || methodCallInfo.Arguments.Count <= 0 || methodCallInfo.Method == null)
                        continue;

                    logger.BeginWriteObject(String.Empty);
                    try {
                        logger.WriteValue("index", frame);
                        WriteMethodCallParameters(methodCallInfo, logger);
                    } catch {
                    } finally {
                        logger.EndWriteObject(String.Empty);
                    }
                }
            } finally {
                logger.EndWriteArray("callParams");
            }
        }
        void WriteMethodCallParameters(MethodCallInfo info, ILogger logger) {
            logger.BeginWriteObject("params");
            try {
                ParameterInfo[] parameters = info.Method.GetParameters();
                if (parameters == null)
                    return;

                int count = Math.Min(parameters.Length, info.Arguments.Count);
                for (int i = 0; i < count; i++)
                    WriteCallParameterValue(parameters[i], info.Arguments[i], logger);
            } catch {
            } finally {
                logger.EndWriteObject("params");
            }
        }
        void WriteCallParameterValue(ParameterInfo parameter, object argument, ILogger logger) {
            if (parameter == null)
                return;

            logger.BeginWriteObject(parameter.Name);
            try {
                string value = argument != null ? argument.ToString() : "null";
                string type = argument != null ? argument.GetType().FullName : parameter.ParameterType.FullName;
                logger.WriteValue("value", value);
                logger.WriteValue("type", type);
            } catch {
            } finally {
                logger.EndWriteObject(parameter.Name);
            }
        }
    }
}
