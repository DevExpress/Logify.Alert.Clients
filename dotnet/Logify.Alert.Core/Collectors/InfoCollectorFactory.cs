using System;

namespace DevExpress.Logify.Core.Internal {
    public interface IInfoCollectorFactory {
        IInfoCollector CreateDefaultCollector(ILogifyClientConfiguration config);
    }
}
