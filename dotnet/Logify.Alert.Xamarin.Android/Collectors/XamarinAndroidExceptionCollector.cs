using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinAndroidExceptionCollector : RootInfoCollector {
        public XamarinAndroidExceptionCollector(LogifyCollectorContext context) : base(context) {
        }

        protected override void RegisterCollectors(LogifyCollectorContext context) {
            Collectors.Add(new DevelopementPlatformCollector(Platform.XAMARIN_ANDROID)); // added in constuctor
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
