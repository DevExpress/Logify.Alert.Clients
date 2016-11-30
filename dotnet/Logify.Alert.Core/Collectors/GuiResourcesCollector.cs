using System;
using System.Drawing;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace DevExpress.Logify.Core {
    public class Win32GuiResourcesCollector : IInfoCollector {
        [HandleProcessCorruptedStateExceptions]
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("win32GuiResources");
            try {
                const int guiResourceGdiObjects = 0;
                const int guiResourceUserObjects = 1;
                IntPtr handle = System.Diagnostics.Process.GetCurrentProcess().Handle;
                logger.WriteValue("gdiHandleCount", GetGuiResources(handle, guiResourceGdiObjects).ToString());
                logger.WriteValue("userHandleCount", GetGuiResources(handle, guiResourceUserObjects).ToString());
            }
            finally {
                logger.EndWriteObject("win32GuiResources");
            }
        }

        [DllImport("User32")]
        extern static int GetGuiResources(IntPtr hProcess, int uiFlags);
    }
}
