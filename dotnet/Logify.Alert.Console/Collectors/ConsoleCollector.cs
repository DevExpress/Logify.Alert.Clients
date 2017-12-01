using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class ConsoleExceptionCollector : RootInfoCollector {
        public ConsoleExceptionCollector(ILogifyClientConfiguration config) : base(config) {
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            if (config.CollectMiniDump)
                Collectors.Add(new MiniDumpCollector());
            Collectors.Add(new DevelopementPlatformCollector(Platform.WIN));
            Collectors.Add(new ConsoleApplicationCollector());
            Collectors.Add(new ConsoleEnvironmentCollector(config)); // Environment info should go first, it may be used for further exception processing
            if (config.CollectMiniDump)
                Collectors.Add(new DeferredMiniDumpCollector());
        }
    }
}
