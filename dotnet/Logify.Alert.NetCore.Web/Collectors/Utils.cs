using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using DevExpress.Logify.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace DevExpress.Logify.Web {
    static class Utils {
        public static void SerializeCookieInfo(IRequestCookieCollection cookies, ILogger logger) {
            if (cookies != null && cookies.Count != 0) {
                try {
                    logger.BeginWriteObject("cookie");
                    foreach (string key in cookies.Keys) {
                        string cookie = cookies[key];
                        logger.BeginWriteObject(key);
                        //logger.WriteValue("domain", cookie.Domain);
                        //logger.WriteValue("expires", cookie.Expires.ToString());
                        logger.WriteValue("name", key);
                        //logger.WriteValue("secure", cookie.Secure);
                        //logger.WriteValue("value", cookie.Value);
                        logger.WriteValue("value", cookie);
                        logger.EndWriteObject(key);
                    }
                }
                finally {
                    logger.EndWriteObject("cookie");
                }
            }
        }
        public static void SerializeInfo(IFormCollection info, string name, ILogger logger) {
            SerializeInfoCore(info, name, logger);
        }
        public static void SerializeInfo(IHeaderDictionary headers, string name, ILogger logger) {
            SerializeInfoCore(headers, name, logger);
        }
        public static void SerializeInfo(IQueryCollection query, string name, ILogger logger) {
            SerializeInfoCore(query, name, logger);
        }
        static void SerializeInfoCore(IEnumerable<KeyValuePair<string, StringValues>> info, string name, ILogger logger) {
            if (info == null)
                return;

            int written = 0;
            try {
                foreach (KeyValuePair<string, StringValues> pair in info) {
                    if (written == 0)
                        logger.BeginWriteObject(name);
                    written++;
                    logger.WriteValue(pair.Key, pair.Value.ToString());
                }
            }
            finally {
                if (written > 0)
                    logger.EndWriteObject(name);
            }
        }
        /*
        public static void SerializeInfo(QueryString queryString, string name, ILogger logger) {
            if (queryString == null || !queryString.HasValue)
                return;

            string content = queryString.Value;
            if (String.IsNullOrEmpty(content))
                return;

            string[] pairs = content.Split('&');
            if (pairs == null || pairs.Length <= 0)
                return;

            if (pairs[0].StartsWith("?"))
                pairs[0] = pairs[0].Substring(1);

            try {
                logger.BeginWriteObject(name);

                int count = pairs.Length;
                for (int i = 0; i < count; i++) {
                    QueryStringItem item = ParseQueryStringItem(pairs[i]);
                    if (item != null)
                        logger.WriteValue(item.Name, item.Value);
                }
            }
            finally {
                logger.EndWriteObject(name);
            }
        }

        static QueryStringItem ParseQueryStringItem(string content) {
            try {
                if (String.IsNullOrEmpty(content))
                    return null;

                int index = content.IndexOf('=');
                if (index < 0 || index >= content.Length)
                    return new QueryStringItem() { Name = content, Value = String.Empty };

                return new QueryStringItem() {
                    Name = content.Substring(0, index),
                    Value = content.Substring(index + 1)
                };
            }
            catch {
                return null;
            }
        }

        class QueryStringItem {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        */
        /*
        public static void SerializeInfo(NameValueCollection info, string name, ILogger logger) {
            if (info != null && info.Count != 0) {
                try {
                    logger.BeginWriteObject(name);
                    foreach (string key in info.AllKeys) {
                        logger.WriteValue(key, info.Get(key));
                    }
                }
                finally {
                    logger.EndWriteObject(name);
                }
            }
        }
        */
        public static string ValidationVersion(string version) {
            string validVersion;
            if (IsVersionInvalid(version)) {
                validVersion = String.Empty;
            } else {
                validVersion = version;
            }

            return validVersion;
        }

        static bool IsVersionInvalid(string version) {
            bool isInvalid = true;

            foreach (string token in version.Split('.')) {
                if (!token.Equals("0")) {
                    isInvalid = false;
                    break;
                }
            }

            return isInvalid;
        }
    }
}
