using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    public interface IStackTraceNormalizer {
        string NormalizeStackTrace(string stackTrace);
    }
    public class RegexpExceptionNormalizedStackCollector : IInfoCollector, IStackTraceNormalizer {
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

        static Regex normalizator = new Regex(@"^(\s*(?:at)\s*)?(.+[\)|-])\s*((\[.+\])?\s+in\s+.*)?$", RegexOptions.Multiline);
        public string NormalizeStackTrace(string stackTrace) {
            if (String.IsNullOrEmpty(stackTrace))
                return String.Empty;
            const string replacement = "$2";
            return normalizator.Replace(stackTrace, replacement) + Environment.NewLine;
        }
    }


    public class LegacyExceptionNormalizedStackCollector : IInfoCollector, IStackTraceNormalizer {
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

        public string NormalizeStackTrace(string stackTrace) {
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