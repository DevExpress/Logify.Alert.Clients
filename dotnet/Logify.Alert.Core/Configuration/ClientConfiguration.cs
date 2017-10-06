namespace DevExpress.Logify.Core {
    public interface ILogifyClientConfiguration {
        bool CollectScreenshot { get; set; }
        bool CollectMiniDump { get; set; }
        bool CollectBreadcrumbs { get; set; }
        int BreadcrumbsMaxCount { get; set; }
    }
    class DefaultClientConfiguration : ILogifyClientConfiguration {
        public DefaultClientConfiguration() {
            this.BreadcrumbsMaxCount = 1000;
        }
        public bool CollectScreenshot { get; set; }
        public bool CollectMiniDump { get; set; }
        public bool CollectBreadcrumbs { get; set; }
        public int BreadcrumbsMaxCount { get; set; }
    }

    public static class ClientConfigHelper {
        public static ILogifyClientConfiguration GetConfig(LogifyClientBase client) {
            return client.Config;
        }
    }
}
