using System;
using System.Collections.Generic;
using System.Web;

namespace DevExpress.Logify.Core.Internal {
    class RequestCollector : IInfoCollector {
        readonly HttpRequest request;
        readonly string name;
        readonly IgnorePropertiesInfo ignoreFormFields;
        readonly IgnorePropertiesInfo ignoreHeaders;
        readonly IgnorePropertiesInfo ignoreCookies;
        readonly IgnorePropertiesInfo ignoreServerVariables;
        readonly bool ignoreRequestBody;
        ILogger logger;

        public RequestCollector(HttpRequest request, IgnorePropertiesInfoConfig ignoreConfig) : this(request, "request", ignoreConfig) { }

        public RequestCollector(HttpRequest request, string name, IgnorePropertiesInfoConfig ignoreConfig) {
            this.request = request;
            this.name = name;
            this.ignoreFormFields = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreFormFields);
            this.ignoreHeaders = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreHeaders);
            this.ignoreCookies = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreCookies);
            this.ignoreServerVariables = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreServerVariables);
            this.ignoreRequestBody = ignoreConfig.IgnoreRequestBody;
        }

        public virtual void Process(Exception e, ILogger logger) {
            this.logger = logger;
            try {
                logger.BeginWriteObject(name);
                logger.WriteValue("acceptTypes", request.AcceptTypes);
                this.SerializeContentInfo();
                this.SerializeFileInfo();
                Utils.SerializeCookieInfo(request.Cookies, this.ignoreCookies, logger);
                Dictionary<string, string> ignoredFormFields = null;
                if (!ignoreRequestBody)
                    ignoredFormFields = Utils.SerializeInfo(request.Form, "form", this.ignoreFormFields, null, logger);
                Utils.SerializeInfo(request.Headers, "headers", this.ignoreHeaders, h => h == "Cookie", logger);
                Utils.SerializeInfo(request.ServerVariables, "serverVariables", this.ignoreServerVariables, v => v.StartsWith("ALL_") || v.StartsWith("HTTP_"), logger);
                Utils.SerializeInfo(request.QueryString, "queryString", null, null, logger);
                logger.WriteValue("applicationPath", request.ApplicationPath);
                logger.WriteValue("httpMethod", request.HttpMethod);
                if (!ignoreRequestBody) {
                    try {
                        string content = RequestBodyFilter.GetRequestContent(request.RequestType, request.ContentType, request.InputStream, ignoredFormFields);
                        if (content != null)
                            logger.WriteValue("httpRequestBody", content);
                    }
                    catch { }
                }
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

        /*
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
        */

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
}