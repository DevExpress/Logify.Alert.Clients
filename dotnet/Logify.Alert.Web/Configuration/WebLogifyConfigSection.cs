using System;
using System.Configuration;

namespace DevExpress.Logify {
    internal class WebLogifyConfigSection : LogifyConfigSectionBase {
        [ConfigurationProperty("version", IsRequired = false)]
        public ClientValueElement Version { get { return (ClientValueElement)base["version"]; } }

        [ConfigurationProperty("appName", IsRequired = false)]
        public ClientValueElement AppName { get { return (ClientValueElement)base["appName"]; } }

        [ConfigurationProperty("ignoreFormFields", IsRequired = false)]
        public ClientValueElement IgnoreFormFields { get { return (ClientValueElement)base["ignoreFormFields"]; } }
        [ConfigurationProperty("ignoreHeaders", IsRequired = false)]
        public ClientValueElement IgnoreHeaders { get { return (ClientValueElement)base["ignoreHeaders"]; } }
        [ConfigurationProperty("ignoreCookies", IsRequired = false)]
        public ClientValueElement IgnoreCookies { get { return (ClientValueElement)base["ignoreCookies"]; } }
        [ConfigurationProperty("ignoreServerVariables", IsRequired = false)]
        public ClientValueElement IgnoreServerVariables { get { return (ClientValueElement)base["ignoreServerVariables"]; } }
        [ConfigurationProperty("ignoreRequestBody", IsRequired = false)]
        public ClientValueElement IgnoreRequestBody { get { return (ClientValueElement)base["ignoreRequestBody"]; } }
    }
}
