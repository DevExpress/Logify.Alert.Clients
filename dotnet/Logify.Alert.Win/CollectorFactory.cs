using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WinFormsExceptionCollectorFactory : IInfoCollectorFactory {
        public IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config) {
            return new WinFormsExceptionCollector(config);
        }
    }
}