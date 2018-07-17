#if NETSTANDARD
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DevExpress.Logify.Core.Internal {
    public static class ClientConfigurationLoader {
        [CLSCompliant(false)]
        public static LogifyAlertConfiguration LoadConfiguration(IConfigurationSection section) {
            LogifyAlertConfiguration config = new LogifyAlertConfiguration();
            if (section == null)
                return config;

            section.Bind(config);
            return config;
        }
    }
}
#else
using System;
using System.Collections.Generic;
using System.Configuration;

namespace DevExpress.Logify {
    public class LogifyConfigSectionBase : ConfigurationSection {
        [ConfigurationProperty("serviceUrl", IsRequired = false)]
        public ClientValueElement ServiceUrl { get { return (ClientValueElement)base["serviceUrl"]; } }

        [ConfigurationProperty("miniDumpServiceUrl", IsRequired = false)]
        public ClientValueElement MiniDumpServiceUrl { get { return (ClientValueElement)base["miniDumpServiceUrl"]; } }

        [ConfigurationProperty("apiKey")]
        public ClientValueElement ApiKey { get { return (ClientValueElement)base["apiKey"]; } }
        [ConfigurationProperty("confirmSend", IsRequired = false)]
        public ClientValueElement ConfirmSend { get { return (ClientValueElement)base["confirmSend"]; } }
        //[ConfigurationProperty("logId")]
        //public ValueElement LogId { get { return (ValueElement)base["logId"]; } }
        [ConfigurationProperty("customData", IsDefaultCollection = false, IsRequired = false)]
        public KeyValueConfigurationCollection CustomData { get { return (KeyValueConfigurationCollection)base["customData"]; } }
        [ConfigurationProperty("tags", IsDefaultCollection = false, IsRequired = false)]
        public KeyValueConfigurationCollection Tags { get { return (KeyValueConfigurationCollection)base["tags"]; } }

        [ConfigurationProperty("offlineReportsEnabled", IsRequired = false)]
        public ClientValueElement OfflineReportsEnabled { get { return (ClientValueElement)base["offlineReportsEnabled"]; } }
        [ConfigurationProperty("offlineReportsDirectory", IsRequired = false)]
        public ClientValueElement OfflineReportsDirectory { get { return (ClientValueElement)base["offlineReportsDirectory"]; } }
        [ConfigurationProperty("offlineReportsCount", IsRequired = false)]
        public ClientValueElement OfflineReportsCount { get { return (ClientValueElement)base["offlineReportsCount"]; } }

        [ConfigurationProperty("collectMiniDump", IsRequired = false)]
        public ClientValueElement CollectMiniDump { get { return (ClientValueElement)base["collectMiniDump"]; } }
        [ConfigurationProperty("collectBreadcrumbs", IsRequired = false)]
        public ClientValueElement CollectBreadcrumbs { get { return (ClientValueElement)base["collectBreadcrumbs"]; } }
        [ConfigurationProperty("breadcrumbsMaxCount", IsRequired = false)]
        public ClientValueElement BreadcrumbsMaxCount { get { return (ClientValueElement)base["breadcrumbsMaxCount"]; } }

        [ConfigurationProperty("allowRemoteConfiguration", IsRequired = false)]
        public ClientValueElement AllowRemoteConfiguration { get { return (ClientValueElement)base["allowRemoteConfiguration"]; } }
        [ConfigurationProperty("remoteConfigurationFetchInterval", IsRequired = false)]
        public ClientValueElement RemoteConfigurationFetchInterval { get { return (ClientValueElement)base["remoteConfigurationFetchInterval"]; } }
    }

    public class ClientValueElement : ConfigurationElement {
        [ConfigurationProperty("value", DefaultValue = "", IsKey = true/*, IsRequired = true*/)]
        public string Value {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }

        public bool ValueAsBool {
            get {
                string value = Value;
                if (String.IsNullOrEmpty(value))
                    return false;
                if (String.Compare(value, "yes", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                    String.Compare(value, "y", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                    String.Compare(value, "true", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                    String.Compare(value, "t", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                    String.Compare(value, "1", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                    String.Compare(value, "on", StringComparison.InvariantCultureIgnoreCase) == 0)
                    return true;
                return false;

            }
        }
        public int ValueAsInt {
            get {
                string value = Value;
                if (String.IsNullOrEmpty(value))
                    return 0;
                int result;
                if (Int32.TryParse(value, out result))
                    return result;
                return 0;
            }
        }
    }
}

namespace DevExpress.Logify.Core.Internal {
    public static class ClientConfigurationLoader {
        public static void ConfigureClientFromFile(LogifyClientBase client, string configFileName) {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = configFileName;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            LogifyConfigSectionBase section = config.GetSection("logifyAlert") as LogifyConfigSectionBase;
            LogifyClientAccessor.Configure(client, LoadCommonConfiguration(section));
        }
        public static LogifyAlertConfiguration LoadCommonConfiguration(LogifyConfigSectionBase section) {
            LogifyAlertConfiguration config = new LogifyAlertConfiguration();
            if (section == null)
                return config;

            if (section.ServiceUrl != null && !String.IsNullOrEmpty(section.ServiceUrl.Value))
                config.ServiceUrl = section.ServiceUrl.Value;
            //if (section.LogId != null)
            //    reportSender.LogId = section.LogId.Value;
            if (section.ApiKey != null)
                config.ApiKey = section.ApiKey.Value;
            if (section.ConfirmSend != null)
                config.ConfirmSend = section.ConfirmSend.ValueAsBool;

            if (section.AllowRemoteConfiguration != null)
                config.AllowRemoteConfiguration = section.AllowRemoteConfiguration.ValueAsBool;
            if (section.RemoteConfigurationFetchInterval != null) {
                int interval = section.RemoteConfigurationFetchInterval.ValueAsInt;
                if (interval <= 0)
                    interval = 10;
                config.RemoteConfigurationFetchInterval = interval;
            }

            //if (section.MiniDumpServiceUrl != null)
            //    client.MiniDumpServiceUrl = section.MiniDumpServiceUrl.Value;

            if (section.OfflineReportsEnabled != null)
                config.OfflineReportsEnabled = section.OfflineReportsEnabled.ValueAsBool;
            if (section.OfflineReportsDirectory != null)
                config.OfflineReportsDirectory = section.OfflineReportsDirectory.Value;
            if (section.OfflineReportsCount != null)
                config.OfflineReportsCount = section.OfflineReportsCount.ValueAsInt;

            if (section.CollectMiniDump != null)
                config.CollectMiniDump = section.CollectMiniDump.ValueAsBool;
            if (section.CollectBreadcrumbs != null)
                config.CollectBreadcrumbs = section.CollectBreadcrumbs.ValueAsBool;
            if (section.BreadcrumbsMaxCount != null) {
                int value = section.BreadcrumbsMaxCount.ValueAsInt;
                if (value > 1)
                    config.BreadcrumbsMaxCount = value;
            }

            config.CustomData = CreateDictionary(section.CustomData);
            config.Tags = CreateDictionary(section.Tags);
            return config;
        }
        static Dictionary<string, string> CreateDictionary(KeyValueConfigurationCollection src) {
            if (src == null || src.Count <= 0)
                return null;
            Dictionary<string, string> result = new Dictionary<string, string>(src.Count);
            foreach (KeyValueConfigurationElement element in src)
                result[element.Key] = element.Value;
            return result;
        }
    }
}
#endif