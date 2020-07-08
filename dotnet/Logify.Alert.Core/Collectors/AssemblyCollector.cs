using System;
using System.Diagnostics;
using System.Reflection;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
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
                logger.WriteValue("gac", asm.GlobalAssemblyCache);
                logger.WriteValue("fullTrust", asm.IsFullyTrusted);
                try {
                    if (!asm.IsDynamic && !string.IsNullOrEmpty(asm.Location)) {
                        FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(asm.Location);
                        logger.WriteValue("fileVersion", fileVersion.FileVersion);
                    }
                }
                catch {
                }
                //etc.
            }
            finally {
                logger.EndWriteObject(name);
            }
        }
    }
}
