namespace DevExpress.Logify {
    public interface ILogifyClientConfiguration {
        bool TakeScreenshot { get; }
        bool MakeMiniDump { get; }
    }
    public class DefaultClientConfiguration : ILogifyClientConfiguration {
        public bool TakeScreenshot { get; set; }
        public bool MakeMiniDump { get; set; }
    }
}
