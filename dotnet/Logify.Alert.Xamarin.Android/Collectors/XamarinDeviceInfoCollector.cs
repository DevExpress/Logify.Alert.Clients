using Android.App;
using Android.Content;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinDeviceInfoCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("device");
            try {
                logger.WriteValue("manufacturer", Android.OS.Build.Manufacturer);
                logger.WriteValue("model", Android.OS.Build.Model);
                logger.WriteValue("product", Android.OS.Build.Product);
            }
            finally {
                logger.EndWriteObject("device");
            }
        }
    }
}
