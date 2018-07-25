using Android.App;
using Android.Content;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinExternalStorageCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("externalStorage");
            try {
                logger.WriteValue("state", Android.OS.Environment.ExternalStorageState);
                logger.WriteValue("directory", Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
                logger.WriteValue("totalSpace", Android.OS.Environment.ExternalStorageDirectory.TotalSpace.ToString());
                logger.WriteValue("freeSpace", Android.OS.Environment.ExternalStorageDirectory.FreeSpace.ToString());
                logger.WriteValue("isRemovable", Android.OS.Environment.IsExternalStorageRemovable.ToString());
                logger.WriteValue("isEmulated", Android.OS.Environment.IsExternalStorageEmulated.ToString());
            }
            finally {
                logger.EndWriteObject("externalStorage");
            }
        }
    }
}