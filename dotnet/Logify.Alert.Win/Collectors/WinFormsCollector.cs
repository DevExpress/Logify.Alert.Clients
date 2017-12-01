using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WinFormsExceptionCollector : RootInfoCollector {
        public WinFormsExceptionCollector(ILogifyClientConfiguration config) : base(config) {
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            if (config.CollectMiniDump)
                Collectors.Add(new MiniDumpCollector());
            Collectors.Add(new DevelopementPlatformCollector(Platform.WIN));
            Collectors.Add(new WinFormsApplicationCollector());
            Collectors.Add(new WinFormsEnvironmentCollector(config)); // Environment info should go first, it may be used for further exception processing
            if (config.CollectMiniDump)
                Collectors.Add(new DeferredMiniDumpCollector());
        }
    }
}
