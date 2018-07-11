using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class NetCoreConsoleExceptionCollector : RootInfoCollector {
        public NetCoreConsoleExceptionCollector(ILogifyClientConfiguration config) : base(config) {
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            Collectors.Add(new DevelopementPlatformCollector(Platform.NETCORE_CONSOLE)); // added in constuctor
            Collectors.Add(new XamarinApplicationCollector());
            Collectors.Add(new XamarinDisplayCollector());
            Collectors.Add(new XamarinMemoryCollector());
            Collectors.Add(new XamarinDeviceInfoCollector());
            Collectors.Add(new XamarinExternalStorageCollector());
            Collectors.Add(new XamarinDroidOSCollector());
            Collectors.Add(new XamarinNetworkCollector());
            Collectors.Add(new DebuggerCollector());
        }
    }
}
