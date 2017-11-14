using System;

namespace DevExpress.Logify.Core.Internal {
    public class EnvironmentCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("environment");
            try {
                logger.WriteValue("machineName", Environment.MachineName);
                logger.WriteValue("userDomainName", Environment.UserDomainName);
                logger.WriteValue("userName", Environment.UserName);
            }
            finally {
                logger.EndWriteObject("environment");
            }
        }
    }
}