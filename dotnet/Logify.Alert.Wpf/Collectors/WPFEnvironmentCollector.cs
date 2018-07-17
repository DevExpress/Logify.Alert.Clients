using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WPFEnvironmentCollector : DesktopEnvironmentCollector
    {
        public WPFEnvironmentCollector(LogifyCollectorContext context)
            : base(context) {
        }
        //protected override void RegisterCollectors() {
        //    base.RegisterCollectors();
        //    //etc
        //}
    }
}
