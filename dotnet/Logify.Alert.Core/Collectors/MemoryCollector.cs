using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace DevExpress.Logify.Core {
    public class MemoryCollector : CompositeInfoCollector {
        public MemoryCollector(ILogifyClientConfiguration config)
            : base(config) {
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            Collectors.Add(new MemoryWorkingSetCollector());
            Collectors.Add(new MemoryPhysicalRamCollector());
#if !ASP_CLIENT
            Collectors.Add(new MemoryClrDetailedCollector());
#endif
        }
        [HandleProcessCorruptedStateExceptions]
        public override void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("memory");
            try {
                base.Process(ex, logger);
            }
            finally {
                logger.EndWriteObject("memory");
            }
        }
    }
    public class MemoryWorkingSetCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.WriteValue("workingSet", String.Format("{0} Mb", System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024)));
        }
    }
    public class MemoryPhysicalRamCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            ulong ram = 0;
            foreach (ManagementObject winPart in searcher.Get())
                ram += Convert.ToUInt64(winPart.Properties["Capacity"].Value) / (1024 * 1024);


            logger.WriteValue("ram", String.Format("{0} Mb", ram));
        }
    }
    public class MemoryClrDetailedCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            using (MemoryPerformanceCounters counters = new MemoryPerformanceCounters(System.Diagnostics.Process.GetCurrentProcess())) {
                logger.WriteValue("clrBytesInAllHeaps", counters.BytesInAllHeaps.ToString("n0", CultureInfo.InvariantCulture));
                logger.WriteValue("clrLargeObjectHeapSize", counters.LargeObjectHeapSize.ToString("n0", CultureInfo.InvariantCulture));
                logger.WriteValue("clrGen0HeapSize", counters.Gen0HeapSize.ToString("n0", CultureInfo.InvariantCulture));
                logger.WriteValue("clrGen1HeapSize", counters.Gen1HeapSize.ToString("n0", CultureInfo.InvariantCulture));
                logger.WriteValue("clrGen2HeapSize", counters.Gen2HeapSize.ToString("n0", CultureInfo.InvariantCulture));
            }
        }
    }
    public class MemoryPerformanceCounters : IDisposable {
        const string CategoryNetClrMemory = ".NET CLR Memory";
        const string ProcessId = "Process ID";
        const int ProcessesToTry = 40;

        PerformanceCounter counterBytesInAllHeaps;
        PerformanceCounter counterLargeObjectHeapSize;
        PerformanceCounter counterGen0HeapSize;
        PerformanceCounter counterGen1HeapSize;
        PerformanceCounter counterGen2HeapSize;

        public MemoryPerformanceCounters(Process p) {
            string processInstanceName = GetManagedPerformanceCounterInstanceName(p);
            counterBytesInAllHeaps = new PerformanceCounter(CategoryNetClrMemory, "# Bytes in all Heaps", processInstanceName);
            counterLargeObjectHeapSize = new PerformanceCounter(CategoryNetClrMemory, "Large Object Heap size", processInstanceName);
            counterGen0HeapSize = new PerformanceCounter(CategoryNetClrMemory, "Gen 0 heap size", processInstanceName);
            counterGen1HeapSize = new PerformanceCounter(CategoryNetClrMemory, "Gen 1 heap size", processInstanceName);
            counterGen2HeapSize = new PerformanceCounter(CategoryNetClrMemory, "Gen 2 heap size", processInstanceName);
        }

        public long BytesInAllHeaps { get { return counterBytesInAllHeaps.NextSample().RawValue; } }
        public long LargeObjectHeapSize { get { return counterLargeObjectHeapSize.NextSample().RawValue; } }
        public long Gen0HeapSize { get { return counterGen0HeapSize.NextSample().RawValue; } }
        public long Gen1HeapSize { get { return counterGen1HeapSize.NextSample().RawValue; } }
        public long Gen2HeapSize { get { return counterGen2HeapSize.NextSample().RawValue; } }

        [HandleProcessCorruptedStateExceptions]
        static string GetCounterInstanceNameForProcess(int instanceCount, Process process) {
            try {
                string instanceName = Path.GetFileNameWithoutExtension(process.MainModule.FileName);

                if (instanceCount > 0)
                    instanceName += "#" + instanceCount.ToString();

                using (PerformanceCounter counter = new PerformanceCounter(CategoryNetClrMemory, ProcessId, instanceName, true)) {
                    long id = 0;

                    try {
                        for (;;) {
                            id = counter.NextSample().RawValue;

                            if (id > 0)
                                break;

                            Thread.Sleep(15);
                        }
                    }
                    catch (InvalidOperationException) {
                    }

                    return (id == process.Id) ? instanceName : null;
                }
            }
            catch {
                return null;
            }
        }

        string GetManagedPerformanceCounterInstanceName(Process process) {
            Func<int, Process, string> PidReader = GetCounterInstanceNameForProcess;
            string instanceName = null;
            AutoResetEvent evProcessFound = new AutoResetEvent(false);

            for (int i = 0; i < ProcessesToTry; i++) {
                int index = i;
                PidReader.BeginInvoke(index, process, (IAsyncResult result) => {
                    try {
                        if (instanceName == null) {
                            string correctInstanceName = PidReader.EndInvoke(result);

                            if (correctInstanceName != null) {
                                instanceName = correctInstanceName;
                                evProcessFound.Set();
                            }
                        }
                    }
                    catch {
                    }
                }, null);
            }


            if (!evProcessFound.WaitOne(1/*20*/ * 1000))
                throw new InvalidOperationException("Could not get managed performance counter instance name for process " + process.Id);

            return instanceName;
        }

        #region IDisposable Members
        public void Dispose() {
            counterBytesInAllHeaps.Dispose();
            counterLargeObjectHeapSize.Dispose();
            counterGen0HeapSize.Dispose();
            counterGen1HeapSize.Dispose();
            counterGen2HeapSize.Dispose();
        }
        #endregion
    }
}
