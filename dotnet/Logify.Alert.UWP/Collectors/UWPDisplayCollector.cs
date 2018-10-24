using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Graphics.Display;

namespace DevExpress.Logify.Core.Internal {
    public class UWPDisplayCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("display");
            try {
                DisplayInformation displayInfo = DisplayInformation.GetForCurrentView();
                logger.WriteValue("width", displayInfo.ScreenWidthInRawPixels.ToString());
                logger.WriteValue("height", displayInfo.ScreenHeightInRawPixels.ToString());
                logger.WriteValue("dpiX", displayInfo.RawDpiX.ToString());
                logger.WriteValue("dpiY", displayInfo.RawDpiY.ToString());
            }
            finally {
                logger.EndWriteObject("display");
            }
        }
    }
}
