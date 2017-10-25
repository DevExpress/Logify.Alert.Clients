using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    class WpfApplicationCollector : ApplicationCollector {
        public override string AppName { get { return Application.ResourceAssembly.GetName().Name; } }
        public override string AppVersion { get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(); } }
        public override string UserId { get { return String.Empty; } }

        public WpfApplicationCollector() : base() { }
    }
}
