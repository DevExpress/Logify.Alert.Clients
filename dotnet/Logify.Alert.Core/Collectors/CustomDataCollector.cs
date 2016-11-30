using System;
using System.Collections.Generic;

namespace DevExpress.Logify.Core {
    public class CustomDataCollector : IInfoCollector {
        readonly IDictionary<string, string> customData;

        public CustomDataCollector(IDictionary<string, string> customData, IDictionary<string, string> additionalCustomData) {
            this.customData = MergeData(customData, additionalCustomData);
        }

        IDictionary<string, string> MergeData(IDictionary<string, string> customData, IDictionary<string, string> additionalCustomData) {
            try {
                if (customData == null || customData.Count <= 0)
                    return additionalCustomData;
                if (additionalCustomData == null || additionalCustomData.Count <= 0)
                    return customData;

                Dictionary<string, string> result = new Dictionary<string, string>(customData);
                foreach (KeyValuePair<string, string> item in additionalCustomData)
                    result[item.Key] = item.Value;
                return result;
            }
            catch {
                return customData;
            }
        }

        public void Process(Exception ex, ILogger logger) {
            if (customData == null || customData.Count <= 0)
                return;

            logger.BeginWriteObject("customData");
            foreach (string key in customData.Keys)
                logger.WriteValue(key, customData[key]);
            logger.EndWriteObject("customData");
        }
    }
}