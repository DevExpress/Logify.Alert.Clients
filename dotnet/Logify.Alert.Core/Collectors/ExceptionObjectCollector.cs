using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;

namespace DevExpress.Logify.Core {
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
            logger.WriteValue("stackTrace", ex.StackTrace);
        }
    }
    //TODO: move to platform specific assembly
    public class ExceptionDataCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            IDictionary data = ex.Data;
            if (data == null || data.Count <= 0)
                return;

            logger.BeginWriteArray("data");
            try {

                foreach (object key in data.Keys) {
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
#if NETSTANDARD1_4
            CultureInfo prevCulture = CultureInfo.CurrentCulture;
            CultureInfo prevUICulture = CultureInfo.CurrentUICulture;
#else
            CultureInfo prevCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo prevUICulture = Thread.CurrentThread.CurrentUICulture;
#endif
            try {
#if NETSTANDARD1_4
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
#else
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
#endif

                string normalizedStackTrace = NormalizeStackTrace(ex.StackTrace);
                logger.WriteValue("normalizedStackTrace", normalizedStackTrace);
            }
            finally {
#if NETSTANDARD1_4
                CultureInfo.CurrentCulture = prevCulture;
                CultureInfo.CurrentUICulture = prevUICulture;
#else
                Thread.CurrentThread.CurrentCulture = prevCulture;
                Thread.CurrentThread.CurrentUICulture = prevUICulture;
#endif
            }
        }

        string NormalizeStackTrace(string stackTrace) {
            if (String.IsNullOrEmpty(stackTrace))
                return String.Empty;
            string[] frames = stackTrace.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < frames.Length; i++)
                result.AppendLine(NormalizeStackFrame(frames[i]));
            return result.ToString();
        }

        string NormalizeStackFrame(string frame) {
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
