using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.System;
using Windows.System.Profile;

namespace DevExpress.Logify.Core.Internal {
    public class UWPOSCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("os");
            try {
                logger.WriteValue("platform", "UWP");
                logger.WriteValue("architecture", RuntimeInformation.OSArchitecture.ToString());
                logger.WriteValue("version", GetVersion());
                logger.WriteValue("is64bit", Is64Bit());
            }
            finally {
                logger.EndWriteObject("os");
            }
        }
        private bool Is64Bit() {
            return RuntimeInformation.OSArchitecture == Architecture.X64 || RuntimeInformation.OSArchitecture == Architecture.Arm64;
        }

        private string GetVersion() {
            try {
                string versionInfo = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                ulong versionCode = ulong.Parse(versionInfo);
                ulong major = (versionCode & 0xFFFF000000000000L) >> 48;
                ulong minor = (versionCode & 0x0000FFFF00000000L) >> 32;
                ulong build = (versionCode & 0x00000000FFFF0000L) >> 16;
                ulong revision = (versionCode & 0x000000000000FFFFL);
                return string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
            }
            catch {
                return null;
            }
        }
    }
}
