using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace DevExpress.Logify.Core {
#if NETSTANDARD
    public class OperatingSystemCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("os");
            try {
                logger.WriteValue("platform", DetectPlatform());
                logger.WriteValue("architecture", RuntimeInformation.OSArchitecture.ToString());
                //logger.WriteValue("servicePack", os.ServicePack);
                Version version = DetectRealOSVersion();
                if (version != null)
                    logger.WriteValue("version", version.ToString());
                logger.WriteValue("is64bit", RuntimeInformation.OSArchitecture == Architecture.X64 || RuntimeInformation.OSArchitecture == Architecture.Arm64);
            }
            finally {
                logger.EndWriteObject("os");
            }
        }

        Version DetectRealOSVersion() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                // "Microsoft Windows 6.3.6900 "
                string content = RuntimeInformation.OSDescription;
                if (String.IsNullOrEmpty(content))
                    return null;
                content = content.Trim();
                int index = content.LastIndexOf(' ');
                if (index >= 0 || index < content.Length - 1) {
                    Version version;
                    if (Version.TryParse(content.Substring(index + 1), out version))
                        return version;
                }
            }

            return null;
        }

        string DetectPlatform() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Win32NT";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "OSX";

            return String.Empty;
        }
    }
#else
    public class OperatingSystemCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("os");
            try {
                OperatingSystem os = Environment.OSVersion;
                logger.WriteValue("platform", os.Platform.ToString());
                logger.WriteValue("servicePack", os.ServicePack);
                logger.WriteValue("version", DetectRealOSVersion(os.Version).ToString());
                logger.WriteValue("is64bit", Environment.Is64BitOperatingSystem);
                //logger.WriteValue("os", "Windows :)");
            }
            finally {
                logger.EndWriteObject("os");
            }
        }
        public Version DetectRealOSVersion(Version originalVersion) {
            if (originalVersion.Major != 6)
                return originalVersion;
            //http://msdn.microsoft.com/en-us/library/windows/desktop/ms724832(v=vs.85).aspx
            // * For applications that have been manifested for Windows 8.1 or Windows 10. Applications not manifested
            // for Windows 8.1 or Windows 10 will return the Windows 8 OS version value (6.2). To manifest your
            // applications for Windows 8.1 or Windows 10, refer to Targeting your application for Windows.

            try {
                try {
                    RegistryKey versionKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                    if (versionKey == null)
                        return originalVersion;


                    object majorValue = versionKey.GetValue("CurrentMajorVersionNumber");
                    if (majorValue == null || !(majorValue is int))
                        return originalVersion;
                    object minorValue = versionKey.GetValue("CurrentMinorVersionNumber");
                    if (minorValue == null || !(minorValue is int))
                        return originalVersion;
                    string currentBuild = versionKey.GetValue("CurrentBuild") as string;
                    return new Version((int)majorValue, (int)minorValue, Convert.ToInt32(currentBuild)); ;
                    //minorVersion = Convert.ToInt32(Build);
                }
                catch {
                    return originalVersion;
                }
            }
            catch {
                return originalVersion;
            }
        }
    }
#endif
}
