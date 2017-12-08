using System;
using log4net;
using System.Diagnostics;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace DevExpress.Logify.Core.Internal {
    public class Logger {
        static ILog log;
        
        public static ILog Log {
            get {
                if (log == null) {
                    lock (typeof(Logger)) {
                        if (log != null)
                            return log;

                        log = LogManager.GetLogger(typeof(Logger));
                    }
                }
                return log;
            }
        }



        public static bool IsDebugEnabled { get { return Log.IsDebugEnabled; } }
        public static bool IsErrorEnabled { get { return Log.IsErrorEnabled; } }
        public static bool IsFatalEnabled { get { return Log.IsFatalEnabled; } }
        public static bool IsInfoEnabled { get { return Log.IsInfoEnabled; } }
        public static bool IsWarnEnabled { get { return Log.IsWarnEnabled; } }

        public static void Debug(object message) {
            if(IsDebugEnabled)
                Log.Debug(message);
        }
        public static void Debug(object message, Exception exception) {
            if(IsDebugEnabled)
                Log.Debug(message, exception);
        }
        public static void DebugFormat(string format, object arg0) {
            if(IsDebugEnabled)
                Log.DebugFormat(format, arg0);
        }
        public static void DebugFormat(string format, params object[] args) {
            if(IsDebugEnabled)
                Log.DebugFormat(format, args);
        }
        public static void DebugFormat(IFormatProvider provider, string format, params object[] args) {
            if(IsDebugEnabled)
                Log.DebugFormat(provider, format, args);
        }
        public static void DebugFormat(string format, object arg0, object arg1) {
            if(IsDebugEnabled)
                Log.DebugFormat(format, arg0, arg1);
        }
        public static void DebugFormat(string format, object arg0, object arg1, object arg2) {
            if(IsDebugEnabled)
                Log.DebugFormat(format, arg0, arg1, arg2);
        }

        public static void Error(object message) {
            if(IsErrorEnabled)
                Log.Error(message);
        }
        public static void Error(object message, Exception exception) {
            if(IsErrorEnabled)
                Log.Error(message, exception);
        }
        public static void ErrorFormat(string format, object arg0) {
            if(IsErrorEnabled)
                Log.ErrorFormat(format, arg0);
        }
        public static void ErrorFormat(string format, params object[] args) {
            if(IsErrorEnabled)
                Log.ErrorFormat(format, args);
        }
        public static void ErrorFormat(IFormatProvider provider, string format, params object[] args) {
            if(IsErrorEnabled)
                Log.ErrorFormat(provider, format, args);
        }
        public static void ErrorFormat(string format, object arg0, object arg1) {
            if(IsErrorEnabled)
                Log.ErrorFormat(format, arg0, arg1);
        }
        public static void ErrorFormat(string format, object arg0, object arg1, object arg2) {
            if(IsErrorEnabled)
                Log.ErrorFormat(format, arg0, arg1, arg2);
        }

        public static void Fatal(object message) {
            if(IsFatalEnabled)
                Log.Fatal(message);
        }
        public static void Fatal(object message, Exception exception) {
            if(IsFatalEnabled)
                Log.Fatal(message, exception);
        }
        public static void FatalFormat(string format, object arg0) {
            if(IsFatalEnabled)
                Log.FatalFormat(format, arg0);
        }
        public static void FatalFormat(string format, params object[] args) {
            if(IsFatalEnabled)
                Log.FatalFormat(format, args);
        }
        public static void FatalFormat(IFormatProvider provider, string format, params object[] args) {
            if(IsFatalEnabled)
                Log.FatalFormat(provider, format, args);
        }
        public static void FatalFormat(string format, object arg0, object arg1) {
            if(IsFatalEnabled)
                Log.FatalFormat(format, arg0, arg1);
        }
        public static void FatalFormat(string format, object arg0, object arg1, object arg2) {
            if(IsFatalEnabled)
                Log.FatalFormat(format, arg0, arg1, arg2);
        }

        public static void Info(object message) {
            if(IsInfoEnabled)
                Log.Info(message);
        }
        public static void Info(object message, Exception exception) {
            if(IsInfoEnabled)
                Log.Info(message, exception);
        }
        public static void InfoFormat(string format, object arg0) {
            if(IsInfoEnabled)
                Log.InfoFormat(format, arg0);
        }
        public static void InfoFormat(string format, params object[] args) {
            if(IsInfoEnabled)
                Log.InfoFormat(format, args);
        }
        public static void InfoFormat(IFormatProvider provider, string format, params object[] args) {
            if(IsInfoEnabled)
                Log.InfoFormat(provider, format, args);
        }
        public static void InfoFormat(string format, object arg0, object arg1) {
            if(IsInfoEnabled)
                Log.InfoFormat(format, arg0, arg1);
        }
        public static void InfoFormat(string format, object arg0, object arg1, object arg2) {
            if(IsInfoEnabled)
                Log.InfoFormat(format, arg0, arg1, arg2);
        }

        public static void Warn(object message) {
            if(IsWarnEnabled)
                Log.Warn(message);
        }
        public static void Warn(object message, Exception exception) {
            if(IsWarnEnabled)
                Log.Warn(message, exception);
        }
        public static void WarnFormat(string format, object arg0) {
            if(IsWarnEnabled)
                Log.WarnFormat(format, arg0);
        }
        public static void WarnFormat(string format, params object[] args) {
            if(IsWarnEnabled)
                Log.WarnFormat(format, args);
        }
        public static void WarnFormat(IFormatProvider provider, string format, params object[] args) {
            if(IsWarnEnabled)
                Log.WarnFormat(provider, format, args);
        }
        public static void WarnFormat(string format, object arg0, object arg1) {
            if(IsWarnEnabled)
                Log.WarnFormat(format, arg0, arg1);
        }
        public static void WarnFormat(string format, object arg0, object arg1, object arg2) {
            if(IsWarnEnabled)
                Log.WarnFormat(format, arg0, arg1, arg2);
        }
    }
}