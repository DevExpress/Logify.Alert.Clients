using System;
using System.Web;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Web {
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