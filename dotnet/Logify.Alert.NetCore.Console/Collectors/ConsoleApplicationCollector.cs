using System;
using DevExpress.Logify.Core;
using System.Reflection;

namespace DevExpress.Logify.Console {
    class NetCoreConsoleApplicationCollector : ApplicationCollector {
        public override string AppName {
            get {
                try {
                    Assembly asm = Assembly.GetEntryAssembly();
                    if (asm == null)
                        return String.Empty;

                    foreach (AssemblyProductAttribute attr in asm.GetCustomAttributes<AssemblyProductAttribute>()) {
                        if (!String.IsNullOrEmpty(attr.Product))
                            return attr.Product;
                    }
                    foreach (AssemblyTitleAttribute attr in asm.GetCustomAttributes<AssemblyTitleAttribute>()) {
                        if (!String.IsNullOrEmpty(attr.Title))
                            return attr.Title;
                    }

                    return TryDetectAppNameByEntryPoint(asm);
                }
                catch {
                    return String.Empty;
                }
            }
        }
        public override string AppVersion {
            get {
                try {
                    Assembly asm = Assembly.GetEntryAssembly();
                    if (asm == null)
                        return String.Empty;

                    foreach (AssemblyInformationalVersionAttribute attr in asm.GetCustomAttributes<AssemblyInformationalVersionAttribute>()) {
                        if (!String.IsNullOrEmpty(attr.InformationalVersion))
                            return attr.InformationalVersion;
                    }
                    foreach (AssemblyVersionAttribute attr in asm.GetCustomAttributes<AssemblyVersionAttribute>()) {
                        if (!String.IsNullOrEmpty(attr.Version))
                            return attr.Version;
                    }
                    foreach (AssemblyFileVersionAttribute attr in asm.GetCustomAttributes<AssemblyFileVersionAttribute>()) {
                        if (!String.IsNullOrEmpty(attr.Version))
                            return attr.Version;
                    }
                }
                catch {
                }
                return String.Empty;
            }
        }

        public override string UserId { get { return String.Empty; } }
        

        public NetCoreConsoleApplicationCollector() : base() {}

        string TryDetectAppNameByEntryPoint(Assembly entryAssembly) {
            if (entryAssembly == null)
                return String.Empty;

            if (entryAssembly.EntryPoint == null)
                return String.Empty;
            Type mainType = entryAssembly.EntryPoint.DeclaringType;
            if (mainType == null)
                return String.Empty;

            string @namespace = mainType.Namespace;
            if (String.IsNullOrEmpty(@namespace))
                return mainType.Name;

            int lastDotIndex = @namespace.LastIndexOf('.');
            if (lastDotIndex >= 0 && lastDotIndex < @namespace.Length - 1)
                return @namespace.Substring(lastDotIndex + 1);
            else
                return @namespace;
        }
    }
}