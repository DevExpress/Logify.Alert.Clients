using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    static class Utils {
        public static void SerializeCookieInfo(HttpCookieCollection cookies, ILogger logger) {
            if (cookies != null && cookies.Count != 0) {
                try {
                    logger.BeginWriteObject("cookie");
                    foreach (string key in cookies.AllKeys) {
                        HttpCookie cookie = cookies.Get(key);
                        if (String.IsNullOrEmpty(key)) 
                            continue;
                        
                        logger.BeginWriteObject(key);
                        logger.WriteValue("domain", cookie.Domain);
                        logger.WriteValue("expires", cookie.Expires.ToString());
                        logger.WriteValue("name", cookie.Name);
                        logger.WriteValue("secure", cookie.Secure);
                        logger.WriteValue("value", cookie.Value);
                        logger.EndWriteObject(key);
                    }
                }
                finally {
                    logger.EndWriteObject("cookie");
                }
            }
        }

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
