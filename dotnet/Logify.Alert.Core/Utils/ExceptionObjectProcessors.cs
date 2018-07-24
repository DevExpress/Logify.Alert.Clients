using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevExpress.Logify.Core.Internal {
    internal interface ISpecificExceptionProcessor {
        Exception PreProcess(Exception ex);
        Exception GetNextException(Exception ex);
    }
    internal class ExceptionProcessors : ISpecificExceptionProcessor {
        ISpecificExceptionProcessor instance;
        public ExceptionProcessors(Exception ex) {
            if (ex is AggregateException)
                instance = new AggregateExceptionProcessor();
            else
                instance = new DefaultExceptionProcessor();
        }
        public Exception GetNextException(Exception ex) {
            return instance.GetNextException(ex);
        }

        public Exception PreProcess(Exception ex) {
            return instance.PreProcess(ex);
        }
    }
    internal class DefaultExceptionProcessor : ISpecificExceptionProcessor {
        public virtual Exception GetNextException(Exception ex) {
            return ex.InnerException;
        }

        public virtual Exception PreProcess(Exception ex) { return ex; }
    }
    class ExceptionObjectKeys {
        public const string InnerExceptionNumber = "innerExceptionNum";
    }
    internal class AggregateExceptionProcessor : ISpecificExceptionProcessor {
        IEnumerator<Exception> innerExceptionsEnumerator;
        int innerExceptionNum;

        public Exception PreProcess(Exception ex) {
            if (ex is AggregateException) {
                innerExceptionNum = 0;
                ex = ((AggregateException)ex).Flatten();
                ex.Data[ExceptionObjectKeys.InnerExceptionNumber] = innerExceptionNum;
                innerExceptionsEnumerator = ((AggregateException)ex).InnerExceptions.GetEnumerator();
                innerExceptionsEnumerator.MoveNext();
            }
            return ex;
        }
        public Exception GetNextException(Exception ex) {
            if (ex.InnerException == null && innerExceptionsEnumerator != null) {
                if (innerExceptionsEnumerator.MoveNext()) {
                    innerExceptionNum++;
                    var current = innerExceptionsEnumerator.Current;
                    if (current != null)
                        current.Data[ExceptionObjectKeys.InnerExceptionNumber] = innerExceptionNum;
                    return current;
                }
            }
            if (ex.InnerException != null)
                ex.InnerException.Data[ExceptionObjectKeys.InnerExceptionNumber] = innerExceptionNum;
            return ex.InnerException;
        }
    }
}
