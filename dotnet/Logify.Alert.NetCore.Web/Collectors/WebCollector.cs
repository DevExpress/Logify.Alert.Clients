using System;
using DevExpress.Logify.Core;
using Microsoft.AspNetCore.Http;

namespace DevExpress.Logify.Core.Internal {
    public class NetCoreWebExceptionCollector : RootInfoCollector {
        public NetCoreWebExceptionCollector(LogifyCollectorContext context, Platform platform) : base(context) {
            Collectors.Add(new DevelopementPlatformCollector(platform));
        }

        protected override void RegisterCollectors(LogifyCollectorContext context) {
            IgnorePropertiesInfoConfig ignoreConfig = context.Config != null ? context.Config.IgnoreConfig : null;
            if (ignoreConfig == null)
                ignoreConfig = new IgnorePropertiesInfoConfig();

            //Collectors.Add(new DevelopementPlatformCollector(Platform.ASP)); // added in constructor

            WebLogifyCollectorContext webContext = context as WebLogifyCollectorContext;
            HttpContext httpContext = webContext != null ? webContext.HttpContext : null;
            Collectors.Add(new NetCoreWebApplicationCollector(httpContext));
            if (httpContext != null) {
                if (httpContext.Request != null)
                    Collectors.Add(new RequestCollector(httpContext.Request, ignoreConfig));
                if (httpContext.Response != null)
                    Collectors.Add(new ResponseCollector(httpContext.Response, ignoreConfig));
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