using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.WPF {
    public class WPFExceptionCollector : CompositeInfoCollector {
        readonly LogifyAppInfoCollector logifyAppInfoCollector = new LogifyAppInfoCollector(new WpfApplicationCollector());

        public WPFExceptionCollector(ILogifyClientConfiguration config) : base(config) {
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
            if (config.CollectMiniDump)
                Collectors.Add(new MiniDumpCollector());
            Collectors.Add(new LogifyProtocolVersionCollector());
            Collectors.Add(new LogifyReportGenerationDateTimeCollector());
            Collectors.Add(logifyAppInfoCollector);
            Collectors.Add(new DevelopementPlatformCollector(Platform.WPF));
            Collectors.Add(new WpfApplicationCollector());

            //Spies.Add(new AppDispatcherCollector());
            //Spies.Add(new DispatcherCollector());
            //Spies.Add(new TaskShedulerCollector());

            Collectors.Add(new WPFEnvironmentCollector(config)); // Environment info should go first, it may be used for further exception processing
            Collectors.Add(new ExceptionObjectInfoCollector(config));
            if (config.CollectMiniDump)
                Collectors.Add(new DeferredMiniDumpCollector());
        }
    }
}
