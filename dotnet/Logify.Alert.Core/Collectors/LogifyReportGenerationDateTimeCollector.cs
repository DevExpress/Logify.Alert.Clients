using System;
using System.Globalization;

namespace DevExpress.Logify.Core.Internal {
    public class LogifyReportGenerationDateTimeCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            DateTime utcNow = DateTime.UtcNow;
            string dateTimeString = utcNow.ToString("o", CultureInfo.InvariantCulture);
            logger.WriteValue("logifyReportDateTimeUtc", dateTimeString);
        }
    }
}
