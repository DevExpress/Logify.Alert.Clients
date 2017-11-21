using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

using DevExpress.Logify.Core;
using System.Collections.Generic;

namespace DevExpress.Logify.Core.Internal {
    class RequestCollector : IInfoCollector {
        readonly HttpRequest request;
        readonly string name;
        readonly IgnorePropertiesInfo ignoreFormFields;
        readonly IgnorePropertiesInfo ignoreHeaders;
        readonly IgnorePropertiesInfo ignoreCookies;
        readonly IgnorePropertiesInfo ignoreServerVariables;
        private ILogger logger;

        public RequestCollector(HttpRequest request, IgnorePropertiesInfoConfig ignoreConfig) : this(request, "request", ignoreConfig) { }

        public RequestCollector(HttpRequest request, string name, IgnorePropertiesInfoConfig ignoreConfig) {
            this.request = request;
            this.name = name;
            this.ignoreFormFields = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreFormFields);
            this.ignoreHeaders = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreHeaders);
            this.ignoreCookies = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreCookies);
            this.ignoreServerVariables = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreServerVariables);
        }

        public virtual void Process(Exception e, ILogger logger) {
            this.logger = logger;
            try {
                logger.BeginWriteObject(name);
                logger.WriteValue("acceptTypes", request.AcceptTypes);
                this.SerializeBrowserInfo();
                this.SerializeContentInfo();
                this.SerializeFileInfo();
                Utils.SerializeCookieInfo(request.Cookies, this.ignoreCookies, logger);
                Utils.SerializeInfo(request.Form, "form", this.ignoreFormFields, logger);
                Utils.SerializeInfo(request.Headers, "headers", this.ignoreHeaders, logger);
                Utils.SerializeInfo(request.ServerVariables, "serverVariables", this.ignoreServerVariables, logger);
                Utils.SerializeInfo(request.QueryString, "queryString", null, logger);
                logger.WriteValue("applicationPath", request.ApplicationPath);
                logger.WriteValue("httpMethod", request.HttpMethod);
                try { logger.WriteValue("httpRequestBody", (new System.IO.StreamReader(request.InputStream)).ReadToEnd()); } catch { }
                logger.WriteValue("isUserAuthenticated", request.IsAuthenticated);
                logger.WriteValue("isLocalRequest", request.IsLocal);
                logger.WriteValue("isSecureConnection", request.IsSecureConnection);
                logger.WriteValue("requestType", request.RequestType);
                logger.WriteValue("totalBytes", request.TotalBytes);
            }
            finally {
                logger.EndWriteObject(name);
            }
        }

        void SerializeBrowserInfo() {
            try {
                logger.BeginWriteObject("browser");
                HttpBrowserCapabilities browserInfo = request.Browser;
                logger.WriteValue("activeXSupport", browserInfo.ActiveXControls);
                logger.WriteValue("isBeta", browserInfo.Beta);
                logger.WriteValue("userAgent", browserInfo.Browser);
                logger.WriteValue("canVoiceCall", browserInfo.CanInitiateVoiceCall);
                logger.WriteValue("canSendMail", browserInfo.CanSendMail);
                logger.WriteValue("cookiesSupport", browserInfo.Cookies);
                logger.WriteValue("isCrawler", browserInfo.Crawler);
                logger.WriteValue("EcmaScriptVersion", browserInfo.EcmaScriptVersion.ToString());
                logger.WriteValue("frameSupport", browserInfo.Frames);
                logger.WriteValue("gatewayVersion", browserInfo.GatewayVersion.ToString());
                logger.WriteValue("inputType", browserInfo.InputType);
                logger.WriteValue("isMobileDevice", browserInfo.IsMobileDevice);
                logger.WriteValue("javaAppletSupport", browserInfo.JavaApplets);
                if (browserInfo.IsMobileDevice) {
                    logger.WriteValue("mobileDeviceManufacturer", browserInfo.MobileDeviceManufacturer);
                    logger.WriteValue("mobileDeviceModel", browserInfo.MobileDeviceModel);
                }
                logger.WriteValue("platform", browserInfo.Platform);
                this.SerializeScreenInfo();
                logger.WriteValue("type", browserInfo.Type);
                logger.WriteValue("version", browserInfo.Version);
                logger.WriteValue("isWin32", browserInfo.Win32);
            }
            finally {
                logger.EndWriteObject("browser");
            }
        }

        void SerializeScreenInfo() {
            try {
                logger.BeginWriteObject("screen");
                logger.WriteValue("height", request.Browser.ScreenPixelsHeight);
                logger.WriteValue("width", request.Browser.ScreenPixelsWidth);
            }
            finally {
                logger.EndWriteObject("screen");
            }
        }

        void SerializeContentInfo() {
            try {
                logger.BeginWriteObject("content");
                logger.WriteValue("encoding", request.ContentEncoding.EncodingName);
                logger.WriteValue("length", request.ContentLength);
                logger.WriteValue("type", request.ContentType);
            }
            finally {
                logger.EndWriteObject("content");
            }
        }

        void SerializeFileInfo() {
            if (request.Files.Count != 0) {
                try {
                    logger.BeginWriteObject("files");
                    foreach (string key in request.Files.AllKeys) {
                        HttpPostedFile file = request.Files.Get(key);
                        logger.BeginWriteObject(key);
                        logger.WriteValue("name", file.FileName);
                        logger.WriteValue("length", file.ContentLength);
                        logger.WriteValue("type", file.ContentType);
                        logger.EndWriteObject(key);
                    }
                }
                finally {
                    logger.EndWriteObject("files");
                }
            }
        }
    }
    public class IgnorePropertiesInfo {
        public bool IgnoreAll { get; set; }
        public HashSet<string> IgnoreNamesExact { get; set; }
        public List<string> IgnoreNamesStartsWith { get; set; }
        public List<string> IgnoreNamesEndsWith { get; set; }
        public List<string> IgnoreNamesContains { get; set; }

        public bool ShouldIgnore(string name) {
            if (IgnoreAll)
                return true;

            if (String.IsNullOrEmpty(name))
                return false;

            return ShouldIgnoreExact(name) ||
                ShouldIgnoreStartsWith(name) ||
                ShouldIgnoreEndsWith(name) ||
                ShouldIgnoreContains(name);
        }
        bool ShouldIgnoreExact(string name) {
            return IgnoreNamesExact != null && IgnoreNamesExact.Contains(name);
        }
        bool ShouldIgnoreStartsWith(string name) {
            if (IgnoreNamesStartsWith == null)
                return false;

            int count = IgnoreNamesStartsWith.Count;
            for (int i = 0; i < count; i++) {
                if (name.StartsWith(IgnoreNamesStartsWith[i], StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }
        bool ShouldIgnoreEndsWith(string name) {
            if (IgnoreNamesEndsWith == null)
                return false;

            int count = IgnoreNamesEndsWith.Count;
            for (int i = 0; i < count; i++) {
                if (name.EndsWith(IgnoreNamesEndsWith[i], StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }
        bool ShouldIgnoreContains(string name) {
            if (IgnoreNamesContains == null)
                return false;

            int count = IgnoreNamesContains.Count;
            for (int i = 0; i < count; i++) {
                if (name.IndexOf(IgnoreNamesContains[i], StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }
        public static IgnorePropertiesInfo FromString(string value) {
            IgnorePropertiesInfo result = new IgnorePropertiesInfo();
            result.IgnoreNamesExact = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            result.IgnoreNamesStartsWith = new List<string>();
            result.IgnoreNamesEndsWith = new List<string>();
            result.IgnoreNamesContains = new List<string>();
            try {
                if (String.IsNullOrEmpty(value))
                    return result;

                string[] names = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (names == null || names.Length <= 0)
                    return result;

                int count = names.Length;
                for (int i = 0; i < count; i++) {
                    string item = names[i].Trim();
                    if (!String.IsNullOrEmpty(item)) {
                        if (item == "*") {
                            result.IgnoreAll = true;
                            break;
                        }
                        bool endsWith = item.StartsWith("*");
                        bool startsWith = item.EndsWith("*");
                        if (startsWith && endsWith) {
                            result.IgnoreNamesContains.Add(item.Trim('*'));
                        }
                        else if (startsWith && !endsWith) {
                            result.IgnoreNamesStartsWith.Add(item.Trim('*'));
                        }
                        else if (!startsWith && endsWith) {
                            result.IgnoreNamesEndsWith.Add(item.Trim('*'));
                        }
                        else {
                            if (!result.IgnoreNamesExact.Contains(item))
                                result.IgnoreNamesExact.Add(item);
                        }
                    }
                }

                return result;
            }
            catch {
                return result;
            }
        }
    }
}