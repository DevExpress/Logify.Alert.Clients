using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class ConsoleExceptionCollectorFactory : IInfoCollectorFactory {
        public IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config) {
            return new ConsoleExceptionCollector(config);
        }
    }
}