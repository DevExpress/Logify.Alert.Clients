using System;
using System.Globalization;

namespace DevExpress.Logify.Core.Internal {
    public class LogifyHardwareIdCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            string hardwareId = HardwareId.Get();
            if (!String.IsNullOrEmpty(hardwareId))
                logger.WriteValue("logifyHardwareId", hardwareId);
        }
    }
}
