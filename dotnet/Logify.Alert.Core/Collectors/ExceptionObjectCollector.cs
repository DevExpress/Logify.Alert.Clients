using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Core.Internal {
    public class ExceptionObjectInfoCollector : CompositeInfoCollector {
        public ExceptionObjectInfoCollector(ILogifyClientConfiguration config)
            : base(config) {
        }

        public override void Process(Exception ex, ILogger logger) {
            logger.BeginWriteArray("exception");
            try {
                for (; ; ) {
                    logger.BeginWriteObject(String.Empty);
                    try {
                        base.Process(ex, logger);
                    }
                    catch {
                    }
                    finally {
                        logger.EndWriteObject(String.Empty);
                    }
                    ex = ex.InnerException;
                    if (ex == null)
                        return;
                }
            }
            finally {
                logger.EndWriteArray("exception");
            }
        }
        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            Collectors.Add(new ExceptionTypeCollector());
            Collectors.Add(new ExceptionMessageCollector());
            Collectors.Add(new ExceptionStackCollector());
            Collectors.Add(new ExceptionNormalizedStackCollector());
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
                        logger.WriteValue("key", key != null ? key.ToString() : "null");
                        logger.WriteValue("value", value != null ? value.ToString() : "null");
                    }
                    finally {
                        logger.EndWriteObject(String.Empty);
                    }

                }
            }
            finally {
                logger.EndWriteArray("data");
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
                string normalizedStackTrace = ExceptionStackCollector.GetFullStackTrace(ex, NormalizeStackTrace(ex.StackTrace), OuterStackKeys.StackNormalized);
                logger.WriteValue("normalizedStackTrace", normalizedStackTrace);
            }
            finally {
                Thread.CurrentThread.CurrentCulture = prevCulture;
                Thread.CurrentThread.CurrentUICulture = prevUICulture;
            }
        }

        internal static string NormalizeStackTrace(string stackTrace) {
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
    }

}
