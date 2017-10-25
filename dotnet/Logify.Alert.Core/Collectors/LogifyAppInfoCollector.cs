using System;

namespace DevExpress.Logify.Core.Internal {
    public interface ILogifyAppInfo {
        string AppName { get; }
        string AppVersion { get; }
        string UserId { get; }
    }
    public class LogifyAppInfoCollector : IInfoCollector {
        readonly ILogifyAppInfo appInfo;

        public LogifyAppInfoCollector(ILogifyAppInfo appInfo) {
            this.appInfo = appInfo;
        }

        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string UserId { get; set; }

        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("logifyApp");
            try {
                logger.WriteValue("name", String.IsNullOrEmpty(AppName) ? appInfo.AppName : AppName);
                logger.WriteValue("version", String.IsNullOrEmpty(AppVersion) ? appInfo.AppVersion : AppVersion);
                logger.WriteValue("userId", String.IsNullOrEmpty(UserId) ? appInfo.UserId : UserId);
            }
            finally {
                logger.EndWriteObject("logifyApp");
            }
        }
    }
}