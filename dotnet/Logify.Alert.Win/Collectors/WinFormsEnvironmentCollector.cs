using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class WinFormsEnvironmentCollector : DesktopEnvironmentCollector {
        public WinFormsEnvironmentCollector(ILogifyClientConfiguration config)
            : base(config) {
        }
        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            base.RegisterCollectors(config);
        }
        //protected override void RegisterCollectors() {
        //    base.RegisterCollectors();
        //    //etc
        //}
    }
}
