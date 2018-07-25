using Android.App;
using Android.Content;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinMemoryCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("memory");
            try {
                ActivityManager activityManager = Application.Context.GetSystemService(Activity.ActivityService) as ActivityManager;
                ActivityManager.MemoryInfo memoryInfo = new ActivityManager.MemoryInfo();
                activityManager.GetMemoryInfo(memoryInfo);
                logger.WriteValue("ram", memoryInfo.TotalMem.ToString());
                logger.WriteValue("free", memoryInfo.AvailMem.ToString());
                logger.WriteValue("isLow", memoryInfo.LowMemory.ToString());
                logger.WriteValue("lowTreshold", memoryInfo.Threshold.ToString());
            }
            finally {
                logger.EndWriteObject("memory");
            }
        }
    }
}
