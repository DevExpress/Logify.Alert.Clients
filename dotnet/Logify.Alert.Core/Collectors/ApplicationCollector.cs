using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevExpress.Logify.Core {
    public abstract class ApplicationCollector : IInfoCollector, ILogifyAppInfo {

        public abstract string AppName { get; }
        public abstract string AppVersion { get; }
        public abstract string UserId { get; }

        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("app");
            try {
                logger.WriteValue("name", AppName);
                logger.WriteValue("version", AppVersion);
                logger.WriteValue("is64bit", Environment.Is64BitProcess);

                try {
                    AppDomainCollector domain = new AppDomainCollector(AppDomain.CurrentDomain, "currentDomain");
                    domain.Process(ex, logger);
                }
                catch {
                }
            }
            finally {
                logger.EndWriteObject("app");
            }
        }

    }
}