using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class UWPExceptionCollector : RootInfoCollector {
        public UWPExceptionCollector(LogifyCollectorContext context) : base(context) {
        }

        protected override void RegisterCollectors(LogifyCollectorContext context) {
            Collectors.Add(new DevelopementPlatformCollector(Platform.UWP)); // added in constuctor
            Collectors.Add(new UWPApplicationCollector());
            Collectors.Add(new UWPDisplayCollector());
            Collectors.Add(new UWPMemoryCollector());
            Collectors.Add(new UWPDeviceInfoCollector());
            Collectors.Add(new UWPOSCollector());
            Collectors.Add(new DebuggerCollector());
        }
    }
}
