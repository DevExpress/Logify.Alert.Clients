using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using Windows.System;

namespace DevExpress.Logify.Core.Internal {
    public class UWPMemoryCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("memory");
            try {
                logger.WriteValue("ram", MemoryManager.AppMemoryUsageLimit.ToString());

                ulong free = MemoryManager.AppMemoryUsageLimit - MemoryManager.AppMemoryUsage;
                logger.WriteValue("free", free.ToString());

                bool isLow = MemoryManager.AppMemoryUsageLevel == AppMemoryUsageLevel.OverLimit;
                logger.WriteValue("isLow", isLow.ToString());
            }
            finally {
                logger.EndWriteObject("memory");
            }
        }
    }
}
