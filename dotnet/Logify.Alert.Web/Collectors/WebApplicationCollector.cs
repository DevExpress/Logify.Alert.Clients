using System;
using System.Web;
using System.Web.Configuration;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    class WebApplicationCollector : ApplicationCollector {
        readonly HttpContext context;
        public WebApplicationCollector(HttpContext context) : base() {
            this.context = context;
        }

        public override string AppName {
            get {
                try {
                    if (context != null && context.Request != null && context.Request.Url != null)
                        return context.Request.Url.AbsolutePath;
                }
                catch {
                }
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
            string version = String.Empty;

            WebLogifyConfigSection configSection = (WebLogifyConfigSection)WebConfigurationManager.GetSection("logifyAlert");
            if (configSection.Version != null) {
                version = configSection.Version.Value;
            }

            return version;
        }

        string TryDetectVersion() {
            try {
                if (context == null)
                    return String.Empty;

                object app = context.ApplicationInstance;
                if (app == null)
                    return String.Empty;

                Type type = app.GetType();
                while (type != null && type != typeof(object) && !type.BaseType.Name.Equals("HttpApplication"))
                    type = type.BaseType;

                return type.Assembly.GetName().Version.ToString();
            }
            catch {
                return String.Empty;
            }
        }
    }
}