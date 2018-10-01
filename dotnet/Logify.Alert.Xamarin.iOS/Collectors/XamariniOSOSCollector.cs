using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using UIKit;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinIOSOSCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("os");
            try {
                logger.WriteValue("platform", "iOS");
                logger.WriteValue("architecture", RuntimeInformation.OSArchitecture.ToString());
                string version = UIDevice.CurrentDevice.SystemVersion;
                if (!string.IsNullOrEmpty(version))
                    logger.WriteValue("version", version);
                logger.WriteValue("is64bit", RuntimeInformation.OSArchitecture == Architecture.X64 || RuntimeInformation.OSArchitecture == Architecture.Arm64);
            }
            finally {
                logger.EndWriteObject("os");
            }
        }
    }
}
