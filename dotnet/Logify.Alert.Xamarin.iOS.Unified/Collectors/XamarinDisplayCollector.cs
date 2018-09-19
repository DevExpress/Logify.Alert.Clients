using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using UIKit;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinDisplayCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("display");
            try {
                UIScreen mainScreen = UIScreen.MainScreen;
                logger.WriteValue("width", mainScreen.Bounds.Width.ToString());
                logger.WriteValue("height", mainScreen.Bounds.Height.ToString());
                //logger.WriteValue("dpiX", mainScreen. metrics.Xdpi.ToString());
                //logger.WriteValue("dpiY", metrics.Ydpi.ToString());
            }
            finally {
                logger.EndWriteObject("display");
            }
        }
    }
}
