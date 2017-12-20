using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

#if !NETSTANDARD
using System.Management;
#endif

namespace DevExpress.Logify.Core.Internal {
    public static class HardwareId {
        public static string Get() {
            try {
                string result = GetCachedValue();
                if (String.IsNullOrEmpty(result)) {
                    result = Calculate();
                    SetCachedValue(result);
                }
                return result;
            }
            catch {
                return String.Empty;
            }
        }

        static string GetFileName() {
            return Path.Combine(Path.GetTempPath(), "logify.hid");
        }
        static string GetCachedValue() {
            try {
                string fileName = GetFileName();
                if (File.Exists(fileName))
                    return File.ReadAllText(fileName);
                else
                    return String.Empty;
            }
            catch {
                return String.Empty;
            }
        }
        static void SetCachedValue(string result) {
            File.WriteAllText(GetFileName(), result);
        }

        public static string Calculate() {
#if !NETSTANDARD
            string content = GetCpuId() + GetHddSerialId();
#else
            string content = String.Empty;
#endif
            if (String.IsNullOrEmpty(content))
                content = new Guid().ToString();
            return MakeHexString(CalcMD5Hash(content));
        }
        static byte[] CalcMD5Hash(string value) {
            MD5 md5 = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(value);
            return md5.ComputeHash(bytes);
        }
        static string MakeHexString(byte[] bytes) {
            StringBuilder sb = new StringBuilder();
            int count = bytes.Length;
            for (int i = 0; i < count; i++)
                sb.Append(bytes[i].ToString("X2"));
            return sb.ToString();
        }
#if !NETSTANDARD
        static string GetCpuId() {
            try {
                using (ManagementClass @class = new ManagementClass("win32_processor")) {
                    ManagementObjectCollection items = @class.GetInstances();
                    String result = String.Empty;
                    foreach (ManagementObject item in items) {
                        result = item.Properties["processorID"].Value.ToString();
                        break;
                    }
                    return result;
                }
            }
            catch {
                return String.Empty;
            }
        }
        static string GetHddSerialId() {
            try {
                using (ManagementClass @class = new ManagementClass("Win32_LogicalDisk")) {
                    ManagementObjectCollection items = @class.GetInstances();
                    string result = String.Empty;
                    foreach (ManagementObject item in items)
                        result += Convert.ToString(item["VolumeSerialNumber"]);
                    return result;
                }
            }
            catch {
                return String.Empty;
            }
        }
#endif
    }
}