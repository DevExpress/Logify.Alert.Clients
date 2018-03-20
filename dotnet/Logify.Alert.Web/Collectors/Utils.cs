using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace DevExpress.Logify.Core.Internal {
    static class Utils {
        public static void SerializeCookieInfo(HttpCookieCollection cookies, IgnorePropertiesInfo ignoreInfo, ILogger logger) {
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
                        logger.WriteValue("secure", cookie.Secure);//always false at this stage
                        //logger.WriteValue("httpOnly", cookie.HttpOnly);

                        if (ignoreInfo != null && ignoreInfo.ShouldIgnore(key))
                            logger.WriteValue("value", RequestBodyFilter.ValueStripped);
                        else
                            logger.WriteValue("value", cookie.Value);
                        logger.EndWriteObject(key);
                    }
                }
                finally {
                    logger.EndWriteObject("cookie");
                }
            }
        }

        public static Dictionary<string, string> SerializeInfo(NameValueCollection info, string name, IgnorePropertiesInfo ignoreInfo, Predicate<string> customIgnorePredicate, ILogger logger) {
            Dictionary<string, string> result = null;
            if (info != null && info.Count != 0) {
                try {
                    logger.BeginWriteObject(name);
                    foreach (string key in info.AllKeys) {
                        if (customIgnorePredicate != null && customIgnorePredicate(key)) {
                            continue;
                        } else if (ignoreInfo != null && ignoreInfo.ShouldIgnore(key)) {
                            if (result == null)
                                result = new Dictionary<string, string>();
                            result[key] = info.Get(key);

                            logger.WriteValue(key, RequestBodyFilter.ValueStripped);
                        }
                        else
                            logger.WriteValue(key, info.Get(key));
                    }

                    
                }
                finally {
                    logger.EndWriteObject(name);
                }
            }
            return result;
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
