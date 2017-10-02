using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DevExpress.Logify.Core {
    public class BreadcrumbsCollector : IInfoCollector {
        readonly BreadcrumbCollection breadcrumbs;

        public BreadcrumbsCollector(BreadcrumbCollection breadcrumbs) {
            this.breadcrumbs = breadcrumbs;
        }

        public void Process(Exception ex, ILogger logger) {
            if (breadcrumbs == null || breadcrumbs.Count <= 0)
                return;

            int totalBreadcrumbsize = 0;
            const int maxTotalBreadcrumbsize = 3 * 1024 * 1024; // 3Mb
            logger.BeginWriteArray("breadcrumbs");
            for (int i = breadcrumbs.Count - 1; i >= 0; i--) { // write breadcrumbs from end to beginning, keeping latest items
                Breadcrumb item = breadcrumbs[i];
                BreadcrumbCollector collector = new BreadcrumbCollector(item, totalBreadcrumbsize, maxTotalBreadcrumbsize, String.Empty);
                int writtenContentSize = collector.PerformProcess(ex, logger);
                totalBreadcrumbsize += writtenContentSize;
            }
            logger.EndWriteArray("breadcrumbs");
        }
    }
    public class BreadcrumbCollector : IInfoCollector {
        readonly Breadcrumb item;
        int totalBreadcrumbsize;
        int maxTotalBreadcrumbsize;
        string nodeName;

        public BreadcrumbCollector(Breadcrumb item, int totalBreadcrumbsize, int maxTotalBreadcrumbsize, string nodeName) {
            this.item = item;
            this.totalBreadcrumbsize = totalBreadcrumbsize;
            this.maxTotalBreadcrumbsize = maxTotalBreadcrumbsize;
            this.nodeName = nodeName;
        }

        public void Process(Exception ex, ILogger logger) {
            PerformProcess(ex, logger);
        }
        int CalculateApproxItemSize() {
            int result = 0;
            try {
                result += CalculateTextSize(item.DateTime.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));
                result += CalculateTextSize(item.Level.ToString());
                result += CalculateTextSize(item.Event.ToString());
                result += CalculateTextSize(item.Category);
                result += CalculateTextSize(item.Message);
                result += CalculateTextSize(item.ClassName);
                result += CalculateTextSize(item.MethodName);
                result += CalculateTextSize(item.Line.ToString());
                result += CalculateTextSize(item.ThreadId);

                if (item.CustomData != null && item.CustomData.Count > 0) {
                    foreach (string key in item.CustomData.Keys) {
                        result += CalculateTextSize(key);
                        result += CalculateTextSize(item.CustomData[key]);
                    }
                }
            }
            catch {
            }
            return result;
        }
        int CalculateTextSize(string text) {
            if (String.IsNullOrEmpty(text))
                return 0;
            return text.Length;
        }
        public int PerformProcess(Exception ex, ILogger logger) {
            int writtenContentSize = 0;
            try {
                if (item == null)
                    return 0;

                writtenContentSize = CalculateApproxItemSize();
                if (totalBreadcrumbsize + writtenContentSize > maxTotalBreadcrumbsize)
                    return 0; // do not store breadcrumbs exceeding size limit

                logger.BeginWriteObject(nodeName);
                try {
                    logger.WriteValue("dateTime", item.DateTime.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));
                    if (item.Level != BreadcrumbLevel.None)
                        logger.WriteValue("level", item.Level.ToString());
                    if (item.Event != BreadcrumbEvent.None)
                        logger.WriteValue("event", item.Event.ToString());
                    if (!String.IsNullOrEmpty(item.Category))
                        logger.WriteValue("category", item.Category);
                    if (!String.IsNullOrEmpty(item.Message))
                        logger.WriteValue("message", item.Message);
                    if (!String.IsNullOrEmpty(item.ClassName))
                        logger.WriteValue("className", item.ClassName);
                    if (!String.IsNullOrEmpty(item.MethodName))
                        logger.WriteValue("methodName", item.MethodName);
                    if (item.Line != 0)
                        logger.WriteValue("line", item.Line);
                    if (!String.IsNullOrEmpty(item.ThreadId))
                        logger.WriteValue("threadId", item.ThreadId);

                    CustomDataCollector customDataCollector = new CustomDataCollector(item.CustomData, null);
                    customDataCollector.Process(ex, logger);
                }
                finally {
                    logger.EndWriteObject(nodeName);
                }
            }
            catch {
            }
            return writtenContentSize;
        }
    }
}