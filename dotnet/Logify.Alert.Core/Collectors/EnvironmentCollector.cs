using System;

namespace DevExpress.Logify.Core.Internal {
    public class EnvironmentCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            /*
            // AM: Due legal reasons need explicit user action to send security-critial data.
            //     Users should use custom data instead.
            logger.BeginWriteObje ct("environment");
            try {
                logger.WriteValue("machineName", Environment.MachineName);
                logger.WriteValue("userDomainName", Environment.UserDomainName);
                logger.WriteValue("userName", Environment.UserName);
            }
            finally {
                logger.EndWriteObject("environment");
            }
            */
        }
    }
}