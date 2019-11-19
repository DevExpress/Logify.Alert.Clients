using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DevExpress.Logify.Core;
using System.ComponentModel;

namespace DevExpress.Logify {
    public class LogifyAlertLoggerProvider : ILoggerProvider {
        readonly IConfigurationSection _config;
        readonly LogifyClientBase _logifyClient;
        internal LogifyAlertLoggerProvider(IConfigurationSection config, LogifyClientBase logifyClient) {
            this._config = config;
            this._logifyClient = logifyClient;
        }
        [CLSCompliant(false)]
        public ILogger CreateLogger(string categoryName) {
            return new LogifyAlertLogger(categoryName, this._config, this._logifyClient);
        }
        public void Dispose() { }
    }
    [CLSCompliant(false)]
    public class LogifyAlertLogger : ILogger {
        class EmptyScope : IDisposable {
            public void Dispose() { }
        }
        readonly string _categoryName;
        internal LogifyAlertLogger(string categoryName, IConfigurationSection config, LogifyClientBase logifyClient) {
            this._categoryName = categoryName;
            if (config != null && logifyClient != null) {
                logifyClient.Configure(Core.Internal.ClientConfigurationLoader.LoadConfiguration(config));
            }
        }
        public IDisposable BeginScope<TState>(TState state) {
            return new EmptyScope();
        }
        public bool IsEnabled(LogLevel logLevel) {
            return IsEnabledInternal();
        }
        bool IsEnabledInternal() {
            return GetLogifyClient() != null;
        }
        LogifyClientBase GetLogifyClient() {
            return LogifyClientBase.Instance;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            //string message = formatter(state, exception);
            if (exception != null && IsEnabledInternal())
                GetLogifyClient().Send(exception);
        }
    }
    public static class ILoggerFactoryExtensions {
        [CLSCompliant(false)]
        public static ILoggerFactory UseLogifyAlert(this ILoggerFactory factory) {
            return AddLogifyAlert(factory, null, null);
        }
        [CLSCompliant(false)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), /*DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),*/ Description("For experimental use only")]
        public static ILoggerFactory UseLogifyAlert(this ILoggerFactory factory, IConfigurationSection config, LogifyClientBase logifyClient) {
            return AddLogifyAlert(factory, config, logifyClient);
        }
        static ILoggerFactory AddLogifyAlert(ILoggerFactory factory, IConfigurationSection config, LogifyClientBase logifyClient) {
            factory.AddProvider(new LogifyAlertLoggerProvider(config, logifyClient));
            return factory;
        }
    }
}