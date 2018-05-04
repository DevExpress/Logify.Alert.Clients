using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WPFExceptionCollector : RootInfoCollector {
        public WPFExceptionCollector(LogifyCollectorContext context) : base(context) {
        }


        protected override void RegisterCollectors(LogifyCollectorContext context) {
            if (context.Config != null && context.Config.CollectMiniDump)
                Collectors.Add(new MiniDumpCollector());
            Collectors.Add(new DevelopementPlatformCollector(Platform.WPF));
            Collectors.Add(new WpfApplicationCollector());

            //Spies.Add(new AppDispatcherCollector());
            //Spies.Add(new DispatcherCollector());
            //Spies.Add(new TaskShedulerCollector());

            Collectors.Add(new WPFEnvironmentCollector(context)); // Environment info should go first, it may be used for further exception processing
            if (context.Config != null && context.Config.CollectMiniDump)
                Collectors.Add(new DeferredMiniDumpCollector());
        }
    }
}
