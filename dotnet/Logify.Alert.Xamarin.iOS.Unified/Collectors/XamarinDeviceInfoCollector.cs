using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using UIKit;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinDeviceInfoCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("device");
            try {
                logger.WriteValue("manufacturer", "Apple");
                logger.WriteValue("model", UIDevice.CurrentDevice.Model);
                logger.WriteValue("orientation", UIDevice.CurrentDevice.Orientation.ToString());
                logger.WriteValue("product", GetIdiomString());
            }
            finally {
                logger.EndWriteObject("device");
            }
        }

        private string GetIdiomString() {
            switch (UIDevice.CurrentDevice.UserInterfaceIdiom) {
                case UIUserInterfaceIdiom.Phone:
                    return "iPhone";
                case UIUserInterfaceIdiom.Pad:
                    return "iPad";
                case UIUserInterfaceIdiom.CarPlay:
                    return "CarPlay";
                case UIUserInterfaceIdiom.TV:
                    return "TV";
            }

            return "Unspecified";
        }
    }
}
