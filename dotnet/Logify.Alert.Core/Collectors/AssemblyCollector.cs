using System;
using System.Diagnostics;
using System.Reflection;
using DevExpress.Logify.Core;

#if NETSTANDARD && !NETSTANDARD1_4
using Microsoft.Extensions.DependencyModel;
#endif

namespace DevExpress.Logify.Core {
    public class AssemblyCollector : IInfoCollector {
        readonly Assembly asm;
        readonly string name;

        public AssemblyCollector(Assembly asm) {
            this.asm = asm;
        }
        public AssemblyCollector(Assembly asm, string name) {
            this.asm = asm;
            this.name = name;
        }
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject(name);
            try {
                logger.WriteValue("fullName", asm.FullName);
                logger.WriteValue("dynamic", asm.IsDynamic);
#if !NETSTANDARD
                logger.WriteValue("gac", asm.GlobalAssemblyCache);
                logger.WriteValue("fullTrust", asm.IsFullyTrusted);
                try {
                    if (!asm.IsDynamic) {
                        FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(asm.Location);
                        logger.WriteValue("fileVersion", fileVersion.FileVersion);
                    }
                }
                catch {
                }
                //etc.
#endif
            }
            finally {
                logger.EndWriteObject(name);
            }
        }
    }

#if NETSTANDARD && !NETSTANDARD1_4
    [CLSCompliant(false)]
    public class RuntimeLibraryCollector : IInfoCollector {
        readonly RuntimeLibrary asm;
        readonly string name;

        public RuntimeLibraryCollector(RuntimeLibrary asm) {
            this.asm = asm;
        }
        public RuntimeLibraryCollector(RuntimeLibrary asm, string name) {
            this.asm = asm;
            this.name = name;
        }
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject(name);
            try {
                logger.WriteValue("fullName", asm.Name + ", Version=" + asm.Version);
                //logger.WriteValue("dynamic", asm.IsDynamic);

                //logger.WriteValue("gac", asm.GlobalAssemblyCache);
                //logger.WriteValue("fullTrust", asm.IsFullyTrusted);
                //try {
                //    FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(asm.Location);
                //    logger.WriteValue("fileVersion", fileVersion.FileVersion);
                //}
                //catch {
                //}
                //etc.
            }
            finally {
                logger.EndWriteObject(name);
            }
        }
    }
#endif
}
