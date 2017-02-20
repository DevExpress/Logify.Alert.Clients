using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Console {
    public class ConsoleExceptionCollectorFactory : IInfoCollectorFactory {
        public IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config) {
            return new ConsoleExceptionCollector(config);
        }
    }
}