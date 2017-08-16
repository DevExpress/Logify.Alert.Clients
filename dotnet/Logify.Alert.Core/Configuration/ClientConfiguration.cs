namespace DevExpress.Logify.Core {
    public interface ILogifyClientConfiguration {
        bool CollectScreenshot { get; set; }
        bool CollectMiniDump { get; set; }
    }
    class DefaultClientConfiguration : ILogifyClientConfiguration {
        public bool CollectScreenshot { get; set; }
        public bool CollectMiniDump { get; set; }
    }

    public static class ClientConfigHelper {
        public static ILogifyClientConfiguration GetConfig(LogifyClientBase client) {
            return client.Config;
        }
    }
}
