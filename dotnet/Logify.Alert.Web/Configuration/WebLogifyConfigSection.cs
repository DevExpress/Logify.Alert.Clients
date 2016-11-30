using System;
using System.Configuration;

namespace DevExpress.Logify {
    internal class WebLogifyConfigSection: LogifyConfigSection {
        [ConfigurationProperty("version")]
        public ClientValueElement Version { get { return (ClientValueElement)base["version"]; } }

        [ConfigurationProperty("appName")]
        public ClientValueElement AppName { get { return (ClientValueElement)base["appName"]; } }
    }
}
