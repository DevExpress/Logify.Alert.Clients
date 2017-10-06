using Accessibility;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace DevExpress.Logify.Core.Internal {
    [SecuritySafeCritical]
    public static class Win32 {
        public delegate int HookProc(int ncode, IntPtr wParam, IntPtr lParam);
        #region Enums and Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int X;
            public int Y;
        }
        [StructLayout(LayoutKind.Sequential)]
        [CLSCompliant(false)]
        public struct MOUSEHOOKSTRUCT {
            public POINT Pt;
            public IntPtr hwnd;
            public uint wHitTestCode;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        [CLSCompliant(false)]
        public class MOUSEHOOKSTRUCTEX {
            public MOUSEHOOKSTRUCT Mouse;
            public int mouseData;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct API_MSG {
            public IntPtr Hwnd;
            public int Msg;
            public IntPtr WParam;
            public IntPtr LParam;
            public int Time;
            public POINT Pt;
            public Message ToMessage() {
                System.Windows.Forms.Message res = new System.Windows.Forms.Message();
                res.HWnd = this.Hwnd;
                res.Msg = this.Msg;
                res.WParam = this.WParam;
                res.LParam = this.LParam;
                return res;
            }
            public void FromMessage(ref Message msg) {
                this.Hwnd = msg.HWnd;
                this.Msg = msg.Msg;
                this.WParam = msg.WParam;
                this.LParam = msg.LParam;
            }
        }
        #region WindowsMessages
        public enum WindowsMessages {
            WM_NULL = 0x00,
            WM_CREATE = 0x01,
            WM_DESTROY = 0x02,
            WM_MOVE = 0x03,
            WM_SIZE = 0x05,
            WM_ACTIVATE = 0x06,
            WM_SETFOCUS = 0x07,
            WM_KILLFOCUS = 0x08,
            WM_ENABLE = 0x0A,
            WM_SETREDRAW = 0x0B,
            WM_SETTEXT = 0x0C,
            WM_GETTEXT = 0x0D,
            WM_GETTEXTLENGTH = 0x0E,
            WM_PAINT = 0x0F,
            WM_CLOSE = 0x10,
            WM_QUERYENDSESSION = 0x11,
            WM_QUIT = 0x12,
            WM_QUERYOPEN = 0x13,
            WM_ERASEBKGND = 0x14,
            WM_SYSCOLORCHANGE = 0x15,
            WM_ENDSESSION = 0x16,
            WM_SYSTEMERROR = 0x17,
            WM_SHOWWINDOW = 0x18,
            WM_CTLCOLOR = 0x19,
            WM_WININICHANGE = 0x1A,
            WM_SETTINGCHANGE = 0x1A,
            WM_DEVMODECHANGE = 0x1B,
            WM_ACTIVATEAPP = 0x1C,
            WM_FONTCHANGE = 0x1D,
            WM_TIMECHANGE = 0x1E,
            WM_CANCELMODE = 0x1F,
            WM_SETCURSOR = 0x20,
            WM_MOUSEACTIVATE = 0x21,
            WM_CHILDACTIVATE = 0x22,
            WM_QUEUESYNC = 0x23,
            WM_GETMINMAXINFO = 0x24,
            WM_PAINTICON = 0x26,
            WM_ICONERASEBKGND = 0x27,
            WM_NEXTDLGCTL = 0x28,
            WM_SPOOLERSTATUS = 0x2A,
            WM_DRAWITEM = 0x2B,
            WM_MEASUREITEM = 0x2C,
            WM_DELETEITEM = 0x2D,
            WM_VKEYTOITEM = 0x2E,
            WM_CHARTOITEM = 0x2F,

            WM_SETFONT = 0x30,
            WM_GETFONT = 0x31,
            WM_SETHOTKEY = 0x32,
            WM_GETHOTKEY = 0x33,
            WM_QUERYDRAGICON = 0x37,
            WM_COMPAREITEM = 0x39,
            WM_COMPACTING = 0x41,
            WM_WINDOWPOSCHANGING = 0x46,
            WM_WINDOWPOSCHANGED = 0x47,
            WM_POWER = 0x48,
            WM_COPYDATA = 0x4A,
            WM_CANCELJOURNAL = 0x4B,
            WM_NOTIFY = 0x4E,
            WM_INPUTLANGCHANGEREQUEST = 0x50,
            WM_INPUTLANGCHANGE = 0x51,
            WM_TCARD = 0x52,
            WM_HELP = 0x53,
            WM_USERCHANGED = 0x54,
            WM_NOTIFYFORMAT = 0x55,
            WM_CONTEXTMENU = 0x7B,
            WM_STYLECHANGING = 0x7C,
            WM_STYLECHANGED = 0x7D,
            WM_DISPLAYCHANGE = 0x7E,
            WM_GETICON = 0x7F,
            WM_SETICON = 0x80,

            WM_NCCREATE = 0x81,
            WM_NCDESTROY = 0x82,
            WM_NCCALCSIZE = 0x83,
            WM_NCHITTEST = 0x84,
            WM_NCPAINT = 0x85,
            WM_NCACTIVATE = 0x86,
            WM_GETDLGCODE = 0x87,
            WM_NCMOUSEMOVE = 0xA0,
            WM_NCLBUTTONDOWN = 0xA1,
            WM_NCLBUTTONUP = 0xA2,
            WM_NCLBUTTONDBLCLK = 0xA3,
            WM_NCRBUTTONDOWN = 0xA4,
            WM_NCRBUTTONUP = 0xA5,
            WM_NCRBUTTONDBLCLK = 0xA6,
            WM_NCMBUTTONDOWN = 0xA7,
            WM_NCMBUTTONUP = 0xA8,
            WM_NCMBUTTONDBLCLK = 0xA9,

            EM_GETPASSWORDCHAR = 0xD2,
            EM_SETPASSWORDCHAR = 0xCC,

            WM_KEYFIRST = 0x100,
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_CHAR = 0x102,
            WM_DEADCHAR = 0x103,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_SYSCHAR = 0x106,
            WM_SYSDEADCHAR = 0x107,
            WM_KEYLAST = 0x108,

            WM_UNICHAR = 0x109,
            WM_WNT_CONVERTREQUESTEX = 0x109,
            WM_CONVERTREQUEST = 0x10A,
            WM_CONVERTRESULT = 0x10B,
            WM_INTERIM = 0x10C,

            WM_IME_STARTCOMPOSITION = 0x10D,
            WM_IME_ENDCOMPOSITION = 0x10E,
            WM_IME_COMPOSITION = 0x10F,
            WM_IME_KEYLAST = 0x10F,

            WM_INITDIALOG = 0x110,
            WM_COMMAND = 0x111,
            WM_SYSCOMMAND = 0x112,
            WM_TIMER = 0x113,
            WM_HSCROLL = 0x114,
            WM_VSCROLL = 0x115,
            WM_INITMENU = 0x116,
            WM_INITMENUPOPUP = 0x117,
            WM_SYSTIMER = 0x118,
            WM_MENUSELECT = 0x11F,
            WM_MENUCHAR = 0x120,
            WM_ENTERIDLE = 0x121,

            WM_CTLCOLORMSGBOX = 0x132,
            WM_CTLCOLOREDIT = 0x133,
            WM_CTLCOLORLISTBOX = 0x134,
            WM_CTLCOLORBTN = 0x135,
            WM_CTLCOLORDLG = 0x136,
            WM_CTLCOLORSCROLLBAR = 0x137,
            WM_CTLCOLORSTATIC = 0x138,

            WM_MOUSEFIRST = 0x200,
            WM_MOUSEMOVE = 0x200,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_LBUTTONDBLCLK = 0x203,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RBUTTONDBLCLK = 0x206,
            WM_MBUTTONDOWN = 0x207,
            WM_MBUTTONUP = 0x208,
            WM_MBUTTONDBLCLK = 0x209,
            WM_MOUSEWHEEL = 0x20A,
            WM_MOUSEHWHEEL = 0x20E,

            WM_PARENTNOTIFY = 0x210,
            WM_ENTERMENULOOP = 0x211,
            WM_EXITMENULOOP = 0x212,
            WM_NEXTMENU = 0x213,
            WM_SIZING = 0x214,
            WM_CAPTURECHANGED = 0x215,
            WM_MOVING = 0x216,
            WM_POWERBROADCAST = 0x218,
            WM_DEVICECHANGE = 0x219,

            WM_MDICREATE = 0x220,
            WM_MDIDESTROY = 0x221,
            WM_MDIACTIVATE = 0x222,
            WM_MDIRESTORE = 0x223,
            WM_MDINEXT = 0x224,
            WM_MDIMAXIMIZE = 0x225,
            WM_MDITILE = 0x226,
            WM_MDICASCADE = 0x227,
            WM_MDIICONARRANGE = 0x228,
            WM_MDIGETACTIVE = 0x229,
            WM_MDISETMENU = 0x230,
            WM_ENTERSIZEMOVE = 0x231,
            WM_EXITSIZEMOVE = 0x232,
            WM_DROPFILES = 0x233,
            WM_MDIREFRESHMENU = 0x234,

            WM_IME_SETCONTEXT = 0x281,
            WM_IME_NOTIFY = 0x282,
            WM_IME_CONTROL = 0x283,
            WM_IME_COMPOSITIONFULL = 0x284,
            WM_IME_SELECT = 0x285,
            WM_IME_CHAR = 0x286,
            WM_IME_KEYDOWN = 0x290,
            WM_IME_KEYUP = 0x291,

            WM_MOUSEHOVER = 0x2A1,
            WM_NCMOUSELEAVE = 0x2A2,
            WM_MOUSELEAVE = 0x2A3,

            WM_CUT = 0x300,
            WM_COPY = 0x301,
            WM_PASTE = 0x302,
            WM_CLEAR = 0x303,
            WM_UNDO = 0x304,

            WM_RENDERFORMAT = 0x305,
            WM_RENDERALLFORMATS = 0x306,
            WM_DESTROYCLIPBOARD = 0x307,
            WM_DRAWCLIPBOARD = 0x308,
            WM_PAINTCLIPBOARD = 0x309,
            WM_VSCROLLCLIPBOARD = 0x30A,
            WM_SIZECLIPBOARD = 0x30B,
            WM_ASKCBFORMATNAME = 0x30C,
            WM_CHANGECBCHAIN = 0x30D,
            WM_HSCROLLCLIPBOARD = 0x30E,
            WM_QUERYNEWPALETTE = 0x30F,
            WM_PALETTEISCHANGING = 0x310,
            WM_PALETTECHANGED = 0x311,

            WM_HOTKEY = 0x312,
            WM_PRINT = 0x317,
            WM_PRINTCLIENT = 0x318,

            WM_HANDHELDFIRST = 0x358,
            WM_HANDHELDLAST = 0x35F,
            WM_PENWINFIRST = 0x380,
            WM_PENWINLAST = 0x38F,
            WM_COALESCE_FIRST = 0x390,
            WM_COALESCE_LAST = 0x39F,
            WM_DDE_FIRST = 0x3E0,
            WM_DDE_INITIATE = 0x3E0,
            WM_DDE_TERMINATE = 0x3E1,
            WM_DDE_ADVISE = 0x3E2,
            WM_DDE_UNADVISE = 0x3E3,
            WM_DDE_ACK = 0x3E4,
            WM_DDE_DATA = 0x3E5,
            WM_DDE_REQUEST = 0x3E6,
            WM_DDE_POKE = 0x3E7,
            WM_DDE_EXECUTE = 0x3E8,
            WM_DDE_LAST = 0x3E8,

            WM_USER = 0x400,
            WM_APP = 0x8000,
        }
        #endregion
        #region CbtEvent
        public enum CbtEvent {
            HCBT_MOVESIZE = 0x0,
            HCBT_MINMAX = 0x1,
            HCBT_QS = 0x2,
            HCBT_CREATEWND = 0x3,
            HCBT_DESTROYWND = 0x4,
            HCBT_ACTIVATE = 0x5,
            HCBT_CLICKSKIPPED = 0x6,
            HCBT_KEYSKIPPED = 0x7,
            HCBT_SYSCOMMAND = 0x8,
            HCBT_SETFOCUS = 0x9,
        }
        #endregion
        #endregion

        public static IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId) {
            return UnsafeNativeMethods.SetWindowsHookEx(idHook, lpfn, hMod, dwThreadId);
        }
        public static bool UnhookWindowsHookEx(IntPtr hhk) {
            return UnsafeNativeMethods.UnhookWindowsHookEx(hhk);
        }
        public static int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam) {
            return UnsafeNativeMethods.CallNextHookEx(hhk, nCode, wParam, lParam);
        }
        public static int GetCurrentThreadId() { return UnsafeNativeMethods.GetCurrentThreadIdSafe(); }
        public static string GetAccessibleName(IntPtr hWnd) {
            try {
                Guid guid = new Guid("618736e0-3c3d-11cf-810c-00aa00389b71");
                Object instance = null;
                const int OBJID_WINDOW = 0x0;
                int hResult = UnsafeNativeMethods.AccessibleObjectFromWindow(hWnd, OBJID_WINDOW, ref guid, ref instance);
                if (hResult != 0 || instance == null)
                    return String.Empty;
                IAccessible accObj = instance as IAccessible;
                if (accObj == null)
                    return String.Empty;
                return accObj.accName;
            }
            catch {
                return String.Empty;
            }
        }
        public static string GetAccessibleName(IntPtr hWnd, Point point) {
            try {
                IAccessible accObj;
                object obj = new object();
                POINT pt = new POINT();
                pt.X = point.X;
                pt.Y = point.Y;
                UnsafeNativeMethods.ClientToScreen(hWnd, ref pt);
                if (UnsafeNativeMethods.AccessibleObjectFromPoint(pt, out accObj, out obj) != IntPtr.Zero)
                    return String.Empty;
                if (accObj == null)
                    return String.Empty;
                return accObj.accName;
            }
            catch {
                return String.Empty;
            }
        }


        [ThreadStatic]
        static StringBuilder windowText;
        internal static string GetWindowText(IntPtr hWnd) {
            if (windowText == null)
                windowText = new StringBuilder(1024);
            int result = UnsafeNativeMethods.GetWindowText(hWnd, windowText, 1024);
            if (result == 0)
                return String.Empty;
            return windowText.ToString();
        }
        public enum WindowLongParam {
            GWL_WNDPROC = -4,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20,
            GWL_USERDATA = -21,
            DWLP_MSGRESULT = 0,
            DWLP_USER = 8,
            DWLP_DLGPROC = 4
        }
        public static int GetWindowLong(IntPtr hWnd, WindowLongParam flags) {
            return UnsafeNativeMethods.GetWindowLong(hWnd, (int)flags);
        }

        public static Point PointFromLParam(IntPtr lParam) {
            long value = lParam.ToInt64();
            short x = unchecked((short)(value & 0xFFFF));
            short y = unchecked((short)(((value & 0xFFFF0000) >> 16)));
            return new Point(x, y);
        }
        [ThreadStatic]
        static StringBuilder className;
        public static bool IsPasswordBox(IntPtr hWnd) {
            if (className == null)
                className = new StringBuilder(1024);

            if (UnsafeNativeMethods.GetClassName(hWnd, className, 1024) <= 0)
                return false;
            //if (String.Compare(className.ToString(), "EDIT", StringComparison.InvariantCultureIgnoreCase) != 0)
            //    return false;

            const int ES_PASSWORD = 32;
            int style = GetWindowLong(hWnd, Win32.WindowLongParam.GWL_STYLE);
            if ((style & ES_PASSWORD) == 0)
                return false;

            int result = SendMessage(hWnd, WindowsMessages.EM_GETPASSWORDCHAR, IntPtr.Zero, IntPtr.Zero);
            return result != 0;
        }
        public static int SendMessage(IntPtr hWnd, Win32.WindowsMessages Msg, IntPtr wParam, IntPtr lParam) {
            return UnsafeNativeMethods.SendMessage(hWnd, (int)Msg, wParam, lParam);
        }


        #region UnsafeNativeMethods
        static class UnsafeNativeMethods {
            [DllImport("oleacc.dll")]
            internal static extern IntPtr AccessibleObjectFromPoint(POINT pt, [Out, MarshalAs(UnmanagedType.Interface)] out IAccessible accObj, [Out] out object ChildID);
            [DllImport("oleacc.dll")]
            internal static extern int AccessibleObjectFromWindow(IntPtr hwnd, int id, ref Guid iid,  [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool ClientToScreen(IntPtr hwnd, ref POINT point);
            [DllImport("USER32.dll")]
            internal static extern int GetWindowLong(IntPtr hwnd, int flags);
            [DllImport("user32.dll")]
            internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
            [SecuritySafeCritical]
            internal static int GetCurrentThreadIdSafe() { return GetCurrentThreadIdCore(); }
            [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto, EntryPoint = "GetCurrentThreadId")]
            static extern int GetCurrentThreadIdCore();
            [DllImport("USER32.dll", CharSet = CharSet.Auto)]
            internal static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);
            [DllImport("USER32.dll", CharSet = CharSet.Auto)]
            internal static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
            [DllImport("USER32.dll", CharSet = CharSet.Auto)]
            internal static extern bool UnhookWindowsHookEx(IntPtr hhk);
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
            [DllImport("USER32.dll")]
            internal static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        }
        #endregion
    }
}
