using System;
using System.Management;
using System.Runtime.ExceptionServices;

namespace DevExpress.Logify.Core {
    public class VirtualMachineCollector : IInfoCollector {
        [HandleProcessCorruptedStateExceptions]
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("vm");
            try {
                logger.WriteValue("name", DetectVm());
                //etc
            }
            finally {
                logger.EndWriteObject("vm");
            }
        }
        bool reentrance;
        string DetectVm() {
            if (reentrance)
                return String.Empty;
            reentrance = true;
            try {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem")) {
                    using (var items = searcher.Get()) {
                        foreach (var item in items) {

                            string manufacturer = item["Manufacturer"].ToString().ToLower();
                            string model = item["Model"].ToString();
                            if (IsMSVm(manufacturer, model))
                                return "HyperV";
                            if (IsVmwareVm(manufacturer, model))
                                return "VMWare";
                            if (IsVirtualBoxVm(manufacturer, model))
                                return "VirtualBox";
                        }
                    }
                }
                return String.Empty;
            }
            catch {
                return String.Empty;
            }
            finally {
                reentrance = false;
            }
        }
        bool IsMSVm(string manufacturer, string model) {
            return (manufacturer == "microsoft corporation") && model.ToLowerInvariant().Contains("virtual");
        }
        bool IsVmwareVm(string manufacturer, string model) {
            return manufacturer.Contains("vmware");
        }
        bool IsVirtualBoxVm(string manufacturer, string model) {
            return String.Compare(model, "VirtualBox", StringComparison.CurrentCultureIgnoreCase) == 0;
        }
    }
}