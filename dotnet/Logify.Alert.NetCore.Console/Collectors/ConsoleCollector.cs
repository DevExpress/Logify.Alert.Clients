using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class NetCoreConsoleExceptionCollector : RootInfoCollector {
        public NetCoreConsoleExceptionCollector(ILogifyClientConfiguration config) : base(config) {
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            if (config.CollectMiniDump)
                Collectors.Add(new MiniDumpCollector());
            Collectors.Add(new DevelopementPlatformCollector(Platform.NETCORE_CONSOLE)); // added in constuctor
            Collectors.Add(new NetCoreConsoleApplicationCollector());

            //HttpContext context = LogifyHttpContext.Current;
            //if (context != null) {
            //    if (context.Request != null)
            //        Collectors.Add(new RequestCollector(context.Request));
            //    if (context.Response != null)
            //        Collectors.Add(new ResponseCollector(context.Response));
            //    //if (context.ApplicationInstance != null && context.ApplicationInstance.Modules != null)
            //    //    Collectors.Add(new ModulesCollector(context.ApplicationInstance.Modules));
            //}
            Collectors.Add(new OperatingSystemCollector());
            //Collectors.Add(new VirtualMachineCollector());
            Collectors.Add(new DebuggerCollector());
            //Collectors.Add(new MemoryCollector(config));
            //Collectors.Add(new FrameworkVersionsCollector());
            if (config.CollectMiniDump)
                Collectors.Add(new DeferredMiniDumpCollector());
        }
    }
}