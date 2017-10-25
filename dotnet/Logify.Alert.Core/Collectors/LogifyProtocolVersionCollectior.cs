using System;

namespace DevExpress.Logify.Core.Internal {
    public class LogifyProtocolVersionCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.WriteValue("logifyProtocolVersion", AssemblyInfo.Version);
        }
    }
}
