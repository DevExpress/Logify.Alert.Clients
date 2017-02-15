using System;
using DevExpress.Logify.Core;
using Microsoft.AspNetCore.Http;

namespace DevExpress.Logify.Web {
    public class NetCoreWebExceptionCollector : CompositeInfoCollector {
        LogifyAppInfoCollector logifyAppInfoCollector;

        public NetCoreWebExceptionCollector(ILogifyClientConfiguration config, Platform platform) : base(config) {
            Collectors.Add(new DevelopementPlatformCollector(platform));
        }

        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string UserId { get; set; }

        LogifyAppInfoCollector LogifyAppInfoCollector {
            get {
                if (logifyAppInfoCollector == null)
                    logifyAppInfoCollector = new LogifyAppInfoCollector(new NetCoreWebApplicationCollector());
                return logifyAppInfoCollector;
            }
        }

        public override void Process(Exception ex, ILogger logger) {
            LogifyAppInfoCollector.AppName = this.AppName;
            LogifyAppInfoCollector.AppVersion = this.AppVersion;
            LogifyAppInfoCollector.UserId = this.UserId;
            base.Process(ex, logger);
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            Collectors.Add(new LogifyProtocolVersionCollector());
            Collectors.Add(LogifyAppInfoCollector);
            //Collectors.Add(new DevelopementPlatformCollector(Platform.ASP)); // added in constuctor
            Collectors.Add(new NetCoreWebApplicationCollector());
            Collectors.Add(new ExceptionObjectInfoCollector(config));

            HttpContext context = LogifyHttpContext.Current;
            if (context != null) {
                if (context.Request != null)
                    Collectors.Add(new RequestCollector(context.Request));
                if (context.Response != null)
                    Collectors.Add(new ResponseCollector(context.Response));
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