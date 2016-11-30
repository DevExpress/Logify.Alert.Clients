using System;
using System.Windows.Forms;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core {
    public class WinFormsApplicationCollector : ApplicationCollector {
        public override string AppName { get { return Application.ProductName; } }
        public override string AppVersion { get { return Application.ProductVersion; } }
        public override string UserId { get { return String.Empty; } }

        public WinFormsApplicationCollector(): base() {}
    }
}
