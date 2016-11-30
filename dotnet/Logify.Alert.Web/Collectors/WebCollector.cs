using System;
using System.Web;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Web {
    public class WebExceptionCollector : CompositeInfoCollector {
        readonly LogifyAppInfoCollector logifyAppInfoCollector = new LogifyAppInfoCollector(new WebApplicationCollector());

        public WebExceptionCollector(ILogifyClientConfiguration config, Platform platform) : base(config) {
            Collectors.Add(new DevelopementPlatformCollector(platform));
        }

        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string UserId { get; set; }

        public override void Process(Exception ex, ILogger logger) {
            logifyAppInfoCollector.AppName = this.AppName;
            logifyAppInfoCollector.AppVersion = this.AppVersion;
            logifyAppInfoCollector.UserId = this.UserId;
            base.Process(ex, logger);
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            HttpContext context = HttpContext.Current;
            Collectors.Add(new LogifyProtocolVersionCollector());
            Collectors.Add(logifyAppInfoCollector);
            //Collectors.Add(new DevelopementPlatformCollector(Platform.ASP)); // added in constuctor
            Collectors.Add(new WebApplicationCollector());
            Collectors.Add(new ExceptionObjectInfoCollector(config));
            Collectors.Add(new RequestCollector(context.Request));
            Collectors.Add(new ResponseCollector(context.Response));
            Collectors.Add(new ModulesCollector(context.ApplicationInstance.Modules));
            Collectors.Add(new OperatingSystemCollector());
            Collectors.Add(new VirtualMachineCollector());
            Collectors.Add(new DebuggerCollector());
            Collectors.Add(new MemoryCollector(config));
            Collectors.Add(new FrameworkVersionsCollector());
        }
    }
}