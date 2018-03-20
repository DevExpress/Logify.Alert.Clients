using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

using DevExpress.Logify.Core;

namespace DevExpress.Logify.Core.Internal {
    class ResponseCollector: IInfoCollector {
        readonly HttpResponse response;
        readonly string name;
        readonly IgnorePropertiesInfo ignoreHeaders;
        readonly IgnorePropertiesInfo ignoreCookies;
        ILogger logger;

        public ResponseCollector(HttpResponse response, IgnorePropertiesInfoConfig ignoreConfig) : this(response, "response", ignoreConfig) { }

        public ResponseCollector(HttpResponse response, string name, IgnorePropertiesInfoConfig ignoreConfig) {
            this.response = response;
            this.name = name;
            this.ignoreHeaders = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreHeaders);
            this.ignoreCookies = IgnorePropertiesInfo.FromString(ignoreConfig.IgnoreCookies);
        }

        public void Process(Exception e, ILogger logger) {
            if (this.response == null) { logger.WriteValue("isResponse", "null"); return;};
            this.logger = logger;
            try {
                logger.BeginWriteObject("response");
                logger.WriteValue("charset", response.Charset);
                logger.WriteValue("contentEncoding", response.ContentEncoding.EncodingName);
                logger.WriteValue("contentType", response.ContentType);
                logger.WriteValue("expires", response.Expires);
                logger.WriteValue("expiresAbsolute", response.ExpiresAbsolute.ToString());
                logger.WriteValue("headerEncoding", response.HeaderEncoding.EncodingName);
                try {
                    if (response.OutputStream != null && response.OutputStream.CanRead)
                        logger.WriteValue("httpResponseBody", (new System.IO.StreamReader(response.OutputStream)).ReadToEnd());
                } catch { }
#if NET_4_5_2
                            logger.WriteValue("isHeadersWritten", response.HeadersWritten);
                            logger.WriteValue("suppressDefaultCacheControlHeader", response.SuppressDefaultCacheControlHeader);
#endif
                logger.WriteValue("isClientConnected", response.IsClientConnected);
                logger.WriteValue("SuppressContent", response.SuppressContent);
                SerializeStatusInfo(e);
                Utils.SerializeCookieInfo(response.Cookies, this.ignoreCookies, logger);
                Utils.SerializeInfo(response.Headers, "headers", this.ignoreHeaders, null, logger);
            }
            finally {
                logger.EndWriteObject("response");
            }
        }

        void SerializeStatusInfo(Exception e) {
            try {
                logger.BeginWriteObject("status");
                logger.WriteValue("full", response.Status);
                logger.WriteValue("code", response.StatusCode);
                try {
                    HttpException ex = e as HttpException;
                    if (ex != null)
                        logger.WriteValue("httpExceptionCode", ex.GetHttpCode());
                }
                catch {
                }
                logger.WriteValue("description", response.StatusDescription);
                try {
                    logger.WriteValue("subStatusCode", response.SubStatusCode);
                } catch (PlatformNotSupportedException) { }
            }
            finally {
                logger.EndWriteObject("status");
            }
        }
    }
}
