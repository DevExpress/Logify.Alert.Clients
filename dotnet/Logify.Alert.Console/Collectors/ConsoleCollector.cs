using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Console {
    public class ConsoleExceptionCollector : CompositeInfoCollector {
        readonly LogifyAppInfoCollector logifyAppInfoCollector = new LogifyAppInfoCollector(new ConsoleApplicationCollector());

        public ConsoleExceptionCollector(ILogifyClientConfiguration config): base(config) {
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
            if (config.MakeMiniDump)
                Collectors.Add(new MiniDumpCollector());
            Collectors.Add(new LogifyProtocolVersionCollector());
            Collectors.Add(new LogifyReportGenerationDateTimeCollector());
            Collectors.Add(logifyAppInfoCollector);
            Collectors.Add(new DevelopementPlatformCollector(Platform.WIN));
            Collectors.Add(new ConsoleApplicationCollector());
            Collectors.Add(new ConsoleEnvironmentCollector(config)); // Environment info should go first, it may be used for further exception processing
            Collectors.Add(new ExceptionObjectInfoCollector(config));
        }
    }
}
