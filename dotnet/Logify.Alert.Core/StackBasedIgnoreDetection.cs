using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace DevExpress.Logify.Core {
    public class StackBasedExceptionIgnoreDetection : IExceptionIgnoreDetection {
        #region ShouldIgnoreException
        public ShouldIgnoreResult ShouldIgnoreException(Exception ex) {
            try {
                for (;;) {
                    if (ex == null)
                        return ShouldIgnoreResult.Unknown;

                    ShouldIgnoreResult result = ShouldIgnoreExceptionByExceptionType(ex);
                    if (result != ShouldIgnoreResult.Unknown)
                        return result;
                    result = ShouldIgnoreExceptionByStack(ex);
                    if (result != ShouldIgnoreResult.Unknown)
                        return result;

                    ex = ex.InnerException;
                }
            }
            catch {
                return ShouldIgnoreResult.Unknown;
            }
        }

        ShouldIgnoreResult ShouldIgnoreExceptionByExceptionType(Exception ex) {
            try {
                Type exceptionType = ex.GetType();
#if NETSTANDARD
                IEnumerable<LogifyIgnoreAttribute> attributes = exceptionType.GetTypeInfo().GetCustomAttributes<LogifyIgnoreAttribute>(true);
                return ShouldIgnoreExceptionByAttributes(attributes);
#else
                object[] attributes = exceptionType.GetCustomAttributes(true);
                return ShouldIgnoreExceptionByAttributes(attributes);
#endif
            }
            catch {
                return ShouldIgnoreResult.Unknown;
            }
        }


        ShouldIgnoreResult ShouldIgnoreExceptionByStack(Exception ex) {
            try {
                StackTrace trace = new StackTrace(ex, false);
#if NETSTANDARD
                StackFrame[] frames = trace.GetFrames();
                if (frames != null && frames.Length > 0) {
                    int count = frames.Length;
                    for (int i = count - 1; i >= 0; i--) { // AM: walk in backward order to process nested calls last
                        StackFrame frame = frames[i];
                        ShouldIgnoreResult result = ShouldIgnoreExceptionByStackFrame(ex, frame);
                        if (result != ShouldIgnoreResult.Unknown)
                            return result;
                    }
                }
#else
                int count = trace.FrameCount;
                for (int i = count - 1; i >= 0; i--) { // AM: walk in backward order to process nested calls last
                    StackFrame frame = trace.GetFrame(i);
                    ShouldIgnoreResult result = ShouldIgnoreExceptionByStackFrame(ex, frame);
                    if (result != ShouldIgnoreResult.Unknown)
                        return result;
                }
#endif
                return ShouldIgnoreResult.Unknown;
            }
            catch {
                return ShouldIgnoreResult.Unknown;
            }
        }

        ShouldIgnoreResult ShouldIgnoreExceptionByStackFrame(Exception ex, StackFrame frame) {
            try {
                MethodBase method = frame.GetMethod();
                if (method == null)
                    return ShouldIgnoreResult.Unknown;
#if NETSTANDARD
                IEnumerable<LogifyIgnoreAttribute> attributes = method.GetCustomAttributes<LogifyIgnoreAttribute>(true);
                return ShouldIgnoreExceptionByAttributes(attributes);
#else
                object[] attributes = method.GetCustomAttributes(true);
                return ShouldIgnoreExceptionByAttributes(attributes);
#endif
            }
            catch {
                return ShouldIgnoreResult.Unknown;
            }
        }

#if NETSTANDARD
        ShouldIgnoreResult ShouldIgnoreExceptionByAttributes(IEnumerable<LogifyIgnoreAttribute> attributes) {
            if (attributes == null)
                return ShouldIgnoreResult.Unknown;

            try {
                foreach (LogifyIgnoreAttribute attribute in attributes) {
                    ShouldIgnoreResult result = ShouldIgnoreExceptionByAttribute(attribute);
                    if (result != ShouldIgnoreResult.Unknown)
                        return result;
                }
                return ShouldIgnoreResult.Unknown;
            }
            catch {
                return ShouldIgnoreResult.Unknown;
            }
        }
        ShouldIgnoreResult ShouldIgnoreExceptionByAttribute(LogifyIgnoreAttribute attribute) {
            if (attribute == null)
                return ShouldIgnoreResult.Unknown;
            return (attribute.Ignore) ? ShouldIgnoreResult.Ignore : ShouldIgnoreResult.Process;
        }
#else
        ShouldIgnoreResult ShouldIgnoreExceptionByAttributes(object[] attributes) {
            try {
                if (attributes == null || attributes.Length <= 0)
                    return ShouldIgnoreResult.Unknown;

                int count = attributes.Length;
                for (int i = 0; i < count; i++) {
                    ShouldIgnoreResult result = ShouldIgnoreExceptionByAttribute(attributes[i]);
                    if (result != ShouldIgnoreResult.Unknown)
                        return result;
                }
                return ShouldIgnoreResult.Unknown;

            }
            catch {
                return ShouldIgnoreResult.Unknown;
            }
        }
        ShouldIgnoreResult ShouldIgnoreExceptionByAttribute(object attribute) {
            try {
                if (attribute == null)
                    return ShouldIgnoreResult.Unknown;

                LogifyIgnoreAttribute ignoreAttribute = attribute as LogifyIgnoreAttribute;
                if (ignoreAttribute != null)
                    return (ignoreAttribute.Ignore) ? ShouldIgnoreResult.Ignore : ShouldIgnoreResult.Process;

                string attributeTypeName = attribute.GetType().Name;
                if (String.Compare(attributeTypeName, "LogifyIgnoreExceptionAttribute", StringComparison.InvariantCultureIgnoreCase) == 0) {
                    PropertyInfo property = attribute.GetType().GetProperty("Ignore");
                    if (property != null && property.PropertyType == typeof(bool)) {
                        return ((bool)property.GetValue(attribute, null)) ? ShouldIgnoreResult.Ignore : ShouldIgnoreResult.Process;
                    }
                }

                return ShouldIgnoreResult.Unknown;
            }
            catch {
                return ShouldIgnoreResult.Unknown;
            }
        }
#endif
#endregion
        }
}