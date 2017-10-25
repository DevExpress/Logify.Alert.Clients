using System;
using System.Diagnostics;

namespace DevExpress.Logify.Core.Internal {
    public class DebuggerCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            try {
                logger.BeginWriteObject("debugger");
                try {
                    logger.WriteValue("isAttached", Debugger.IsAttached);
                    //etc
                }
                finally {
                    logger.EndWriteObject("debugger");
                }
            }
            catch {
            }
        }
    }
}