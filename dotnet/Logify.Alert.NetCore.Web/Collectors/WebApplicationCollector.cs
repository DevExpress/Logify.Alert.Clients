using System;
using Microsoft.AspNetCore.Http;

namespace DevExpress.Logify.Core.Internal {
    class NetCoreWebApplicationCollector : ApplicationCollector {
        readonly HttpContext context;
        public NetCoreWebApplicationCollector(HttpContext context) : base() {
            this.context = context;
        }
        public override string AppName {
            get {
                if (context != null && context.Request != null)
                    return Utils.GetRequestFullPath(context.Request);
                return String.Empty;
            }
        }
        public override string AppVersion {
            get {
                string version = this.GetVersion();
                return Utils.ValidationVersion(version);
            }
        }
        public override string UserId { get { return String.Empty; } }
        
        string GetVersion() {
            string version = this.TryGetVersionFromConfig();
            if (String.IsNullOrEmpty(version)) {
                version = this.TryDetectVersion();
            }

            return version;
        }

        string TryGetVersionFromConfig() {
            return String.Empty;
            /*
            string version = String.Empty;

            WebLogifyConfigSection configSection = (WebLogifyConfigSection)WebConfigurationManager.GetSection("logifyAlert");
            if (configSection.Version != null) {
                version = configSection.Version.Value;
            }

            return version;
            */
        }

        string TryDetectVersion() {
            return String.Empty;
            /*
            HttpContext current = LogifyHttpContext.Current;
            if (current == null)
                return String.Empty;

            object app = current.ApplicationInstance;
            if (app == null)
                return String.Empty;

            Type type = app.GetType();
            while (type != null && type != typeof(object) && !type.BaseType.Name.Equals("HttpApplication"))
                type = type.BaseType;

            return type.Assembly.GetName().Version.ToString();
            */
        }
    }
}