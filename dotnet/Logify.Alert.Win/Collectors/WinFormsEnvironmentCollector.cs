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
            IInfoCollector collector = Collectors.FirstOrDefault(c => c.GetType() == typeof(MemoryCollector));
            if (collector != null) {
                int index = Collectors.IndexOf(collector);
                Collectors[index] = new WinFormsMemoryCollector(context);
            }
        }

        //protected override void RegisterCollectors() {
        //    base.RegisterCollectors();
        //    //etc
        //}
    }
}
