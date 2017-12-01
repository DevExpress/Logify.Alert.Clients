using System;
using System.Web;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WebExceptionCollector : RootInfoCollector {
        public WebExceptionCollector(ILogifyClientConfiguration config, Platform platform) : base(config) {
            Collectors.Add(new DevelopementPlatformCollector(platform));
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            IgnorePropertiesInfoConfig ignoreConfig = config.IgnoreConfig;
            if (ignoreConfig == null)
                ignoreConfig = new IgnorePropertiesInfoConfig();

            //Collectors.Add(new DevelopementPlatformCollector(Platform.ASP)); // added in constuctor
            Collectors.Add(new WebApplicationCollector());

            try {
                HttpContext context = HttpContext.Current;
                if (context != null) {
                    try {
                        if (context.Request != null)
                            Collectors.Add(new RequestCollector(context.Request, ignoreConfig));
                    }
                    catch {
                    }
                    try {
                        if (context.Response != null)
                            Collectors.Add(new ResponseCollector(context.Response, ignoreConfig));
                    }
                    catch {
                    }
                    try {
                        if (context.ApplicationInstance != null && context.ApplicationInstance.Modules != null)
                            Collectors.Add(new ModulesCollector(context.ApplicationInstance.Modules));
                    }
                    catch {
                    }
                }
            }
            catch {
            }
            Collectors.Add(new OperatingSystemCollector());
            Collectors.Add(new VirtualMachineCollector());
            Collectors.Add(new DebuggerCollector());
            Collectors.Add(new MemoryCollector(config));
            Collectors.Add(new FrameworkVersionsCollector());
            Collectors.Add(new EnvironmentCollector());
        }
    }
}