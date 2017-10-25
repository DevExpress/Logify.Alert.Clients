namespace DevExpress.Logify.Core.Internal {
    public class ExceptionLoggerFactory {
        static ExceptionLoggerFactory instance;

        public IInfoCollectorFactory PlatformCollectorFactory { get; set; }
        public IExceptionReportSender PlatformReportSender { get; set; }
        public IExceptionIgnoreDetection PlatformIgnoreDetection { get; set; }

        public static ExceptionLoggerFactory Instance {
            get {
                if (instance != null)
                    return instance;

                lock (typeof(ExceptionLoggerFactory)) {
                    if (instance != null)
                        return instance;

                    instance = new ExceptionLoggerFactory();
                    return instance;
                }
            }
        }
    }
}