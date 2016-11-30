using System;

namespace DevExpress.Logify.Core {
    public class DevelopementPlatformCollector : IInfoCollector {

        readonly Platform platform;

        public DevelopementPlatformCollector(Platform platform) {
            this.platform = platform;
        }

        public virtual void Process(Exception ex, ILogger logger) {
            logger.WriteValue("devPlatform", "dotnet");
            logger.WriteValue("platform", this.platform.ToString());
        }
    }
}
