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

    public static class ClientConfigHelper {
        public static ILogifyClientConfiguration GetConfig(LogifyClientBase client) {
            return client.Config;
        }
    }

    public class IgnorePropertiesInfoConfig {
        public string IgnoreFormFields { get; set; }
        public string IgnoreHeaders { get; set; }
        public string IgnoreCookies { get; set; }
        public string IgnoreServerVariables { get; set; }
        public bool IgnoreRequestBody { get; set; }
    }
}
