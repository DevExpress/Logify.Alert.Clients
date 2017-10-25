using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {

    class ModulesCollector : IInfoCollector {

        readonly HttpModuleCollection modules;
        readonly string name;

        public ModulesCollector(HttpModuleCollection modules) : this(modules, "modules") { }

        public ModulesCollector(HttpModuleCollection modules, string name) {
            this.modules = modules;
            this.name = name;
        }

        public void Process(Exception e, ILogger logger) {
            if (modules.Count != 0) {
                try {
                    logger.WriteValue(name, modules.Cast<string>().ToArray());
                } catch {}
            }
        }
    }
}