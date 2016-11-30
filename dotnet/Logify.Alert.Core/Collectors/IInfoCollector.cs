using System;

namespace DevExpress.Logify.Core {
    public interface IInfoCollector {
        void Process(Exception ex, ILogger logger);
    }
}
