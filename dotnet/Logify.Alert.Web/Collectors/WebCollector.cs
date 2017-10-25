using System;
using System.Web;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WebExceptionCollector : CompositeInfoCollector {
        LogifyAppInfoCollector logifyAppInfoCollector;

        public WebExceptionCollector(ILogifyClientConfiguration config, Platform platform) : base(config) {
            Collectors.Add(new DevelopementPlatformCollector(platform));
        }

        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string UserId { get; set; }

        LogifyAppInfoCollector LogifyAppInfoCollector {
            get {
                if (logifyAppInfoCollector == null)
                    logifyAppInfoCollector = new LogifyAppInfoCollector(new WebApplicationCollector());
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
            Collectors.Add(new LogifyReportGenerationDateTimeCollector());
            Collectors.Add(LogifyAppInfoCollector);
            //Collectors.Add(new DevelopementPlatformCollector(Platform.ASP)); // added in constuctor
            Collectors.Add(new WebApplicationCollector());
            Collectors.Add(new ExceptionObjectInfoCollector(config));

            try {
                HttpContext context = HttpContext.Current;
                if (context != null) {
                    try {
                        if (context.Request != null)
                            Collectors.Add(new RequestCollector(context.Request));
                    }
                    catch {
                    }
                    try {
                        if (context.Response != null)
                            Collectors.Add(new ResponseCollector(context.Response));
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
        }
    }
}