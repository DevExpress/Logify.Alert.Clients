using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class ConsoleExceptionCollector : RootInfoCollector {
        public ConsoleExceptionCollector(LogifyCollectorContext context) : base(context) {
        }

        protected override void RegisterCollectors(LogifyCollectorContext context) {
            if (context.Config != null && context.Config.CollectMiniDump)
                Collectors.Add(new MiniDumpCollector());
            Collectors.Add(new DevelopementPlatformCollector(Platform.WIN));
            Collectors.Add(new ConsoleApplicationCollector());
            Collectors.Add(new ConsoleEnvironmentCollector(context)); // Environment info should go first, it may be used for further exception processing
            if (context.Config != null && context.Config.CollectMiniDump)
                Collectors.Add(new DeferredMiniDumpCollector());
        }
    }
}
