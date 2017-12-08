using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DevExpress.Logify.Core.Internal {
    public class ManagedAssemblyInjector {
        public string BootstrapDllPath { get; set; }
        public string ManagedAssemblyPath { get; set; }
        public string ManagedClassName { get; set; }
        public string ManagedMethodName { get; set; }
        public string ManagedMethodParameter { get; set; }

        string RemoteCallParameter { get { return ManagedAssemblyPath + " " + ManagedClassName + " " + ManagedMethodName + " " + ManagedMethodParameter; } }

        public bool Inject(Process process, int dwProcessId) {
            Logger.InfoFormat("Injecting {0} into target process", BootstrapDllPath);

            long offset = GetMethodOffset(BootstrapDllPath, "InjectManagedAssembly");
            if (offset == 0)
                return false;

            IntPtr procHandle = Win32.OpenProcess(Win32.PROCESS_CREATE_THREAD | Win32.PROCESS_QUERY_INFORMATION | Win32.PROCESS_VM_OPERATION | Win32.PROCESS_VM_WRITE | Win32.PROCESS_VM_READ, false, dwProcessId);
            if (procHandle == IntPtr.Zero) {
                Logger.ErrorFormat("Unable to open target process. ProcessId={0}, LastError={1}", dwProcessId, Marshal.GetLastWin32Error());
                return false;
            }

            if (!InjectDll(procHandle, BootstrapDllPath))
                return false;

            ulong baseAddr = GetRemoteModuleHandle(process, Path.GetFileName(BootstrapDllPath));
            if (baseAddr == 0)
                return false;

            Logger.Info("SUCCESS");

            Logger.InfoFormat("Executing injected code. Asm={0}, Class={1}, Method={2}, Param={3}", ManagedAssemblyPath, ManagedClassName, ManagedMethodName, ManagedMethodParameter);
            IntPtr remoteAddress = new IntPtr((long)(baseAddr + (ulong)offset));
            bool isOk = MakeRemoteCall(procHandle, remoteAddress, RemoteCallParameter);
            if (isOk)
                Logger.Info("SUCCESS");
            else
                Logger.Error("FAILED");
            Win32.CloseHandle(procHandle);
            return true;
        }
        static bool InjectDll(IntPtr procHandle, string dllName) {
            const string libName = "kernel32.dll";
            const string procName = "LoadLibraryW";
            IntPtr loadLibraryAddr = Win32.GetProcAddress(Win32.GetModuleHandle(libName), procName);
            if (loadLibraryAddr == IntPtr.Zero) {
                Logger.ErrorFormat("Unable to find address of '{0}' procedure, lib={1}, LastError={1}", procName, libName, Marshal.GetLastWin32Error());
                return false;
            }

            return MakeRemoteCall(procHandle, loadLibraryAddr, dllName);
        }
        static bool MakeRemoteCall(IntPtr procHandle, IntPtr methodAddr, string argument) {
            uint textSize = (uint)Encoding.Unicode.GetByteCount(argument);
            uint allocSize = textSize + 2;
            IntPtr allocMemAddress = Win32.VirtualAllocEx(procHandle, IntPtr.Zero, allocSize, Win32.AllocationType.Commit | Win32.AllocationType.Reserve, Win32.PAGE_READWRITE);
            if (allocMemAddress == IntPtr.Zero) {
                Logger.ErrorFormat("Unable allocate memory in target process LastError={0}", Marshal.GetLastWin32Error());
                return false;
            }

            UIntPtr bytesWritten;
            Win32.WriteProcessMemory(procHandle, allocMemAddress, Encoding.Unicode.GetBytes(argument), textSize, out bytesWritten);

            bool isOk = false;
            IntPtr threadHandle = Win32.CreateRemoteThread(procHandle, IntPtr.Zero, 0, methodAddr, allocMemAddress, 0, IntPtr.Zero);
            if (threadHandle != IntPtr.Zero) {
                Win32.WaitForSingleObject(threadHandle, Win32.INFINITE);
                isOk = true;
            }
            else {
                Logger.ErrorFormat("Unable create remote thread in target process LastError={0}", Marshal.GetLastWin32Error());
            }

            Win32.VirtualFreeEx(procHandle, allocMemAddress, allocSize, Win32.AllocationType.Release);
            if (threadHandle != IntPtr.Zero)
                Win32.CloseHandle(threadHandle);
            return isOk;
        }
        static long GetMethodOffset(string dllPath, string methodName) {
            IntPtr hLib = Win32.LoadLibrary(dllPath);
            if (hLib == IntPtr.Zero) {
                Logger.ErrorFormat("Unable to load library {0}, LastError={1} into loader process", dllPath, Marshal.GetLastWin32Error());
                return 0;
            }

            IntPtr call = Win32.GetProcAddress(hLib, methodName);
            if (call == IntPtr.Zero) {
                Logger.ErrorFormat("Unable to find address of '{0}' procedure, lib={1}, LastError={1}", methodName, dllPath, Marshal.GetLastWin32Error());
                return 0;
            }
            long result = call.ToInt64() - hLib.ToInt64();
            Win32.FreeLibrary(hLib);
            return result;
        }
        static ulong GetRemoteModuleHandle(Process process, string moduleName) {
            int count = process.Modules.Count;
            for (int i = 0; i < count; i++) {
                ProcessModule module = process.Modules[i];
                if (String.Compare(module.ModuleName, moduleName, StringComparison.InvariantCultureIgnoreCase) == 0) {
                    return (ulong)module.BaseAddress;
                }
            }
            Logger.ErrorFormat("Injected module {0} not found in target process ProcessId={1}", moduleName, process.Id);
            Logger.Error("Injection FAILED");
            return 0;
        }
        /*
        [Flags]
        public enum SnapshotFlags : uint {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            All = (HeapList | Process | Thread | Module),
            Inherit = 0x80000000,
            NoHeaps = 0x40000000

        }

        //[StructLayout(LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        [StructLayout(LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct MODULEENTRY32 {
            internal int dwSize;
            internal uint th32ModuleID;
            internal uint th32ProcessID;
            internal uint GlblcntUsage;
            internal uint ProccntUsage;
            internal IntPtr modBaseAddr;
            internal uint modBaseSize;
            internal IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szExePath;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);
        [DllImport("kernel32.dll")]
        static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);
        [DllImport("kernel32.dll")]
        static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);
        static ulong GetRemoteModuleHandleWin32(int processId, string moduleName) {
            MODULEENTRY32 me32 = new MODULEENTRY32();
            // get snapshot of all modules in the remote process 
            me32.dwSize = Marshal.SizeOf(typeof(MODULEENTRY32));
            IntPtr hSnapshot = CreateToolhelp32Snapshot(SnapshotFlags.Module | SnapshotFlags.Module32, processId);

            // can we start looking?
            if (!Module32First(hSnapshot, ref me32)) {
                Logger.ErrorFormat("Unable enumerate modules in target process ProcessId={0}, LastError={1}", processId, Marshal.GetLastWin32Error());
                Win32.CloseHandle(hSnapshot);
                return 0;
            }

            // enumerate all modules till we find the one we are looking for 
            // or until every one of them is checked
            for (; ; ) {
                if (String.Compare(me32.szModule, moduleName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    break;

                if (!Module32Next(hSnapshot, ref me32))
                    break;
            }

            // close the handle
            Win32.CloseHandle(hSnapshot);

            // check if module handle was found and return it
            if (String.Compare(me32.szModule, moduleName, StringComparison.InvariantCultureIgnoreCase) == 0)
                return (ulong)me32.modBaseAddr.ToInt64();

            Logger.ErrorFormat("Injected module {0} not found in target process ProcessId={1} LastError={2}", moduleName, processId, Marshal.GetLastWin32Error());
            Logger.Error("Injection FAILED");
            return 0;
        }
        */
    }

}
