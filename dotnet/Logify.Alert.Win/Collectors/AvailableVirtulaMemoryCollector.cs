using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace DevExpress.Logify.Core.Internal {
    [SecuritySafeCritical]
    internal class AvailableVirtulaMemoryCollector : IInfoCollector {
        long GetAvailableVirtualMemory() {
            MEMORYSTATUSEX memoryStatus = new MEMORYSTATUSEX();
            if (!GlobalMemoryStatusEx(memoryStatus))
                return (Int32.MaxValue >> 20);
            return (long)(memoryStatus.ullAvailVirtual >> 20);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        sealed class MEMORYSTATUSEX {
            readonly uint dwLength;
            public MEMORYSTATUSEX() {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }
        #region DllImport
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
        #endregion DllImport

        [HandleProcessCorruptedStateExceptions]
        public void Process(Exception ex, ILogger logger) {
            try {
                logger.WriteValue("availableVirtualMem", String.Format("{0} Mb", GetAvailableVirtualMemory()));
            } catch { }
        }
    }
}