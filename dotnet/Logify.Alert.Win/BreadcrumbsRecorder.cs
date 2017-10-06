using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace DevExpress.Logify.Core.Internal {
    [SecuritySafeCritical]
    public class WinFormsBreadcrumsRecorder : IMessageFilterEx {
        public bool VerboseKeyboardEvents { get; set; }
        public bool IncludePasswords { get; set; }
        public bool PreFilterMessage(ref Message m) {
            try {
                Win32.WindowsMessages message = (Win32.WindowsMessages)m.Msg;
                if (message == Win32.WindowsMessages.WM_LBUTTONUP)
                    return ProcessMouseMessage(ref m, MouseButtons.Left, true);
                else if (message == Win32.WindowsMessages.WM_RBUTTONUP)
                    return ProcessMouseMessage(ref m, MouseButtons.Right, true);
                else if (message == Win32.WindowsMessages.WM_MBUTTONUP)
                    return ProcessMouseMessage(ref m, MouseButtons.Middle, true);
                else if (message == Win32.WindowsMessages.WM_LBUTTONDOWN)
                    return ProcessMouseMessage(ref m, MouseButtons.Left, false);
                else if (message == Win32.WindowsMessages.WM_RBUTTONDOWN)
                    return ProcessMouseMessage(ref m, MouseButtons.Right, false);
                else if (message == Win32.WindowsMessages.WM_MBUTTONDOWN)
                    return ProcessMouseMessage(ref m, MouseButtons.Middle, false);
                else if (message == Win32.WindowsMessages.WM_LBUTTONDBLCLK)
                    return ProcessDblClickMessage(ref m, MouseButtons.Left);
                else if (message == Win32.WindowsMessages.WM_RBUTTONDBLCLK)
                    return ProcessDblClickMessage(ref m, MouseButtons.Right);
                else if (message == Win32.WindowsMessages.WM_MBUTTONDBLCLK)
                    return ProcessDblClickMessage(ref m, MouseButtons.Middle);
                else if (message == Win32.WindowsMessages.WM_MOUSEWHEEL)
                    return ProcessMouseWheelMessage(ref m);
                else if (message == Win32.WindowsMessages.WM_KEYDOWN)
                    return ProcessKeyMessage(ref m, false);
                else if (message == Win32.WindowsMessages.WM_KEYUP)
                    return ProcessKeyMessage(ref m, true);
                else if (message == Win32.WindowsMessages.WM_CHAR)
                    return ProcessKeyCharMessage(ref m);

                return false;
            }
            catch {
                return false;
            }
        }
        public bool ProcessCbtProc(Win32.CbtEvent @event, IntPtr wParam, IntPtr lParam) {
            switch (@event) {
                case Win32.CbtEvent.HCBT_ACTIVATE:
                    return ProcessActivateMessage(wParam, lParam);
                case Win32.CbtEvent.HCBT_SETFOCUS:
                    return ProcessSetFocusMessage(wParam, lParam);
            }
            //Debug.WriteLine(@event.ToString());
            return false;
        }

        void PopulateCommonBreadcrumbInfo(Breadcrumb item) {
            item.DateTime = DateTime.Now;
            item.Level = BreadcrumbLevel.Info;
            item.ThreadId = Win32.GetCurrentThreadId().ToString();
            item.Category = "input";
        }
        bool ProcessMouseMessage(ref Message m, MouseButtons button, bool isUp) {
            string windowText = GetWindowTextOrAccessibleName(m.HWnd, Win32.PointFromLParam(m.LParam));

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["mouseButton"] = button.ToString();
            data["action"] = isUp ? "up" : "down";
            data["windowText"] = windowText;

            Breadcrumb item = new Breadcrumb();
            item.Event = isUp ? BreadcrumbEvent.MouseUp : BreadcrumbEvent.MouseDown;
            item.CustomData = data;
            item.Message = data["mouseButton"] + " mouse button " + data["action"] + " over [" + data["windowText"] + "]";

            AddBreadcrumb(item);
            return false;
        }
        bool ProcessDblClickMessage(ref Message m, MouseButtons button) {
            string windowText = GetWindowTextOrAccessibleName(m.HWnd, Win32.PointFromLParam(m.LParam));

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["mouseButton"] = button.ToString();
            data["action"] = "doubleClick";
            data["windowText"] = windowText;

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.MouseDoubleClick;
            item.CustomData = data;
            item.Message = data["mouseButton"] + " mouse button double click over[" + data["windowText"] + "]";

            AddBreadcrumb(item);
            return false;
        }
        bool ProcessMouseWheelMessage(ref Message m) {
            Dictionary<string, string> data = new Dictionary<string, string>();

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.MouseWheel;
            item.CustomData = data;
            item.Message = "Mouse wheel";

            AddBreadcrumb(item);
            return false;
        }
        bool ProcessKeyMessage(ref Message m, bool isUp) {
            string key = ((Keys)m.WParam).ToString();
            if (ShouldMaskMessage(ref m))
                key = "*";

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["key"] = key;
            data["scanCode"] = GetScanCode(m.LParam).ToString();
            data["action"] = isUp ? "up" : "down";

            Breadcrumb item = new Breadcrumb();
            item.Event = isUp ? BreadcrumbEvent.KeyUp : BreadcrumbEvent.KeyDown;
            item.CustomData = data;
            item.Message = "[" + data["key"] + "] " + data["action"];

            AddBreadcrumb(item);
            //if (isUp)
            //    TryOptimizeLastKeyboardBreadcrumbs();
            return false;
        }
        bool ProcessKeyCharMessage(ref Message m) {
            char @char = (char)m.WParam;
            if (ShouldMaskMessage(ref m))
                @char = '*';

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["char"] = new string(@char, 1);
            data["scanCode"] = GetScanCode(m.LParam).ToString();
            data["action"] = "press";

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.KeyPress;
            item.CustomData = data;
            item.Message = "Type '" + data["char"] + "'";

            AddBreadcrumb(item);
            return false;
        }
        int GetScanCode(IntPtr param) {
            int value = param.ToInt32();
            return (value >> 16) & 0xFF;
        }
        bool ShouldMaskMessage(ref Message m) {
            if (IncludePasswords)
                return false;
            return Win32.IsPasswordBox(m.HWnd);
        }
        bool ProcessActivateMessage(IntPtr wParam, IntPtr lParam) {
            string windowText = GetWindowText(wParam);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["windowCaption"] = windowText;

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.WindowActivate;
            item.CustomData = data;
            item.Message = "Window activated [" + data["windowCaption"] + "]";

            AddBreadcrumb(item);
            return false;
        }
        bool ProcessSetFocusMessage(IntPtr wParam, IntPtr lParam) {
            string windowText = GetWindowText(wParam);

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["windowCaption"] = windowText;

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.FocusChange;
            item.CustomData = data;
            item.Message = "Keyboard focus changed to [" + data["windowCaption"] + "]";

            AddBreadcrumb(item);
            return false;
        }
        void AddBreadcrumb(Breadcrumb item) {
            if (LogifyClientBase.Instance != null) {
                PopulateCommonBreadcrumbInfo(item);
                LogifyClientBase.Instance.Breadcrumbs.AddSimple(item);
            }
        }

        string GetWindowText(IntPtr hWnd) {
            string windowText = Win32.GetWindowText(hWnd);
            if (String.IsNullOrEmpty(windowText))
                windowText = Win32.GetAccessibleName(hWnd);
            return windowText;
        }
        string GetWindowTextOrAccessibleName(IntPtr hWnd, Point point) {
            string windowText = Win32.GetWindowText(hWnd);
            if (String.IsNullOrEmpty(windowText))
                windowText = Win32.GetAccessibleName(hWnd, point);
            return windowText;
        }
        /*
        void TryOptimizeLastKeyboardBreadcrumbs() {
            if (!VerboseKeyboardEvents)
                return;
            if (LogifyClientBase.Instance == null)
                return;
            BreadcrumbCollection breadcrumbs = LogifyClientBase.Instance.Breadcrumbs;
            if (breadcrumbs == null || breadcrumbs.Count < 3) // at lest 3 events need: KeyDown, KeyChar, KeyUp
                return;

            int upIndex = breadcrumbs.Count - 1;
            Breadcrumb lastUp = breadcrumbs[upIndex];
            if (lastUp.Event != BreadcrumbEvent.KeyUp)
                return;

            StringBuilder textBuffer = new StringBuilder();
            int downIndex = -1;
            for (int i = upIndex - 1; i >= 0; i--) {
                char ch = GetBreadcrumbChar(breadcrumbs[i]);
                if (ch == nullChar) {
                    downIndex = i;
                    break;
                }
                textBuffer.Insert(0, ch);
            }
            if (downIndex < 0)
                return;

            Breadcrumb firstDown = breadcrumbs[downIndex];
            if (firstDown.Event != BreadcrumbEvent.KeyDown)
                return;


            string text = textBuffer.ToString();
            if (text.Length <= 0)
                return;

            breadcrumbs.RemoveRange(downIndex, upIndex - downIndex + 1);

            if (downIndex > 0) {
                if (TryAppendLastTypeText(breadcrumbs[downIndex - 1], text)) {
                    return;
                }
            }

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["text"] = text;
            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.TypeText;
            item.CustomData = data;
            item.Message = "Type text '" + text + "'";
            AddBreadcrumb(item);
        }
        const char nullChar = '\u0000';
        char GetBreadcrumbChar(Breadcrumb breadcrumb) {
            if (breadcrumb.Event != BreadcrumbEvent.KeyPress)
                return nullChar;
            IDictionary<string, string> data = breadcrumb.CustomData;
            if (data == null || data.Count <= 0)
                return nullChar;

            string text;
            if (!data.TryGetValue("char", out text))
                return nullChar;
            if (text.Length != 1)
                return nullChar;
            return text[0];
        }
        bool TryAppendLastTypeText(Breadcrumb item, string text) {
            if (item.Event != BreadcrumbEvent.TypeText)
                return false;

            var data = item.CustomData;
            if (data == null || data.Count <= 0)
                return false;
            string existingText;
            if (!data.TryGetValue("text", out existingText))
                return false;

            string newText = existingText + text;
            data["text"] = newText;
            item.Message = "Type text '" + newText + "'";
            return true;
        }
        */
    }
}