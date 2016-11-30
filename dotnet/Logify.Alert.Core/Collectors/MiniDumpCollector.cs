using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace DevExpress.Logify.Core {
    public class MiniDumpCollector : IInfoCollector {
        public const string DumpGuidKey = "miniDumpGuid";
        public const string DumpFileNameKey = "miniDumpFileName";

        [HandleProcessCorruptedStateExceptions]
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("miniDump");
            try {
                Guid dumpGuid = Guid.NewGuid();
                string fileName = dumpGuid.ToString() + ".dmp";
                fileName = Path.Combine(Path.GetTempPath(), fileName);
                if (MiniDumpWriter.Write(fileName, MiniDumpType.Normal)) {
                    logger.WriteValue("dumpGuid", dumpGuid.ToString());
                    logger.Data[DumpGuidKey] = dumpGuid.ToString();
                    logger.Data[DumpFileNameKey] = fileName;
                }
            }
            catch {
            }
            finally {
                logger.EndWriteObject("miniDump");
            }
        }
    }

    [Flags]
    enum MiniDumpType : uint {
        Normal = 0x00000000,
        WithDataSegs = 0x00000001,
        WithFullMemory = 0x00000002,
        WithHandleData = 0x00000004,
        FilterMemory = 0x00000008,
        ScanMemory = 0x00000010,
        WithUnloadedModules = 0x00000020,
        WithIndirectlyReferencedMemory = 0x00000040,
        FilterModulePaths = 0x00000080,
        WithProcessThreadData = 0x00000100,
        WithPrivateReadWriteMemory = 0x00000200,
        WithoutOptionalData = 0x00000400,
        WithFullMemoryInfo = 0x00000800,
        WithThreadInfo = 0x00001000,
        WithCodeSegs = 0x00002000,
        WithoutAuxiliaryState = 0x00004000,
        WithFullAuxiliaryState = 0x00008000,
        WithPrivateWriteCopyMemory = 0x00010000,
        IgnoreInaccessibleMemory = 0x00020000,
        ValidTypeFlags = 0x0003ffff,
    };
    class MiniDumpWriter {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]  // Pack=4 is important! So it works also for x64!
        struct MiniDumpExceptionInformation {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            public int ClientPointers;
        }

        [DllImport("dbghelp.dll",
          EntryPoint = "MiniDumpWriteDump",
          CallingConvention = CallingConvention.StdCall,
          CharSet = CharSet.Unicode,
          ExactSpelling = true, SetLastError = true)]
        static extern bool MiniDumpWriteDump(
          IntPtr hProcess,
          uint processId,
          IntPtr hFile,
          uint dumpType,
          ref MiniDumpExceptionInformation expParam,
          IntPtr userStreamParam,
          IntPtr callbackParam);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        static extern uint GetCurrentThreadId();
        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess", ExactSpelling = true)]
        static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcessId", ExactSpelling = true)]
        static extern uint GetCurrentProcessId();

        [HandleProcessCorruptedStateExceptions]
        public static bool Write(string fileName, MiniDumpType dumpType) {
            try {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    MiniDumpExceptionInformation exp;
                    exp.ThreadId = GetCurrentThreadId();
                    exp.ClientPointers = 0;
                    exp.ExceptionPointers = System.Runtime.InteropServices.Marshal.GetExceptionPointers();
                    return MiniDumpWriteDump(
                      GetCurrentProcess(),
                      GetCurrentProcessId(),
                      fs.SafeFileHandle.DangerousGetHandle(),
                      (uint)dumpType,
                      ref exp,
                      IntPtr.Zero,
                      IntPtr.Zero);
                }
            }
            catch {
                return false;
            }
        }
    }
}