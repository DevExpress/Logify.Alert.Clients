using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WinFormsEnvironmentCollector : DesktopEnvironmentCollector {
        public WinFormsEnvironmentCollector(LogifyCollectorContext context)
            : base(context) {
        }
        protected override void RegisterCollectors(LogifyCollectorContext context) {
            base.RegisterCollectors(context);
        }
        //protected override void RegisterCollectors() {
        //    base.RegisterCollectors();
        //    //etc
        //}
    }
}
