using System;
using System.Web;
using System.Web.Configuration;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Web
{
    class WebApplicationCollector : ApplicationCollector {
        public override string AppName { get { return HttpContext.Current.Request.Url.AbsolutePath;  } }
        public override string AppVersion {
            get {
                string version = this.GetVersion();
                return Utils.ValidationVersion(version);
            }
        }
        public override string UserId { get { return String.Empty; } }
        

        public WebApplicationCollector() : base() {}

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
            object app = HttpContext.Current.ApplicationInstance;
            if (app == null) return String.Empty;

            Type type = app.GetType();
            while (type != null && type != typeof(object) && !type.BaseType.Name.Equals("HttpApplication"))
                type = type.BaseType;

            return type.Assembly.GetName().Version.ToString();
        }
    }
}