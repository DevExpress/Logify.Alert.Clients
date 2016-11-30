using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.WPF {
    public class WPFExceptionCollectorFactory : IInfoCollectorFactory {
        public IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config) {
            return new WPFExceptionCollector(config);
        }
    }
}