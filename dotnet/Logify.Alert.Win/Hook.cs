using Accessibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace DevExpress.Logify.Core.Internal {
    public interface IMessageFilterEx : IMessageFilter {
        bool ProcessCbtProc(Win32.CbtEvent @event, IntPtr wParam, IntPtr lParam);
        //bool ProcessKeyboardProc(Keys key, Int32 lParam);
        //bool ProcessMouseProc(WindowsMessages message);
    }

    [SecuritySafeCritical]
    public class Win32HookManager : Win32HookManagerBase {
        static Win32HookManager instance;

        public static Win32HookManager Instance {
            get {
                if (instance != null)
                    return instance;
                lock (typeof(Win32HookManager)) {
                    if (instance == null)
                        instance = new Win32HookManager();
                }
                return instance;
            }
        }

        public Win32HookManager() {
            Application.ApplicationExit += OnApplicationExit;
            Application.ThreadExit += OnThreadExit;
        }

        ~Win32HookManager() {
            try {
                RemoveHooks();
                Application.ApplicationExit -= OnApplicationExit;
                Application.ThreadExit -= OnThreadExit;
            }
            catch {
            }
        }
        void OnThreadExit(object sender, EventArgs e) {
            try {
                RemoveHook();
            }
            catch {
            }
        }
        void OnApplicationExit(object sender, EventArgs e) {
            try {
                Application.ThreadExit -= OnThreadExit;
                Application.ApplicationExit -= OnApplicationExit;
                //if (GetCurrentThreadId() == startupThreadID)
                RemoveHooks();
            }
            catch {
            }
        }
    }

    [SecuritySafeCritical]
    public class Win32HookManagerBase {
        class HookInfo {
            internal IntPtr GetMsgHookHandle;
            internal IntPtr CbtHookHandle;
            //internal IntPtr MouseHandle;
            //internal IntPtr KeyboardHandle;
            internal IMessageFilterEx Filter;
            internal bool InHook;

            internal Win32.MOUSEHOOKSTRUCTEX hookStrEx = new Win32.MOUSEHOOKSTRUCTEX();
        }

        
        static readonly Dictionary<int, HookInfo> hooks = new Dictionary<int, HookInfo>();

        public bool AddHook(IMessageFilterEx hook) {
            try {
                int threadId = Win32.GetCurrentThreadId();
                lock (hooks) {
                    HookInfo existingHook;
                    if (hooks.TryGetValue(threadId, out existingHook))
                        return false;

                    HookInfo hookInfo = new HookInfo();
                    hookInfo.Filter = hook;
                    if (InstallHook(threadId, hookInfo)) {
                        hooks.Add(threadId, hookInfo);
                        return true;
                    }
                    return false;
                }
            }
            catch {
                return false;
            }
        }

        public bool RemoveHook() {
            try {
                int threadId = Win32.GetCurrentThreadId();
                lock (hooks) {
                    HookInfo existingHook;
                    if (hooks.TryGetValue(threadId, out existingHook)) {
                        UninstallHook(threadId, existingHook);
                        hooks.Remove(threadId);
                        return true;
                    }
                }
                return false;
            }
            catch {
                return false;
            }
        }
        protected void RemoveHooks() {
            lock (hooks) {
                foreach (int threadId in hooks.Keys)
                    UninstallHook(threadId, hooks[threadId]);
                hooks.Clear();
            }
        }

        static Win32.HookProc procGetMsg = Win32HookManager.GetMsgProc;
        static Win32.HookProc procCbt = Win32HookManager.CbtProc;

        bool InstallHook(int threadId, HookInfo hookInfo) {
            if (hookInfo != null) {
                bool isOk = false;
                hookInfo.CbtHookHandle = IntPtr.Zero;
                hookInfo.GetMsgHookHandle = IntPtr.Zero;
                if (hookInfo.GetMsgHookHandle == IntPtr.Zero) {
                    const int WH_GETMESSAGE = 3;
                    hookInfo.GetMsgHookHandle = Win32.SetWindowsHookEx(WH_GETMESSAGE, procGetMsg, IntPtr.Zero, threadId);
                    isOk = isOk || (hookInfo.GetMsgHookHandle != IntPtr.Zero);
                }
                if (hookInfo.CbtHookHandle == IntPtr.Zero) {
                    const int WH_CBT = 5;
                    hookInfo.CbtHookHandle = Win32.SetWindowsHookEx(WH_CBT, procCbt, IntPtr.Zero, threadId);
                    isOk = isOk || (hookInfo.CbtHookHandle != IntPtr.Zero);
                }
                //if (hookInfo.MouseHandle == IntPtr.Zero) {
                //    const int WH_MOUSE = 7;
                //    hookInfo.MouseHandle = Win32.SetWindowsHookEx(WH_MOUSE, MouseProc, 0, threadId);
                //    isOk = isOk || (hookInfo.MouseHandle != IntPtr.Zero);
                //}
                //if (hookInfo.KeyboardHandle == IntPtr.Zero) {
                //    const int WH_KEYBOARD = 2;
                //    hookInfo.KeyboardHandle = Win32.SetWindowsHookEx(WH_KEYBOARD, KeyboardProc, 0, threadId);
                //    isOk = isOk || (hookInfo.KeyboardHandle != IntPtr.Zero);
                //}
                //if (hookInfo.MsgFilterHandle == IntPtr.Zero) {
                //    const int WH_MSGFILTER = -1;
                //    hookInfo.MsgFilterHandle = Win32.SetWindowsHookEx(WH_MSGFILTER, MsgFilterProc, 0, threadId);
                //    isOk = isOk || (hookInfo.MsgFilterHandle != IntPtr.Zero);
                //}

                return isOk;
            }
            return false;
        }
        bool UninstallHook(int threadId, HookInfo hookInfo) {
            if (hookInfo != null) {
                bool isOk = false;
                //if (hookInfo.MsgFilterHandle != IntPtr.Zero)
                //    isOk = isOk || UnhookWindowsHookEx(hookInfo.GetMsgHandle);
                if (hookInfo.GetMsgHookHandle != IntPtr.Zero) {
                    IntPtr handle = hookInfo.GetMsgHookHandle;
                    hookInfo.GetMsgHookHandle = IntPtr.Zero;
                    isOk = Win32.UnhookWindowsHookEx(handle) || isOk;
                }
                if (hookInfo.CbtHookHandle != IntPtr.Zero) {
                    IntPtr handle = hookInfo.CbtHookHandle;
                    hookInfo.CbtHookHandle = IntPtr.Zero;
                    isOk = Win32.UnhookWindowsHookEx(handle) || isOk;
                }

                //if (hookInfo.MouseHandle != IntPtr.Zero)
                //    isOk = isOk || Win32.UnhookWindowsHookEx(hookInfo.MouseHandle);
                //if (hookInfo.KeyboardHandle != IntPtr.Zero)
                //    isOk = isOk || Win32.UnhookWindowsHookEx(hookInfo.KeyboardHandle);
                return isOk;
            }
            else
                return false;
        }

        /*
        static int MsgFilterProc(int nCode, IntPtr wParam, IntPtr lParam) {
            // nCode:
            //const int MSGF_DDEMGR = 0x8001; // The input event occurred while the DDEML was waiting for a synchronous transaction to finish. For more information about DDEML
            //const int MSGF_DIALOGBOX = 0; // The input event occurred in a message box or dialog box.
            //const int MSGF_MENU = 2; // The input event occurred in a menu.
            //const int MSGF_SCROLLBAR = 5; // The input event occurred in a scroll bar.

            try {
                HookInfo existingHook;
                int threadId = GetCurrentThreadId();
                if (hooks.TryGetValue(threadId, out existingHook)) {
                    if (!existingHook.InHook && lParam != IntPtr.Zero) {
                        try {
                            existingHook.InHook = true;

                            API_MSG msg = (API_MSG)Marshal.PtrToStructure(lParam, typeof(API_MSG));
                            Message message = msg.ToMessage();

                            existingHook.Filter.PreFilterMessage(ref message);
                        }
                        catch {
                        }
                        existingHook.InHook = false;
                    }
                    return CallNextHookEx(existingHook.MsgFilterHandle, nCode, wParam, lParam);
                }
            }
            catch {
            }
            return 0;

        }
        */
        static int GetMsgProc(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode < 0 || wParam == IntPtr.Zero || lParam == IntPtr.Zero)
                return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);

            try {
                HookInfo info;
                int threadId = Win32.GetCurrentThreadId();
                if (hooks.TryGetValue(threadId, out info)) {

                    // wParam:
                    //const int PM_NOREMOVE = 0x0000; // The message has not been removed from the queue. (An application called the PeekMessage function, specifying the PM_NOREMOVE flag.)
                    //const int PM_REMOVE = 0x0001; // The message has been removed from the queue. (An application called GetMessage, or it called the PeekMessage function, specifying the PM_REMOVE flag.)
                    if (!info.InHook) {
                        try {
                            info.InHook = true;

                            Win32.API_MSG msg = (Win32.API_MSG)Marshal.PtrToStructure(lParam, typeof(Win32.API_MSG));
                            Message message = msg.ToMessage();
                            info.Filter.PreFilterMessage(ref message);
                        }
                        catch {
                        }
                        info.InHook = false;
                    }
                    return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
                }
            }
            catch {
            }
            return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
        static int CbtProc(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode < 0 || lParam == IntPtr.Zero)
                return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            try {
                HookInfo info;
                int threadId = Win32.GetCurrentThreadId();
                if (hooks.TryGetValue(threadId, out info)) {
                    if (!info.InHook) {
                        try {
                            info.InHook = true;

                            info.Filter.ProcessCbtProc((Win32.CbtEvent)nCode, wParam, lParam);
                        }
                        catch {
                        }
                        info.InHook = false;
                    }
                    return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
                }
            }
            catch {
            }
            return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);

        }
        //static int MouseProc(int nCode, IntPtr wParam, IntPtr lParam) {
        //    try {
        //        HookInfo info;
        //        int threadId = GetCurrentThreadId();
        //        if (hooks.TryGetValue(threadId, out info)) {
        //            if (nCode != 0)
        //                return CallNextHookEx(info.MouseHandle, nCode, wParam, lParam); ;

        //            if (!info.InHook && lParam != IntPtr.Zero) {
        //                try {
        //                    info.InHook = true;

        //                    MOUSEHOOKSTRUCTEX hookStrEx = info.hookStrEx;
        //                    Marshal.PtrToStructure(lParam, hookStrEx);
        //                    MOUSEHOOKSTRUCT hookStr = hookStrEx.Mouse;

        //                    info.Filter.ProcessMouseProc((WindowsMessages)wParam);
        //                }
        //                catch {
        //                }
        //                info.InHook = false;
        //            }
        //            return Win32.CallNextHookEx(info.CbtHandle, nCode, wParam, lParam);
        //        }
        //    }
        //    catch {
        //    }
        //    return 0;

        //}
        //static int KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam) {
        //    try {
        //        HookInfo info;
        //        int threadId = GetCurrentThreadId();
        //        if (hooks.TryGetValue(threadId, out info)) {
        //            if (nCode != 0)
        //                return CallNextHookEx(info.KeyboardHandle, nCode, wParam, lParam); ;
        //            if (!info.InHook && lParam != IntPtr.Zero) {
        //                try {
        //                    info.InHook = true;

        //                    info.Filter.ProcessKeyboardProc((Keys)wParam, lParam.ToInt32());
        //                }
        //                catch {
        //                }
        //                info.InHook = false;
        //            }
        //            return Win32.CallNextHookEx(info.CbtHandle, nCode, wParam, lParam);
        //        }
        //    }
        //    catch {
        //    }
        //    return 0;

        //}
    }
}