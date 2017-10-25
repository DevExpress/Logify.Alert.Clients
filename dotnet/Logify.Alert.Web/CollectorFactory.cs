using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    class WebDefaultExceptionCollectorFactory : IInfoCollectorFactory {

        readonly Platform platform;

        public WebDefaultExceptionCollectorFactory(Platform platform) {
            this.platform = platform;
        }

        public IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config) {
            return new WebExceptionCollector(config, this.platform);
        }
    }
}