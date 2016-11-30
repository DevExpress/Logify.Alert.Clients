using System;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.WPF
{
    public class WPFEnvironmentCollector : DesktopEnvironmentCollector
    {
        public WPFEnvironmentCollector(ILogifyClientConfiguration config)
            : base(config) {
        }
        //protected override void RegisterCollectors() {
        //    base.RegisterCollectors();
        //    //etc
        //}
    }
}
