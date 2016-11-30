using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Win {
    public class WinFormsExceptionCollectorFactory : IInfoCollectorFactory {
        public IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config) {
            return new WinFormsExceptionCollector(config);
        }
    }
}