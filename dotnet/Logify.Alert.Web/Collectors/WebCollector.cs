using System;
using System.Web;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WebExceptionCollector : RootInfoCollector {
        public WebExceptionCollector(LogifyCollectorContext context, Platform platform) : base(context) {
            Collectors.Add(new DevelopementPlatformCollector(platform));
        }

        protected override void RegisterCollectors(LogifyCollectorContext context) {
            IgnorePropertiesInfoConfig ignoreConfig = context.Config != null ? context.Config.IgnoreConfig : null;
            if (ignoreConfig == null)
                ignoreConfig = new IgnorePropertiesInfoConfig();

            //Collectors.Add(new DevelopementPlatformCollector(Platform.ASP)); // added in constuctor

            try {
                WebLogifyCollectorContext webContext = context as WebLogifyCollectorContext;
                HttpContext httpContext = webContext != null ? webContext.HttpContext : null;
                Collectors.Add(new WebApplicationCollector(httpContext));
                if (httpContext != null) {
                    try {
                        if (httpContext.Request != null)
                            Collectors.Add(new RequestCollector(httpContext.Request, ignoreConfig));
                    }
                    catch {
                    }
                    try {
                        if (httpContext.Response != null)
                            Collectors.Add(new ResponseCollector(httpContext.Response, ignoreConfig));
                    }
                    catch {
                    }
                    try {
                        if (httpContext.ApplicationInstance != null && httpContext.ApplicationInstance.Modules != null)
                            Collectors.Add(new ModulesCollector(httpContext.ApplicationInstance.Modules));
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
            Collectors.Add(new MemoryCollector(context));
            Collectors.Add(new FrameworkVersionsCollector());
            Collectors.Add(new EnvironmentCollector());
        }
    }
}