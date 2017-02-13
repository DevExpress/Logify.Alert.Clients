using System.Collections.Generic;

namespace DevExpress.Logify.Web {
    internal class LogifyAlertConfiguration {
        public string ServiceUrl { get; set; }
        public string MiniDumpServiceUrl { get; set; }
        public string ApiKey { get; set; }
        public bool ConfirmSend { get; set; }
        public bool OfflineReportsEnabled { get; set; }
        public string OfflineReportsDirectory { get; set; }
        public int OfflineReportsCount { get; set; }
        public Dictionary<string, string> CustomData { get; set; }
    }
}