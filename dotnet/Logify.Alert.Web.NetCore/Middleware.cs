using System;
using System.Threading.Tasks;
using DevExpress.Logify.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace DevExpress.Logify.Web {
    internal class LogifyAlertMiddleware {
        RequestDelegate next;

        public LogifyAlertMiddleware(RequestDelegate next, Microsoft.Extensions.Configuration.IConfigurationSection config) {
            this.next = next;
            if (config != null)
                LogifyAlert.Instance.Configure(config);
        }

        public async Task Invoke(HttpContext context) {
            try {
                await next(context);
            }
            catch (Exception e) {
                LogifyHttpContext.Current = context;
                LogifyAlert.Instance.Send(e);
                LogifyHttpContext.Current = null;
                throw e;
            }
        }
    }

    public static class LogifyIAlertApplicationBuilderExtensions {
        public static IApplicationBuilder UseLogifyAlert(this IApplicationBuilder builder, Microsoft.Extensions.Configuration.IConfigurationSection config) {
            return builder.UseMiddleware<LogifyAlertMiddleware>(config);
        }
    }
}