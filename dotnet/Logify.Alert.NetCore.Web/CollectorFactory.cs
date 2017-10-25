using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    class NetCoreWebDefaultExceptionCollectorFactory : IInfoCollectorFactory {

        readonly Platform platform;

        public NetCoreWebDefaultExceptionCollectorFactory(Platform platform) {
            this.platform = platform;
        }

        public IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config) {
            return new NetCoreWebExceptionCollector(config, this.platform);
        }
    }
}