using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace DevExpress.Logify.Core.Internal {
    public class BreadcrumbsCollector : IInfoCollector {
        readonly BreadcrumbCollection breadcrumbs;

        public BreadcrumbsCollector(BreadcrumbCollection breadcrumbs) {
            this.breadcrumbs = breadcrumbs;
        }

        public void Process(Exception ex, ILogger logger) {
            if (breadcrumbs == null || breadcrumbs.Count <= 0)
                return;

            //string dump = DumpBreadcrumbs(breadcrumbs);

            int totalBreadcrumbsize = 0;
            const int maxTotalBreadcrumbsize = 3 * 1024 * 1024; // 3Mb
            logger.BeginWriteArray("breadcrumbs");
            //for (int i = breadcrumbs.Count - 1; i >= 0; i--) { // write breadcrumbs from end to beginning, keeping latest items
            foreach (Breadcrumb item in breadcrumbs) { // time backward order
                //Breadcrumb item = breadcrumbs[i];
                BreadcrumbCollector collector = new BreadcrumbCollector(item, totalBreadcrumbsize, maxTotalBreadcrumbsize, String.Empty);
                int writtenContentSize = collector.PerformProcess(ex, logger);
                totalBreadcrumbsize += writtenContentSize;
            }
            logger.EndWriteArray("breadcrumbs");
        }
#if DEBUG
        string DumpBreadcrumbs(BreadcrumbCollection breadcrumbs) {
            StringBuilder content = new StringBuilder();
            content.AppendLine("BreadcrumbCollection breadcrumbs = new BreadcrumbCollection();");
            foreach (Breadcrumb item in breadcrumbs) {
                DumpBreadcrumb(item, content);
            }
            return content.ToString();
        }

        void DumpBreadcrumb(Breadcrumb item, StringBuilder content) {
            content.AppendLine("breadcrumbs.Add(");
            content.AppendLine("    new Breadcrumb() {");
            if (item.Event != BreadcrumbEvent.None) {
                string text = item.Event.ToString();
                text = Char.ToUpper(text[0]) + text.Substring(1);
                content.AppendFormat("        Event = BreadcrumbEvent.{0},\r\n", text);
            }
            if (item.Level != BreadcrumbLevel.None)
                content.AppendFormat("        Level = BreadcrumbLevel.{0},\r\n", item.Level);
            if (!String.IsNullOrEmpty(item.ThreadId))
                content.AppendFormat("        ThreadId = \"{0}\",\r\n", item.ThreadId);
            if (!String.IsNullOrEmpty(item.Category))
                content.AppendFormat("        Category = \"{0}\",\r\n", item.Category);
            if (!String.IsNullOrEmpty(item.ClassName))
                content.AppendFormat("        ClassName = \"{0}\",\r\n", item.ClassName);
            if (!String.IsNullOrEmpty(item.MethodName))
                content.AppendFormat("        MethodName = \"{0}\",\r\n", item.MethodName);
            if (item.Line != 0)
                content.AppendFormat("        Line = {0},\r\n", item.Line);
            if (item.DateTime != DateTime.Now)
                content.AppendFormat("        DateTime = DateTime.ParseExact(\"{0}\", \"o\", CultureInfo.InvariantCulture),\r\n", item.DateTime.ToString("o", CultureInfo.InvariantCulture));
            if (item.CustomData != null && item.CustomData.Count > 0) {
                content.AppendLine("        CustomData = new Dictionary<string, string>() {");
                foreach (string key in item.CustomData.Keys) {
                    content.AppendFormat("            {{ \"{0}\", \"{1}\" }},\r\n", key, item.CustomData[key]);
                }
                content.AppendLine("        }");
            }
            content.AppendLine("    }");
            content.AppendLine(");");
        }
#endif
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
                result += CalculateTextSize("dateTime", item.DateTime.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));
                result += CalculateTextSize("level", item.Level.ToString());
                result += CalculateTextSize("event", item.Event.ToString());
                result += CalculateTextSize("category", item.Category);
                result += CalculateTextSize("message", item.Message);
                result += CalculateTextSize("className", item.ClassName);
                result += CalculateTextSize("methodName", item.MethodName);
                result += CalculateTextSize("line", item.Line.ToString());
                result += CalculateTextSize("threadId", item.ThreadId);
                if (item.IsAuto)
                    result += CalculateTextSize("a", item.IsAuto.ToString());

                if (item.CustomData != null && item.CustomData.Count > 0) {
                    foreach (string key in item.CustomData.Keys) {
                        result += CalculateTextSize(key, item.CustomData[key]);
                    }
                }
            }
            catch {
            }
            return result;
        }
        int CalculateTextSize(string name, string text) {
            if (String.IsNullOrEmpty(text))
                return 0;
            return name.Length + text.Length;
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
                    if (item.IsAuto)
                        logger.WriteValue("a", "1");

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