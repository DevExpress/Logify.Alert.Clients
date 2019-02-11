using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.Logify.Core;
using DevExpress.Logify.Core.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DevExpress.Logify.Web {
    internal class LogifyAlertMiddleware {
        RequestDelegate next;

        public LogifyAlertMiddleware(RequestDelegate next, Microsoft.Extensions.Configuration.IConfigurationSection config) {
            this.next = next;
            if (config != null)
                LogifyAlert.Instance.Configure(config);
        }

        public async Task Invoke(HttpContext context) {
            //context.Response.StatusCode
            try {
                if(LogifyAlert.Instance.CollectBreadcrumbs)
                    NetCoreWebBreadcrumbsRecorder.Instance.AddBreadcrumb(context);
                await next(context);
            }
            catch (Exception e) {
                ReportException(e, context);
                throw;
            }
            if (context.Response != null && context.Response.StatusCode == 404) {
                try {
                    LogifyHttpException e = new LogifyHttpException(404, Create404ExceptionMessage(context));
                    throw e;
                }
                catch (Exception ex) {
                    ReportException(ex, context);
                }
            }
        }
        [IgnoreCallTracking]
        string Create404ExceptionMessage(HttpContext context) {
            try {
                if (context.Request == null)
                    return String.Empty;
                return String.Format("The controller for path '{0}' was not found or does not implement IController.", Utils.GetRequestFullPath(context.Request));
            }
            catch {
                return String.Empty;
            }
        }
        [IgnoreCallTracking]
        void ReportException(Exception ex, HttpContext context) {
            LogifyAlert.Instance.Send(ex, context);
        }
    }

    public class LogifyHttpException : Exception {
        readonly int httpStatusCode;

        [IgnoreCallTracking]
        public LogifyHttpException(int httpStatusCode) {
            this.httpStatusCode = httpStatusCode;
        }

        [IgnoreCallTracking]
        public LogifyHttpException(int httpStatusCode, string message) : base(message) {
            this.httpStatusCode = httpStatusCode;
        }

        [IgnoreCallTracking]
        public LogifyHttpException(int httpStatusCode, string message, Exception inner) : base(message, inner) {
            this.httpStatusCode = httpStatusCode;
        }

        public int StatusCode { get { return this.httpStatusCode; } }
    }
    public static class LogifyIAlertApplicationBuilderExtensions {
        [CLSCompliant(false)]
        public static IApplicationBuilder UseLogifyAlert(this IApplicationBuilder builder, Microsoft.Extensions.Configuration.IConfigurationSection config) {
            return builder.UseMiddleware<LogifyAlertMiddleware>(config);
        }
    }
}