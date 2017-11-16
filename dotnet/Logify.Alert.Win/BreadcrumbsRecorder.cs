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
    [SecuritySafeCritical]
    public class WinFormsBreadcrumsRecorder : BreadcrumbsRecorderBase, IMessageFilterEx {
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

        
        bool ProcessMouseMessage(ref Message m, MouseButtons button, bool isUp) {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["mouseButton"] = button.ToString();
            data["action"] = isUp ? "up" : "down";
            AppendMouseCoords(ref m, data);
            AppendTargetInfo(m.HWnd, Win32.PointFromLParam(m.LParam), data);

            Breadcrumb item = new Breadcrumb();
            item.Event = isUp ? BreadcrumbEvent.MouseUp : BreadcrumbEvent.MouseDown;
            item.CustomData = data;

            AddBreadcrumb(item);
            return false;
        }
        bool ProcessDblClickMessage(ref Message m, MouseButtons button) {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["mouseButton"] = button.ToString();
            data["action"] = "doubleClick";
            AppendMouseCoords(ref m, data);
            AppendTargetInfo(m.HWnd, Win32.PointFromLParam(m.LParam), data);

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.MouseDoubleClick;
            item.CustomData = data;

            AddBreadcrumb(item);
            return false;
        }
        bool ProcessMouseWheelMessage(ref Message m) {
            /*
            Dictionary<string, string> data = new Dictionary<string, string>();

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.MouseWheel;
            item.CustomData = data;
            //item.Message = "Mouse wheel";

            AddBreadcrumb(item);
            */
            return false;
        }
        bool ProcessKeyMessage(ref Message m, bool isUp) {
            string key = ((Keys)m.WParam).ToString();
            bool maskKey = ShouldMaskMessage(ref m) && !IsNonSymbolKey((Keys)m.WParam);
            if (maskKey)
                key = Keys.Multiply.ToString();

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["key"] = key;
            data["scanCode"] = maskKey ? "0" : GetScanCode(m.LParam).ToString();
            data["action"] = isUp ? "up" : "down";

            Breadcrumb item = new Breadcrumb();
            item.Event = isUp ? BreadcrumbEvent.KeyUp : BreadcrumbEvent.KeyDown;
            item.CustomData = data;

            AddBreadcrumb(item);
            return false;
        }

        bool IsNonSymbolKey(Keys key) {
            return key == Keys.Tab ||
                key == Keys.Left ||
                key == Keys.Right ||
                key == Keys.Up ||
                key == Keys.Down ||
                key == Keys.PageUp ||
                key == Keys.PageDown ||
                key == Keys.Home ||
                key == Keys.End ||
                key == Keys.Enter ||
                key == Keys.Return ||
                key == Keys.Control ||
                key == Keys.ControlKey ||
                key == Keys.LControlKey ||
                key == Keys.RControlKey ||
                key == Keys.Shift ||
                key == Keys.ShiftKey ||
                key == Keys.LShiftKey ||
                key == Keys.RShiftKey;
        }

        bool ProcessKeyCharMessage(ref Message m) {
            char @char = (char)m.WParam;
            bool maskKey = ShouldMaskMessage(ref m);
            if (maskKey)
                @char = '*';

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["char"] = new string(@char, 1);
            data["scanCode"] = maskKey ? "0" : GetScanCode(m.LParam).ToString();
            data["action"] = "press";

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.KeyPress;
            item.CustomData = data;

            AddBreadcrumb(item);
            return false;
        }
        Point GetScreenMousePosition(IntPtr hWnd, IntPtr param) {
            int x = GetMouseX(param);
            int y = GetMouseY(param);
            return Win32.ClientToScreen(hWnd, new Point(x, y));
        }
        int GetMouseX(IntPtr param) {
            int value = param.ToInt32();
            return value & 0xFFFF;
        }
        int GetMouseY(IntPtr param) {
            int value = param.ToInt32();
            return (value >> 16) & 0xFFFF;
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
            Dictionary<string, string> data = new Dictionary<string, string>();
            AppendTargetInfo(wParam, data);

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.WindowActivate;
            item.CustomData = data;

            AddBreadcrumb(item);
            return false;
        }
        bool ProcessSetFocusMessage(IntPtr wParam, IntPtr lParam) {
            Dictionary<string, string> data = new Dictionary<string, string>();
            AppendTargetInfo(wParam, data);

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.FocusChange;
            item.CustomData = data;

            AddBreadcrumb(item);
            return false;
        }
        void AppendMouseCoords(ref Message m, Dictionary<string, string> data) {
            data["x"] = GetMouseX(m.LParam).ToString();
            data["y"] = GetMouseY(m.LParam).ToString();
            Point screenPos = GetScreenMousePosition(m.HWnd, m.LParam);
            data["sx"] = screenPos.X.ToString();
            data["sy"] = screenPos.Y.ToString();
        }
        void AppendTargetInfo(IntPtr hWnd, IDictionary<string, string> data) {
            IAccessible accessible = Win32.GetAccessibleObject(hWnd);
            AppendTargetInfoFromAccessible(accessible, data);
            string windowCaption = TrimStringValue(Win32.GetWindowText(hWnd));
            if (!String.IsNullOrEmpty(windowCaption))
                data["windowCaption"] = windowCaption;
        }
        void AppendTargetInfo(IntPtr hWnd, Point point, IDictionary<string, string> data) {
            IAccessible accessible = Win32.GetAccessibleObject(hWnd, point);
            AppendTargetInfoFromAccessible(accessible, data);
            string windowCaption = TrimStringValue(Win32.GetWindowText(hWnd));
            if (!String.IsNullOrEmpty(windowCaption))
                data["windowCaption"] = windowCaption;
        }
        string TrimStringValue(string value) {
            const int maxLength = 64;
            if (String.IsNullOrEmpty(value))
                return value;
            if (value.Length <= maxLength)
                return value;
            else
                return value.Substring(0, maxLength);
        }

        bool AppendTargetInfoFromAccessible(IAccessible accessible, IDictionary<string, string> data) {
            if (accessible == null)
                return false;

            data["accRole"] = accessible.accRole.ToString();
            bool result = false;
            string accName = accessible.accName;
            if (!String.IsNullOrEmpty(accName)) {
                data["accName"] = TrimStringValue(accName);
                result = true;
            }
            

            if (IsGridRole(accessible)) {
                return result;
            }

            IAccessible parent = accessible.accParent as IAccessible;
            if (parent == null) {
                return result;
            }

            if (IsGridRole(parent)) {
                data["parentAccRole"] = parent.accRole.ToString();
                string parentAccName = TrimStringValue(parent.accName.ToString());
                if (!String.IsNullOrEmpty(parentAccName))
                    data["parentAccName"] = parentAccName;
                return result;
            }
            else
                return result;
        }

        bool IsGridRole(IAccessible instance) {
            try {
                AccessibleRole role = (AccessibleRole)instance.accRole;
                return role == AccessibleRole.Cell || role == AccessibleRole.Row || role == AccessibleRole.RowHeader || role == AccessibleRole.Column || role == AccessibleRole.ColumnHeader;
            }
            catch {
                return false;
            }
        }
        protected override string GetThreadId() {
            return Win32.GetCurrentThreadId().ToString();
        }
    }
}