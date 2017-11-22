using System;
using System.Configuration;

namespace DevExpress.Logify {
    internal class WebLogifyConfigSection : LogifyConfigSection {
        [ConfigurationProperty("version")]
        public ClientValueElement Version { get { return (ClientValueElement)base["version"]; } }

        [ConfigurationProperty("appName")]
        public ClientValueElement AppName { get { return (ClientValueElement)base["appName"]; } }

        [ConfigurationProperty("ignoreFormFields")]
        public ClientValueElement IgnoreFormFields { get { return (ClientValueElement)base["ignoreFormFields"]; } }
        [ConfigurationProperty("ignoreHeaders")]
        public ClientValueElement IgnoreHeaders { get { return (ClientValueElement)base["ignoreHeaders"]; } }
        [ConfigurationProperty("ignoreCookies")]
        public ClientValueElement IgnoreCookies { get { return (ClientValueElement)base["ignoreCookies"]; } }
        [ConfigurationProperty("ignoreServerVariables")]
        public ClientValueElement IgnoreServerVariables { get { return (ClientValueElement)base["ignoreServerVariables"]; } }
        [ConfigurationProperty("ignoreRequestBody")]
        public ClientValueElement IgnoreRequestBody { get { return (ClientValueElement)base["ignoreRequestBody"]; } }
    }
}
