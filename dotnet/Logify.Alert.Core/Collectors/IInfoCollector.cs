using System;

namespace DevExpress.Logify.Core.Internal {
    public interface IInfoCollector {
        void Process(Exception ex, ILogger logger);
    }
}
