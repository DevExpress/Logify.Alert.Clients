using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinDroidOSCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("os");
            try {
                logger.WriteValue("platform", "Android");
                logger.WriteValue("architecture", RuntimeInformation.OSArchitecture.ToString());
                string version = Android.OS.Build.VERSION.Release;
                if (!string.IsNullOrEmpty(version))
                    logger.WriteValue("version", version.ToString());
                logger.WriteValue("is64bit", RuntimeInformation.OSArchitecture == Architecture.X64 || RuntimeInformation.OSArchitecture == Architecture.Arm64);
            }
            finally {
                logger.EndWriteObject("os");
            }
        }
    }
}
