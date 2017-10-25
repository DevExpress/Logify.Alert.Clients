using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace DevExpress.Logify.Core.Internal {
    public class DevExpressVersionsInStackCollector : IInfoCollector {
        public void Process(Exception ex, ILogger logger) {
            List<string> versions = new List<string>();
            for (; ; ) {
                StackTrace trace = new StackTrace(ex, false);
                TryDetectDxVersion(trace, versions);
                ex = ex.InnerException;
                if (ex == null)
                    break;
            }

            if (versions.Count <= 0)
                return;

            logger.WriteValue("dxVersionsInStack", versions.ToArray());
        }

        void TryDetectDxVersion(StackTrace trace, List<string> versions) {
#if NETSTANDARD
            StackFrame[] frames = trace.GetFrames();
            if (frames == null || frames.Length <= 0)
                return;
            int count = frames.Length;
            for (int i = 0; i < count; i++)
                TryDetectDxVersion(frames[i], versions);
#else
            int count = trace.FrameCount;
            for (int i = 0; i < count; i++)
                TryDetectDxVersion(trace.GetFrame(i), versions);
#endif
        }
        void TryDetectDxVersion(StackFrame frame, List<string> versions) {
            MethodBase method = frame.GetMethod();
            if (method == null)
                return;

#if NETSTANDARD
            Assembly asm = method.DeclaringType.GetTypeInfo().Assembly;
#else
            Assembly asm = method.DeclaringType.Assembly;
#endif
            if (!IsSignedDevExpressAssembly(asm))
                return;

            string version = GetVersionString(asm);
            if (!String.IsNullOrEmpty(version) && !versions.Contains(version))
                versions.Add(version);
        }
        static bool IsSignedDevExpressAssembly(Assembly asm) {
            string assemblyName = asm.FullName;
            if (String.IsNullOrEmpty(assemblyName))
                return false;
            if (assemblyName.Contains("PublicKeyToken=b88d1754d700e49a") ||
                assemblyName.Contains("PublicKeyToken=35c9f04b7764aa3d") ||
                assemblyName.Contains("PublicKeyToken=2a123bb213b3ffe4") ||
                assemblyName.Contains("PublicKeyToken=3f8e338797a7a380"))
                return true;
            return false;
        }
        string GetVersionString(Assembly info) {
            string name = info.FullName;
            if (String.IsNullOrEmpty(name))
                return String.Empty;

            int index = name.IndexOf("Version=");
            if (index < 0)
                return String.Empty;

            index += "Version=".Length;
            int commaIndex = name.IndexOf(',', index);
            if (commaIndex < 0)
                return name.Substring(index);
            else if (commaIndex > index)
                return name.Substring(index, commaIndex - index);
            else
                return String.Empty;
        }

    }
}