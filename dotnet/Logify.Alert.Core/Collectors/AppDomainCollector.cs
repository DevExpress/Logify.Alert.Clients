using System;
using System.Reflection;
using DevExpress.Logify.Core;

#if NETSTANDARD && !NETSTANDARD1_4
using Microsoft.Extensions.DependencyModel;
#endif

namespace DevExpress.Logify.Core.Internal {
#if NETSTANDARD
    public class AppDomainCollector : IInfoCollector {
        readonly string name;

        public AppDomainCollector(string name) {
            this.name = name;
        }
        public virtual void Process(Exception ex, ILogger logger) {
#if !NETSTANDARD1_4
            logger.BeginWriteObject(name);
            try {
                //logger.WriteValue("friendlyName", domain.FriendlyName); // AppImageName
                DumpAssemblies(logger);
                //etc
            }
            finally {
                logger.EndWriteObject(name);
            }
#endif
        }

#if !NETSTANDARD1_4
        void DumpAssemblies(ILogger logger) {
            logger.BeginWriteArray("assemblies");
            try {
                try {
                    var assemblies = DependencyContext.Default.RuntimeLibraries;
                    int count = assemblies.Count;
                    for (int i = 0; i < count; i++) {
                        try {
                            RuntimeLibraryCollector collector = new RuntimeLibraryCollector(assemblies[i]);
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
#endif
        }
#else
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
#endif
}
