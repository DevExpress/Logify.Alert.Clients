using System;
using System.Linq;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WinFormsEnvironmentCollector : DesktopEnvironmentCollector {
        public WinFormsEnvironmentCollector(LogifyCollectorContext context)
            : base(context) {
        }
        protected override void RegisterCollectors(LogifyCollectorContext context) {
            base.RegisterCollectors(context);
            ExtendCollectors(context);
        }

        void ExtendCollectors(LogifyCollectorContext context) {
            ExtendMemoryCollector(context);
        }

        void ExtendMemoryCollector(LogifyCollectorContext context) {
            MemoryCollector collector = Collectors.FirstOrDefault(c => c.GetType() == typeof(MemoryCollector)) as MemoryCollector;
            if (collector != null) {
                collector.Collectors.Add(new AvailableVirtulaMemoryCollector());
            }
        }

        //protected override void RegisterCollectors() {
        //    base.RegisterCollectors();
        //    //etc
        //}
    }
}
