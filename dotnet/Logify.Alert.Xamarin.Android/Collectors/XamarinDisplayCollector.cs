using Android.Content.Res;
using Android.Util;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinDisplayCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("display");
            try {
                DisplayMetrics metrics = Resources.System.DisplayMetrics;
                logger.WriteValue("width", metrics.WidthPixels.ToString());
                logger.WriteValue("height", metrics.HeightPixels.ToString());
                logger.WriteValue("dpiX", metrics.Xdpi.ToString());
                logger.WriteValue("dpiY", metrics.Ydpi.ToString());
            }
            finally {
                logger.EndWriteObject("display");
            }
        }
    }
}
