using System;
using System.Reflection;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    public class AppDomainCollector : IInfoCollector {
        readonly AppDomain domain;
        readonly string name;

        public AppDomainCollector(AppDomain domain) {
            this.domain = domain;
        }
        public AppDomainCollector(AppDomain domain, string name) {
            this.domain = domain;
            this.name = name;
        }
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject(name);
            try {
                logger.WriteValue("friendlyName", domain.FriendlyName);
                DumpAssemblies(logger);
                //etc
            }
            finally {
                logger.EndWriteObject(name);
            }
        }
        void DumpAssemblies(ILogger logger) {
            logger.BeginWriteArray("assemblies");
            try {
                try {
                    Assembly[] assemblies = domain.GetAssemblies();
                    int count = assemblies.Length;
                    for (int i = 0; i < count; i++) {
                        try {
                            AssemblyCollector collector = new AssemblyCollector(assemblies[i]);
                            collector.Process(null, logger);
                        }
                        catch {
                        }
                    }
                    //etc.
                }
                catch {
                }
            }
            finally {
                logger.EndWriteArray("assemblies");
            }
        }
    }
}
