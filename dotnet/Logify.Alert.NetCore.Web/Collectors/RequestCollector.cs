using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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
                //logger.WriteValue("acceptTypes", request.AcceptTypes);
                this.SerializeContentInfo();
                this.SerializeFileInfo();
                Utils.SerializeCookieInfo(request.Cookies, this.ignoreCookies, logger);
                Dictionary<string, string> ignoredFormFields = null;
                try {
                    if (!ignoreRequestBody)
                        ignoredFormFields = Utils.SerializeInfo(request.Form, "form", this.ignoreFormFields, null, logger);
                }
                catch {
                }
                Utils.SerializeInfo(request.Headers, "headers", this.ignoreHeaders, h => h == "Cookie", logger);
                //Utils.SerializeInfo(request.ServerVariables, "serverVariables", this.ignoreServerVariables, v => v.StartsWith("ALL_") || v.StartsWith("HTTP_"), logger);
                Utils.SerializeInfo(request.Query, "queryString", null, null, logger);
                //logger.WriteValue("applicationPath", request.ApplicationPath);
                logger.WriteValue("httpMethod", request.Method);
                if (!ignoreRequestBody) {
                    try {
                        string content = RequestBodyFilter.GetRequestContent(request.Method, request.ContentType, request.Body, ignoredFormFields);
                        if (content != null)
                            logger.WriteValue("httpRequestBody", content);
                    }
                    catch {
                    }
                }
                //logger.WriteValue("isUserAuthenticated", request.IsAuthenticated);
                //logger.WriteValue("isLocalRequest", request.IsLocal);
                logger.WriteValue("isSecureConnection", request.IsHttps);
                //logger.WriteValue("requestType", request.RequestType); // request.Protocol ?
                //logger.WriteValue("totalBytes", request.TotalBytes);
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
                //logger.WriteValue("encoding", request.ContentEncoding.EncodingName);
                logger.WriteValue("length", request.ContentLength.HasValue ? (int)request.ContentLength : -1);
                logger.WriteValue("type", request.ContentType);
            }
            finally {
                logger.EndWriteObject("content");
            }
        }

        void SerializeFileInfo() {
            try {
                if (request.Form == null)
                    return;

                IFormFileCollection files = request.Form.Files;
                if (files == null || files.Count <= 0)
                    return;

                logger.BeginWriteObject("files");
                try {
                    foreach (IFormFile file in files) {
                        logger.BeginWriteObject(file.Name);
                        logger.WriteValue("name", file.FileName);
                        logger.WriteValue("length", (int)file.Length);
                        logger.WriteValue("type", file.ContentType);
                        logger.EndWriteObject(file.Name);
                    }
                }
                finally {
                    logger.EndWriteObject("files");
                }
            }
            catch {
            }
        }
    }
}