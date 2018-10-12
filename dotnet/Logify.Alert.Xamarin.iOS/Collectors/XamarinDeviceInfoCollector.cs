using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Foundation;
using UIKit;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinDeviceInfoCollector : IInfoCollector {
        private static Dictionary<string, string> modelsMap;
        private const string modelProperty = "hw.machine";
        [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
        static internal extern int sysctlbyname([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);

        #region Models map initialization
        static XamarinDeviceInfoCollector() {
            Dictionary<string, string> modelsMap = new Dictionary<string, string>() {
                { "iPhone1,1", "iPhone" },
                { "iPhone1,2", "iPhone 3G" },
                { "iPhone2,1", "iPhone 3GS" },
                { "iPhone3,1", "iPhone 4 GSM" },
                { "iPhone3,2", "iPhone 4 GSM" },
                { "iPhone3,3", "iPhone 4 CDMA" },
                { "iPhone4,1", "iPhone 4S" },
                { "iPhone5,1", "iPhone 5 GSM" },
                { "iPhone5,2", "iPhone 5 Global" },
                { "iPhone5,3", "iPhone 5C GSM" },
                { "iPhone5,4", "iPhone 5C Global" },
                { "iPhone6,1", "iPhone 5S GSM" },
                { "iPhone6,2", "iPhone 5S Global" },
                { "iPhone7,2", "iPhone 6" },
                { "iPhone7,1", "iPhone 6 Plus" },
                { "iPhone8,1", "iPhone 6S" },
                { "iPhone8,2", "iPhone 6S Plus" },
                { "iPhone8,4", "iPhone SE" },
                { "iPhone9,1", "iPhone 7" },
                { "iPhone9,3", "iPhone 7" },
                { "iPhone9,2", "iPhone 7 Plus" },
                { "iPhone9,4", "iPhone 7 Plus" },
                { "iPhone10,1", "iPhone 8" },
                { "iPhone10,4", "iPhone 8" },
                { "iPhone10,2", "iPhone 8 Plus" },
                { "iPhone10,5", "iPhone 8 Plus" },
                { "iPhone10,3", "iPhone X" },
                { "iPhone10,6", "iPhone X" },
                { "iPhone11,8", "iPhone XR" },
                { "iPhone11,4", "iPhone Xs Max" },
                { "iPhone11,6", "iPhone Xs Max" },
                { "iPhone11,2", "iPhone Xs" },
                { "iPod1,1", "iPod touch" },
                { "iPod2,1", "iPod touch 2G" },
                { "iPod3,1", "iPod touch 3G" },
                { "iPod4,1", "iPod touch 4G" },
                { "iPod5,1", "iPod touch 5G" },
                { "iPod7,1", "iPod touch 6G" },
                { "iPad1,1", "iPad" },
                { "iPad2,1", "iPad 2 WiFi" },
                { "iPad2,2", "iPad 2 GSM" },
                { "iPad2,3", "iPad 2 CDMA" },
                { "iPad2,4", "iPad 2 Wifi" },
                { "iPad3,1", "iPad 3 WiFi" },
                { "iPad3,2", "iPad 3 Wi-Fi + Cellular (VZ)" },
                { "iPad3,3", "iPad 3 Wi-Fi + Cellular" },
                { "iPad3,4", "iPad 4 Wifi" },
                { "iPad3,5", "iPad 4 Wi-Fi + Cellular" },
                { "iPad3,6", "iPad 4 Wi-Fi + Cellular (MM)" },
                { "iPad4,1", "iPad Air Wifi" },
                { "iPad4,2", "iPad Air Wi-Fi + Cellular" },
                { "iPad4,3", "iPad Air Wi-Fi + Cellular (TD-LTE)" },
                { "iPad5,3", "iPad Air 2" },
                { "iPad5,4", "iPad Air 2" },
                { "iPad6,7", "iPad Pro" },
                { "iPad6,8", "iPad Pro Wi-Fi + Cellular" },
                { "iPad6,3", "iPad Pro (9.7-inch)" },
                { "iPad6,4", "iPad Pro (9.7-inch) Wi-Fi + Cellular" },
                { "iPad7,1", "iPad Pro (12.9-inch) (2nd generation)" },
                { "iPad7,2", "iPad Pro (12.9-inch) (2nd generation) Wi-Fi + Cellular" },
                { "iPad7,3", "iPad Pro (10.5-inch)" },
                { "iPad7,4", "iPad Pro (10.5-inch) Wi-Fi + Cellular" },
                { "iPad2,5", "iPad mini Wifi" },
                { "iPad2,6", "iPad mini Wi-Fi + Cellular" },
                { "iPad2,7", "iPad mini Wi-Fi + Cellular (MM)" },
                { "iPad4,4", "iPad mini 2 Wifi" },
                { "iPad4,5", "iPad mini 2 Wi-Fi + Cellular" },
                { "iPad4,6", "iPad mini 2 Wi-Fi + Cellular (TD-LTE)" },
                { "iPad4,7", "iPad mini 3 Wifi" },
                { "iPad4,8", "iPad mini 3 Wi-Fi + Cellular" },
                { "iPad4,9", "iPad mini 3 Wi-Fi + Cellular (TD-LTE)" },
                { "iPad5,1", "iPad mini 4" },
                { "iPad5,2", "iPad mini 4 Wi-Fi + Cellular" },
                { "iPad6,11", "iPad 5 Wi-Fi" },
                { "iPad6,12", "iPad 5 Wi-Fi + Cellular" },
                { "iPad7,5", "iPad 6 Wi-Fi" },
                { "iPad7,6", "iPad 6 Wi-Fi + Cellular" },
                { "i386", "Simulator" },
                { "x86_64", "Simulator" }
            };
        }
        #endregion

        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("device");
            try {
                logger.WriteValue("manufacturer", "Apple");
                string modelCode = GetModelCode();
                logger.WriteValue("model", GetModel(modelCode));
                logger.WriteValue("modelCode", modelCode);
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
        private string GetModelCode() {
            try {
                IntPtr ptrLen = Marshal.AllocHGlobal(sizeof(int));
                sysctlbyname(modelProperty, IntPtr.Zero, ptrLen, IntPtr.Zero, 0);

                int strLength = Marshal.ReadInt32(ptrLen);
                IntPtr ptrStr = Marshal.AllocHGlobal(strLength);

                sysctlbyname(modelProperty, ptrStr, ptrLen, IntPtr.Zero, 0);
                string modelCode = Marshal.PtrToStringAnsi(ptrStr);

                Marshal.FreeHGlobal(ptrLen);
                Marshal.FreeHGlobal(ptrStr);

                return modelCode;
            }
            catch {
                return "Unknown";
            }
        }

        private string GetModel(string modelCode) {
            string unknown = "Unknown";
            try {
                string suffix = "";
                string code;
                if (modelCode == "i386" || modelCode == "x86_64") { // Running on a simulator
                    code = NSProcessInfo.ProcessInfo.Environment["SIMULATOR_MODEL_IDENTIFIER"].ToString();
                    suffix = " Simulator";
                }
                else {
                    code = modelCode;
                }
                string result;
                if (modelsMap.TryGetValue(code, out result)) {
                    return result + suffix;
                }
                return unknown;
            }
            catch {
                return unknown;
            }
        }
    }
}
