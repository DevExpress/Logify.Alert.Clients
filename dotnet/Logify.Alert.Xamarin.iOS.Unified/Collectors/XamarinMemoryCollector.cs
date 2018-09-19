using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using Foundation;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinMemoryCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("memory");
            try {
                long allocated = System.GC.GetTotalMemory(false);
                long total = Convert.ToInt64(NSProcessInfo.ProcessInfo.PhysicalMemory);
                long free = total - allocated;

                logger.WriteValue("ram", NSProcessInfo.ProcessInfo.PhysicalMemory.ToString());
                logger.WriteValue("free", free.ToString());
                //logger.WriteValue("isLow", memoryInfo.LowMemory.ToString());
                //logger.WriteValue("lowTreshold", memoryInfo.Threshold.ToString());
            }
            finally {
                logger.EndWriteObject("memory");
            }
        }
    }
}
