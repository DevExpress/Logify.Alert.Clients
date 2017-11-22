using System.Collections.Generic;

namespace DevExpress.Logify.Core.Internal {
    public interface ILogifyClientConfiguration {
        bool CollectScreenshot { get; set; }
        bool CollectMiniDump { get; set; }
        bool CollectBreadcrumbs { get; set; }
        int BreadcrumbsMaxCount { get; set; }

        IgnorePropertiesInfoConfig IgnoreConfig { get; }
    }
    class DefaultClientConfiguration : ILogifyClientConfiguration {
        public DefaultClientConfiguration() {
            this.BreadcrumbsMaxCount = 1000;
            this.IgnoreConfig = new IgnorePropertiesInfoConfig();
        }
        public bool CollectScreenshot { get; set; }
        public bool CollectMiniDump { get; set; }
        public bool CollectBreadcrumbs { get; set; }
        public int BreadcrumbsMaxCount { get; set; }
        public IgnorePropertiesInfoConfig IgnoreConfig { get; private set; }
    }

    public class IgnorePropertiesInfoConfig {
        public string IgnoreFormFields { get; set; }
        public string IgnoreHeaders { get; set; }
        public string IgnoreCookies { get; set; }
        public string IgnoreServerVariables { get; set; }
        public bool IgnoreRequestBody { get; set; }
    }

    public class LogifyAlertConfiguration {
        public LogifyAlertConfiguration() {
            this.BreadcrumbsMaxCount = 1000;
            this.OfflineReportsCount = 100;
        }
        public string ServiceUrl { get; set; }
        //public string MiniDumpServiceUrl { get; set; }
        public string ApiKey { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public bool ConfirmSend { get; set; }
        public bool OfflineReportsEnabled { get; set; }
        public string OfflineReportsDirectory { get; set; }
        public int OfflineReportsCount { get; set; }
        public Dictionary<string, string> CustomData { get; set; }
        public bool CollectMiniDump { get; set; }
        public bool CollectScreenshot { get; set; }
        public bool CollectBreadcrumbs { get; set; }
        public int BreadcrumbsMaxCount { get; set; }
        public string IgnoreFormFields { get; set; }
        public string IgnoreHeaders { get; set; }
        public string IgnoreCookies { get; set; }
        public string IgnoreServerVariables { get; set; }
        public bool IgnoreRequestBody { get; set; }
    }
}
