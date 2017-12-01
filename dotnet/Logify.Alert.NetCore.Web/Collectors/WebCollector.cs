using System;
using DevExpress.Logify.Core;
using Microsoft.AspNetCore.Http;

namespace DevExpress.Logify.Core.Internal {
    public class NetCoreWebExceptionCollector : RootInfoCollector {
        public NetCoreWebExceptionCollector(ILogifyClientConfiguration config, Platform platform) : base(config) {
            Collectors.Add(new DevelopementPlatformCollector(platform));
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            IgnorePropertiesInfoConfig ignoreConfig = config.IgnoreConfig;
            if (ignoreConfig == null)
                ignoreConfig = new IgnorePropertiesInfoConfig();

            //Collectors.Add(new DevelopementPlatformCollector(Platform.ASP)); // added in constuctor
            Collectors.Add(new NetCoreWebApplicationCollector());

            HttpContext context = LogifyHttpContext.Current;
            if (context != null) {
                if (context.Request != null)
                    Collectors.Add(new RequestCollector(context.Request, ignoreConfig));
                if (context.Response != null)
                    Collectors.Add(new ResponseCollector(context.Response, ignoreConfig));
                //if (context.ApplicationInstance != null && context.ApplicationInstance.Modules != null)
                //    Collectors.Add(new ModulesCollector(context.ApplicationInstance.Modules));
            }
            Collectors.Add(new OperatingSystemCollector());
            //Collectors.Add(new VirtualMachineCollector());
            Collectors.Add(new DebuggerCollector());
            //Collectors.Add(new MemoryCollector(config));
            //Collectors.Add(new FrameworkVersionsCollector());
        }
    }
}