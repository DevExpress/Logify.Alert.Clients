using System;
using System.Diagnostics;
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
                object[] attributes = exceptionType.GetCustomAttributes(true);
                return ShouldIgnoreExceptionByAttributes(attributes);
            }
            catch {
                return ShouldIgnoreResult.Unknown;
            }
        }


        ShouldIgnoreResult ShouldIgnoreExceptionByStack(Exception ex) {
            try {
                StackTrace trace = new StackTrace(ex);
                int count = trace.FrameCount;
                for (int i = count - 1; i >= 0; i--) { // AM: walk in backward order to process nested calls last
                    StackFrame frame = trace.GetFrame(i);
                    ShouldIgnoreResult result = ShouldIgnoreExceptionByStackFrame(ex, frame);
                    if (result != ShouldIgnoreResult.Unknown)
                        return result;
                }
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

                object[] attributes = method.GetCustomAttributes(true);
                return ShouldIgnoreExceptionByAttributes(attributes);
            }
            catch {
                return ShouldIgnoreResult.Unknown;
            }
        }

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
        #endregion
    }
}