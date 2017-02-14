using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.NetCore.Console {
    public class NetCoreConsoleExceptionCollectorFactory : IInfoCollectorFactory {
        public IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config) {
            return new NetCoreConsoleExceptionCollector(config);
        }
    }
}