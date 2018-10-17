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
                nfloat scale = mainScreen.Scale;
                nfloat width = mainScreen.Bounds.Width * scale;
                nfloat height = mainScreen.Bounds.Height * scale;
                logger.WriteValue("width", width.ToString());
                logger.WriteValue("height", height.ToString());
                //logger.WriteValue("dpiX", mainScreen. metrics.Xdpi.ToString());
                //logger.WriteValue("dpiY", metrics.Ydpi.ToString());
            }
            finally {
                logger.EndWriteObject("display");
            }
        }
    }
}
