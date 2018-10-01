using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinIOSExceptionCollector : RootInfoCollector {
        public XamarinIOSExceptionCollector(LogifyCollectorContext context) : base(context) {
        }

        protected override void RegisterCollectors(LogifyCollectorContext context) {
            Collectors.Add(new DevelopementPlatformCollector(Platform.XAMARIN_IOS)); // added in constuctor
            Collectors.Add(new XamarinApplicationCollector());
            Collectors.Add(new XamarinDisplayCollector());
            Collectors.Add(new XamarinMemoryCollector());
            Collectors.Add(new XamarinDeviceInfoCollector());
            Collectors.Add(new XamarinIOSOSCollector());
            //Collectors.Add(new XamarinNetworkCollector());
            Collectors.Add(new DebuggerCollector());
        }
    }
}
