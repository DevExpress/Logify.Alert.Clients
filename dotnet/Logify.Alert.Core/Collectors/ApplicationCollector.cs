using System;

namespace DevExpress.Logify.Core {
    public abstract class ApplicationCollectorBase : IInfoCollector, ILogifyAppInfo {

        public abstract string AppName { get; }
        public abstract string AppVersion { get; }
        public abstract string UserId { get; }

        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("app");
            try {
                logger.WriteValue("name", AppName);
                logger.WriteValue("version", AppVersion);
#if !NETSTANDARD
                logger.WriteValue("is64bit", Environment.Is64BitProcess);
#endif
                SerializeCurrentDomainInfo(ex, logger, "currentDomain");
            }
            finally {
                logger.EndWriteObject("app");
            }
        }

        protected abstract void SerializeCurrentDomainInfo(Exception ex, ILogger logger, string name);
    }
#if NETSTANDARD
    public abstract class ApplicationCollector : ApplicationCollectorBase {
        protected override void SerializeCurrentDomainInfo(Exception ex, ILogger logger, string name) {
            //try {
            //    AppDomainCollector domain = new AppDomainCollector(AppDomain.CurrentDomain, name);
            //    domain.Process(ex, logger);
            //}
            //catch {
            //}
        }
    }
#else
    public abstract class ApplicationCollector : ApplicationCollectorBase {
        protected override void SerializeCurrentDomainInfo(Exception ex, ILogger logger, string name) {
            try {
                AppDomainCollector domain = new AppDomainCollector(AppDomain.CurrentDomain, name);
                domain.Process(ex, logger);
            }
            catch {
            }
        }
    }
#endif
}