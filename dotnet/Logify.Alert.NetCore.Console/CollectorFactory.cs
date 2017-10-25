using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class NetCoreConsoleExceptionCollectorFactory : IInfoCollectorFactory {
        public IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config) {
            return new NetCoreConsoleExceptionCollector(config);
        }
    }
}