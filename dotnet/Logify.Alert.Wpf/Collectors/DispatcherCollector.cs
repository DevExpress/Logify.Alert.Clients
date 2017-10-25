using DevExpress.Logify.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.Logify.Core.Internal {
    class DispatcherCollector : WpfApplicationCollector
    {
        public override void Process(Exception ex, ILogger logger)
        {
            logger.WriteValue("Exception Type", "DispatcherUnhandledException");
        }
    }
}
