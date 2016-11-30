using System;

namespace DevExpress.Logify.Core {
    public interface IInfoCollectorFactory {
        IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config);
    }
}
