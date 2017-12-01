using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WPFExceptionCollector : RootInfoCollector {
        public WPFExceptionCollector(ILogifyClientConfiguration config) : base(config) {
        }


        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            if (config.CollectMiniDump)
                Collectors.Add(new MiniDumpCollector());
            Collectors.Add(new DevelopementPlatformCollector(Platform.WPF));
            Collectors.Add(new WpfApplicationCollector());

            //Spies.Add(new AppDispatcherCollector());
            //Spies.Add(new DispatcherCollector());
            //Spies.Add(new TaskShedulerCollector());

            Collectors.Add(new WPFEnvironmentCollector(config)); // Environment info should go first, it may be used for further exception processing
            if (config.CollectMiniDump)
                Collectors.Add(new DeferredMiniDumpCollector());
        }
    }
}
