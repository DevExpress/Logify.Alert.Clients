using System;
using System.Configuration;

namespace DevExpress.Logify {
    public class LogifyConfigSection : ConfigurationSection {
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
        [ConfigurationProperty("customData", IsDefaultCollection = true, IsRequired = false)]
        public KeyValueConfigurationCollection CustomData { get { return (KeyValueConfigurationCollection)base["customData"]; } }
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
    }
}

namespace DevExpress.Logify.Core {
    public static class ClientConfigurationLoader {
        public static void ApplyClientConfiguration(LogifyClientBase client) {
            LogifyConfigSection section = ConfigurationManager.GetSection("logifyAlert") as LogifyConfigSection;
            if (section != null) {
                if (section.ServiceUrl != null && !String.IsNullOrEmpty(section.ServiceUrl.Value))
                    client.ServiceUrl = section.ServiceUrl.Value;
                //if (section.LogId != null)
                //    reportSender.LogId = section.LogId.Value;
                if (section.ApiKey != null)
                    client.ApiKey = section.ApiKey.Value;
                if (section.ConfirmSend != null)
                    client.ConfirmSendReport = section.ConfirmSend.ValueAsBool;

                if (section.MiniDumpServiceUrl != null)
                    client.MiniDumpServiceUrl = section.MiniDumpServiceUrl.Value;

                if (section.CustomData != null && section.CustomData.Count > 0) {
                    foreach (KeyValueConfigurationElement element in section.CustomData)
                        client.CustomData[element.Key] = element.Value;
                }
            }
        }
    }
}