// Virastyar
// http://www.virastyar.ir
// Copyright (C) 2011 Supreme Council for Information and Communication Technology (SCICT) of Iran
// 
// This file is part of Virastyar.
// 
// Virastyar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Virastyar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Virastyar.  If not, see <http://www.gnu.org/licenses/>.
// 
// Additional permission under GNU GPL version 3 section 7
// The sole exception to the license's terms and requierments might be the
// integration of Virastyar with Microsoft Word (any version) as an add-in.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SCICT.Microsoft.Win32
{
    #region API Functions

    public class User32
    {
        [DllImport("user32.dll")]
        static extern int GetKeyNameText(int lParam, [Out] StringBuilder lpString, int nSize);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32", ExactSpelling = false, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("User32.dll")]
        public extern static IntPtr GetFocus();

        [DllImport("User32.dll")]
        public extern static IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll")]
        public extern static bool GetCaretPos(ref Point point);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int MapWindowPoints(HandleRef hWndFrom, HandleRef hWndTo, ref POINT pt, int cPoints);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr GetMsgHookHandlerDelegate(
            int nCode, IntPtr wParam, ref MSG lParam);

        public delegate IntPtr CWPMsgHookHandlerDelegate(
            int nCode, IntPtr wParam, ref CWPSTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook,
            GetMsgHookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook,
            CWPMsgHookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref MSG lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref CWPSTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int ToUnicodeEx(
            uint wVirtKey,
            uint wScanCode,
            Keys[] lpKeyState,
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags,
            IntPtr dwhkl);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr GetKeyboardLayout(uint threadId);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern bool GetKeyboardState(Keys[] keyStates);
    }

    public class Kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern int GlobalAddAtom(string Name);
        [DllImport("kernel32.dll")]
        public static extern int GlobalDeleteAtom(int atom);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }

    public class Gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject( int fnObject );

    }

    #endregion

    #region Structs
    /// <summary>
    /// Structure used by WH_GETMESSAGE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public int wParam;
        public int lParam;
        public int time;
        public POINT pt;
    }

    /// <summary>
    /// Message structure used by WH_CALLWNDPROC
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CWPSTRUCT
    {
        public int lParam;
        public int wParam;
        public uint message;
        public IntPtr hwnd;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GUITHREADINFO 
    {
        public int cbSize;
        public int flags;
        public IntPtr hwndActive;
        public IntPtr hwndFocus;
        public IntPtr hwndCapture;
        public IntPtr hwndMenuOwner;
        public IntPtr hwndMoveSize;
        public IntPtr hwndCaret;
        public Rectangle rcCaret;
    };

    #endregion

    #region Enums

    [Flags]
    public enum Modifiers 
    {
        None    = 0x0000, 
        Alt     = 0x0001, 
        Control = 0x0002, 
        Shift   = 0x0004, 
        Win     = 0x0008
    }

    public enum Msgs
    {
        WM_NULL = 0x0000,
        WM_CREATE = 0x0001,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_PAINT = 0x000F,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUIT = 0x0012,
        WM_QUERYOPEN = 0x0013,
        WM_ERASEBKGND = 0x0014,
        WM_SYSCOLORCHANGE = 0x0015,
        WM_ENDSESSION = 0x0016,
        WM_SHOWWINDOW = 0x0018,
        WM_WININICHANGE = 0x001A,
        WM_SETTINGCHANGE = 0x001A,
        WM_DEVMODECHANGE = 0x001B,
        WM_ACTIVATEAPP = 0x001C,
        WM_FONTCHANGE = 0x001D,
        WM_TIMECHANGE = 0x001E,
        WM_CANCELMODE = 0x001F,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEACTIVATE = 0x0021,
        WM_CHILDACTIVATE = 0x0022,
        WM_QUEUESYNC = 0x0023,
        WM_GETMINMAXINFO = 0x0024,
        WM_PAINTICON = 0x0026,
        WM_ICONERASEBKGND = 0x0027,
        WM_NEXTDLGCTL = 0x0028,
        WM_SPOOLERSTATUS = 0x002A,
        WM_DRAWITEM = 0x002B,
        WM_MEASUREITEM = 0x002C,
        WM_DELETEITEM = 0x002D,
        WM_VKEYTOITEM = 0x002E,
        WM_CHARTOITEM = 0x002F,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_SETHOTKEY = 0x0032,
        WM_GETHOTKEY = 0x0033,
        WM_QUERYDRAGICON = 0x0037,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003D,
        WM_COMPACTING = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_POWER = 0x0048,
        WM_COPYDATA = 0x004A,
        WM_CANCELJOURNAL = 0x004B,
        WM_NOTIFY = 0x004E,
        WM_INPUTLANGCHANGEREQUEST = 0x0050,
        WM_INPUTLANGCHANGE = 0x0051,
        WM_TCARD = 0x0052,
        WM_HELP = 0x0053,
        WM_USERCHANGED = 0x0054,
        WM_NOTIFYFORMAT = 0x0055,
        WM_CONTEXTMENU = 0x007B,
        WM_STYLECHANGING = 0x007C,
        WM_STYLECHANGED = 0x007D,
        WM_DISPLAYCHANGE = 0x007E,
        WM_GETICON = 0x007F,
        WM_SETICON = 0x0080,
        WM_NCCREATE = 0x0081,
        WM_NCDESTROY = 0x0082,
        WM_NCCALCSIZE = 0x0083,
        WM_NCHITTEST = 0x0084,
        WM_NCPAINT = 0x0085,
        WM_NCACTIVATE = 0x0086,
        WM_GETDLGCODE = 0x0087,
        WM_SYNCPAINT = 0x0088,
        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_CHAR = 0x0102,
        WM_DEADCHAR = 0x0103,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_SYSCHAR = 0x0106,
        WM_SYSDEADCHAR = 0x0107,
        WM_KEYLAST = 0x0108,
        WM_IME_STARTCOMPOSITION = 0x010D,
        WM_IME_ENDCOMPOSITION = 0x010E,
        WM_IME_COMPOSITION = 0x010F,
        WM_IME_KEYLAST = 0x010F,
        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111,
        WM_SYSCOMMAND = 0x0112,
        WM_TIMER = 0x0113,
        WM_HSCROLL = 0x0114,
        WM_VSCROLL = 0x0115,
        WM_INITMENU = 0x0116,
        WM_INITMENUPOPUP = 0x0117,
        WM_MENUSELECT = 0x011F,
        WM_MENUCHAR = 0x0120,
        WM_ENTERIDLE = 0x0121,
        WM_MENURBUTTONUP = 0x0122,
        WM_MENUDRAG = 0x0123,
        WM_MENUGETOBJECT = 0x0124,
        WM_UNINITMENUPOPUP = 0x0125,
        WM_MENUCOMMAND = 0x0126,
        WM_CTLCOLORMSGBOX = 0x0132,
        WM_CTLCOLOREDIT = 0x0133,
        WM_CTLCOLORLISTBOX = 0x0134,
        WM_CTLCOLORBTN = 0x0135,
        WM_CTLCOLORDLG = 0x0136,
        WM_CTLCOLORSCROLLBAR = 0x0137,
        WM_CTLCOLORSTATIC = 0x0138,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_MOUSEWHEEL = 0x020A,
        WM_PARENTNOTIFY = 0x0210,
        WM_ENTERMENULOOP = 0x0211,
        WM_EXITMENULOOP = 0x0212,
        WM_NEXTMENU = 0x0213,
        WM_SIZING = 0x0214,
        WM_CAPTURECHANGED = 0x0215,
        WM_MOVING = 0x0216,
        WM_DEVICECHANGE = 0x0219,
        WM_MDICREATE = 0x0220,
        WM_MDIDESTROY = 0x0221,
        WM_MDIACTIVATE = 0x0222,
        WM_MDIRESTORE = 0x0223,
        WM_MDINEXT = 0x0224,
        WM_MDIMAXIMIZE = 0x0225,
        WM_MDITILE = 0x0226,
        WM_MDICASCADE = 0x0227,
        WM_MDIICONARRANGE = 0x0228,
        WM_MDIGETACTIVE = 0x0229,
        WM_MDISETMENU = 0x0230,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_DROPFILES = 0x0233,
        WM_MDIREFRESHMENU = 0x0234,
        WM_IME_SETCONTEXT = 0x0281,
        WM_IME_NOTIFY = 0x0282,
        WM_IME_CONTROL = 0x0283,
        WM_IME_COMPOSITIONFULL = 0x0284,
        WM_IME_SELECT = 0x0285,
        WM_IME_CHAR = 0x0286,
        WM_IME_REQUEST = 0x0288,
        WM_IME_KEYDOWN = 0x0290,
        WM_IME_KEYUP = 0x0291,
        WM_MOUSEHOVER = 0x02A1,
        WM_MOUSELEAVE = 0x02A3,
        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CLEAR = 0x0303,
        WM_UNDO = 0x0304,
        WM_RENDERFORMAT = 0x0305,
        WM_RENDERALLFORMATS = 0x0306,
        WM_DESTROYCLIPBOARD = 0x0307,
        WM_DRAWCLIPBOARD = 0x0308,
        WM_PAINTCLIPBOARD = 0x0309,
        WM_VSCROLLCLIPBOARD = 0x030A,
        WM_SIZECLIPBOARD = 0x030B,
        WM_ASKCBFORMATNAME = 0x030C,
        WM_CHANGECBCHAIN = 0x030D,
        WM_HSCROLLCLIPBOARD = 0x030E,
        WM_QUERYNEWPALETTE = 0x030F,
        WM_PALETTEISCHANGING = 0x0310,
        WM_PALETTECHANGED = 0x0311,
        WM_HOTKEY = 0x0312,
        WM_PRINT = 0x0317,
        WM_PRINTCLIENT = 0x0318,
        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,
        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,
        WM_APP = 0x8000,
        WM_USER = 0x0400,
        WM_DDE_INITIATE = 0x03E0,
        WM_DDE_TERMINATE,
        WM_DDE_ADVISE,
        WM_DDE_UNADVISE,
        WM_DDE_ACK,
        WM_DDE_DATA,
        WM_DDE_REQUEST,
        WM_DDE_POKE,
        WM_DDE_EXECUTE
    }

    /// <summary>
    /// Virtual Keys
    /// </summary>
    public enum VK : uint
    {
        LBUTTON = 0x01,  // Left mouse button
        RBUTTON = 0x02,  // Right mouse button
        CANCEL = 0x03,  // Control-break processing
        MBUTTON = 0x04,  // Middle mouse button (three-button mouse)
        XBUTTON1 = 0x05,  // Windows 2000/XP: X1 mouse button
        XBUTTON2 = 0x06,  // Windows 2000/XP: X2 mouse button
        //            0x07   // Undefined
        BACK = 0x08,  // BACKSPACE key
        TAB = 0x09,  // TAB key
        //           0x0A-0x0B,  // Reserved
        CLEAR = 0x0C,  // CLEAR key
        RETURN = 0x0D,  // ENTER key
        //        0x0E-0x0F, // Undefined
        SHIFT = 0x10,  // SHIFT key
        CONTROL = 0x11,  // CTRL key
        MENU = 0x12,  // ALT key
        PAUSE = 0x13,  // PAUSE key
        CAPITAL = 0x14,  // CAPS LOCK key
        KANA = 0x15,  // Input Method Editor (IME) Kana mode
        HANGUL = 0x15,  // IME Hangul mode
        //            0x16,  // Undefined
        JUNJA = 0x17,  // IME Junja mode
        FINAL = 0x18,  // IME final mode
        HANJA = 0x19,  // IME Hanja mode
        KANJI = 0x19,  // IME Kanji mode
        //            0x1A,  // Undefined
        ESCAPE = 0x1B,  // ESC key
        CONVERT = 0x1C,  // IME convert
        NONCONVERT = 0x1D,  // IME nonconvert
        ACCEPT = 0x1E,  // IME accept
        MODECHANGE = 0x1F,  // IME mode change request
        SPACE = 0x20,  // SPACEBAR
        PRIOR = 0x21,  // PAGE UP key
        NEXT = 0x22,  // PAGE DOWN key
        END = 0x23,  // END key
        HOME = 0x24,  // HOME key
        LEFT = 0x25,  // LEFT ARROW key
        UP = 0x26,  // UP ARROW key
        RIGHT = 0x27,  // RIGHT ARROW key
        DOWN = 0x28,  // DOWN ARROW key
        SELECT = 0x29,  // SELECT key
        PRINT = 0x2A,  // PRINT key
        EXECUTE = 0x2B,  // EXECUTE key
        SNAPSHOT = 0x2C,  // PRINT SCREEN key
        INSERT = 0x2D,  // INS key
        DELETE = 0x2E,  // DEL key
        HELP = 0x2F,  // HELP key
        KEY_0 = 0x30, // 0 key
        KEY_1 = 0x31,  // 1 key
        KEY_2 = 0x32,  // 2 key
        KEY_3 = 0x33,  // 3 key
        KEY_4 = 0x34,  // 4 key
        KEY_5 = 0x35,  // 5 key
        KEY_6 = 0x36,  // 6 key
        KEY_7 = 0x37,  // 7 key
        KEY_8 = 0x38,  // 8 key
        KEY_9 = 0x39,  // 9 key
        //        0x3A-0x40, // Undefined
        KEY_A = 0x41,  // A key
        KEY_B = 0x42,  // B key
        KEY_C = 0x43,  // C key
        KEY_D = 0x44,  // D key
        KEY_E = 0x45,  // E key
        KEY_F = 0x46,  // F key
        KEY_G = 0x47,  // G key
        KEY_H = 0x48,  // H key
        KEY_I = 0x49,  // I key
        KEY_J = 0x4A,  // J key
        KEY_K = 0x4B,  // K key
        KEY_L = 0x4C,  // L key
        KEY_M = 0x4D,  // M key
        KEY_N = 0x4E,  // N key
        KEY_O = 0x4F,  // O key
        KEY_P = 0x50,  // P key
        KEY_Q = 0x51,  // Q key
        KEY_R = 0x52,  // R key
        KEY_S = 0x53,  // S key
        KEY_T = 0x54,  // T key
        KEY_U = 0x55,  // U key
        KEY_V = 0x56,  // V key
        KEY_W = 0x57,  // W key
        KEY_X = 0x58,  // X key
        KEY_Y = 0x59,  // Y key
        KEY_Z = 0x5A,  // Z key
        LWIN = 0x5B,  // Left Windows key (Microsoft Natural keyboard) 
        RWIN = 0x5C,  // Right Windows key (Natural keyboard)
        APPS = 0x5D,  // Applications key (Natural keyboard)
        //             0x5E, // Reserved
        SLEEP = 0x5F,  // Computer Sleep key
        NUMPAD0 = 0x60,  // Numeric keypad 0 key
        NUMPAD1 = 0x61,  // Numeric keypad 1 key
        NUMPAD2 = 0x62,  // Numeric keypad 2 key
        NUMPAD3 = 0x63,  // Numeric keypad 3 key
        NUMPAD4 = 0x64,  // Numeric keypad 4 key
        NUMPAD5 = 0x65,  // Numeric keypad 5 key
        NUMPAD6 = 0x66,  // Numeric keypad 6 key
        NUMPAD7 = 0x67,  // Numeric keypad 7 key
        NUMPAD8 = 0x68,  // Numeric keypad 8 key
        NUMPAD9 = 0x69,  // Numeric keypad 9 key
        MULTIPLY = 0x6A,  // Multiply key
        ADD = 0x6B,  // Add key
        SEPARATOR = 0x6C,  // Separator key
        SUBTRACT = 0x6D,  // Subtract key
        DECIMAL = 0x6E,  // Decimal key
        DIVIDE = 0x6F,  // Divide key
        F1 = 0x70,  // F1 key
        F2 = 0x71,  // F2 key
        F3 = 0x72,  // F3 key
        F4 = 0x73,  // F4 key
        F5 = 0x74,  // F5 key
        F6 = 0x75,  // F6 key
        F7 = 0x76,  // F7 key
        F8 = 0x77,  // F8 key
        F9 = 0x78,  // F9 key
        F10 = 0x79,  // F10 key
        F11 = 0x7A,  // F11 key
        F12 = 0x7B,  // F12 key
        F13 = 0x7C,  // F13 key
        F14 = 0x7D,  // F14 key
        F15 = 0x7E,  // F15 key
        F16 = 0x7F,  // F16 key
        F17 = 0x80,  // F17 key  
        F18 = 0x81,  // F18 key  
        F19 = 0x82,  // F19 key  
        F20 = 0x83,  // F20 key  
        F21 = 0x84,  // F21 key  
        F22 = 0x85,  // F22 key, (PPC only) Key used to lock device.
        F23 = 0x86,  // F23 key  
        F24 = 0x87,  // F24 key  
        //           0x88-0X8F,  // Unassigned
        NUMLOCK = 0x90,  // NUM LOCK key
        SCROLL = 0x91,  // SCROLL LOCK key
        //           0x92-0x96,  // OEM specific
        //           0x97-0x9F,  // Unassigned
        LSHIFT = 0xA0,  // Left SHIFT key
        RSHIFT = 0xA1,  // Right SHIFT key
        LCONTROL = 0xA2,  // Left CONTROL key
        RCONTROL = 0xA3,  // Right CONTROL key
        LMENU = 0xA4,  // Left MENU key
        RMENU = 0xA5,  // Right MENU key
        BROWSER_BACK = 0xA6,  // Windows 2000/XP: Browser Back key
        BROWSER_FORWARD = 0xA7,  // Windows 2000/XP: Browser Forward key
        BROWSER_REFRESH = 0xA8,  // Windows 2000/XP: Browser Refresh key
        BROWSER_STOP = 0xA9,  // Windows 2000/XP: Browser Stop key
        BROWSER_SEARCH = 0xAA,  // Windows 2000/XP: Browser Search key 
        BROWSER_FAVORITES = 0xAB,  // Windows 2000/XP: Browser Favorites key
        BROWSER_HOME = 0xAC,  // Windows 2000/XP: Browser Start and Home key
        VOLUME_MUTE = 0xAD,  // Windows 2000/XP: Volume Mute key
        VOLUME_DOWN = 0xAE,  // Windows 2000/XP: Volume Down key
        VOLUME_UP = 0xAF,  // Windows 2000/XP: Volume Up key
        MEDIA_NEXT_TRACK = 0xB0,  // Windows 2000/XP: Next Track key
        MEDIA_PREV_TRACK = 0xB1,  // Windows 2000/XP: Previous Track key
        MEDIA_STOP = 0xB2,  // Windows 2000/XP: Stop Media key
        MEDIA_PLAY_PAUSE = 0xB3,  // Windows 2000/XP: Play/Pause Media key
        LAUNCH_MAIL = 0xB4,  // Windows 2000/XP: Start Mail key
        LAUNCH_MEDIA_SELECT = 0xB5,  // Windows 2000/XP: Select Media key
        LAUNCH_APP1 = 0xB6,  // Windows 2000/XP: Start Application 1 key
        LAUNCH_APP2 = 0xB7,  // Windows 2000/XP: Start Application 2 key
        //           0xB8-0xB9,  // Reserved
        OEM_1 = 0xBA,  // Used for miscellaneous characters; it can vary by keyboard.
        // Windows 2000/XP: For the US standard keyboard, the ';:' key 
        OEM_PLUS = 0xBB,  // Windows 2000/XP: For any country/region, the '+' key
        OEM_COMMA = 0xBC,  // Windows 2000/XP: For any country/region, the ',' key
        OEM_MINUS = 0xBD,  // Windows 2000/XP: For any country/region, the '-' key
        OEM_PERIOD = 0xBE,  // Windows 2000/XP: For any country/region, the '.' key
        OEM_2 = 0xBF,  // Used for miscellaneous characters; it can vary by keyboard.
        // Windows 2000/XP: For the US standard keyboard, the '/?' key 
        OEM_3 = 0xC0,  // Used for miscellaneous characters; it can vary by keyboard. 
        // Windows 2000/XP: For the US standard keyboard, the '`~' key 
        //           0xC1-0xD7,  // Reserved
        //           0xD8-0xDA,  // Unassigned
        OEM_4 = 0xDB,  // Used for miscellaneous characters; it can vary by keyboard. 
        // Windows 2000/XP: For the US standard keyboard, the '[{' key
        OEM_5 = 0xDC,  // Used for miscellaneous characters; it can vary by keyboard. 
        // Windows 2000/XP: For the US standard keyboard, the '\|' key
        OEM_6 = 0xDD,  // Used for miscellaneous characters; it can vary by keyboard. 
        // Windows 2000/XP: For the US standard keyboard, the ']}' key
        OEM_7 = 0xDE,  // Used for miscellaneous characters; it can vary by keyboard. 
        // Windows 2000/XP: For the US standard keyboard, the 'single-quote/double-quote' key
        OEM_8 = 0xDF,  // Used for miscellaneous characters; it can vary by keyboard.
        //            0xE0,  // Reserved
        //            0xE1,  // OEM specific
        OEM_102 = 0xE2,  // Windows 2000/XP: Either the angle bracket key or the backslash key on the RT 102-key keyboard
        //         0xE3-E4,  // OEM specific
        PROCESSKEY = 0xE5,  // Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
        //            0xE6,  // OEM specific
        PACKET = 0xE7,  // Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
        //            0xE8,  // Unassigned
        //         0xE9-F5,  // OEM specific
        ATTN = 0xF6,  // Attn key
        CRSEL = 0xF7,  // CrSel key
        EXSEL = 0xF8,  // ExSel key
        EREOF = 0xF9,  // Erase EOF key
        PLAY = 0xFA,  // Play key
        ZOOM = 0xFB,  // Zoom key
        NONAME = 0xFC,  // Reserved 
        PA1 = 0xFD,  // PA1 key
        OEM_CLEAR = 0xFE  // Clear key
    }

    /// <summary>
    /// Windows Hook constants (hook ids)
    /// </summary>
    public enum WindowsHooks : int
    {
        WH_MIN = -1,
        WH_MSGFILTER = -1,
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    public enum StockObjects : int
    {
        WHITE_BRUSH = 0,
        LTGRAY_BRUSH = 1,
        GRAY_BRUSH = 2,
        DKGRAY_BRUSH = 3,
        BLACK_BRUSH = 4,
        NULL_BRUSH = 5,
        HOLLOW_BRUSH = 5,
        WHITE_PEN = 6,
        BLACK_PEN = 7,
        NULL_PEN = 8,
        OEM_FIXED_FONT = 10,
        ANSI_FIXED_FONT = 11,
        ANSI_VAR_FONT = 12,
        SYSTEM_FONT = 13,
        DEVICE_DEFAULT_FONT = 14,
        DEFAULT_PALETTE = 15,
        SYSTEM_FIXED_FONT = 16,
        DEFAULT_GUI_FONT = 17,
        DC_BRUSH = 18,
        DC_PEN = 19
    }

    [Flags]
    public enum WindowStyles : uint
    {
        WS_OVERLAPPED = 0x00000000,
        WS_POPUP = 0x80000000,
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_GROUP = 0x00020000,
        WS_TABSTOP = 0x00010000,

        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,

        WS_CAPTION = WS_BORDER | WS_DLGFRAME,
        WS_TILED = WS_OVERLAPPED,
        WS_ICONIC = WS_MINIMIZE,
        WS_SIZEBOX = WS_THICKFRAME,
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
        WS_CHILDWINDOW = WS_CHILD,

        //Extended Window Styles

        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_TRANSPARENT = 0x00000020,

        //#if(WINVER >= 0x0400)

        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_WINDOWEDGE = 0x00000100,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,

        WS_EX_RIGHT = 0x00001000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,

        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,

        WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),

        //#endif /* WINVER >= 0x0400 */

        //#if(WIN32WINNT >= 0x0500)

        WS_EX_LAYERED = 0x00080000,

        //#endif /* WIN32WINNT >= 0x0500 */

        //#if(WINVER >= 0x0500)

        WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
        WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring

        //#endif /* WINVER >= 0x0500 */

        //#if(WIN32WINNT >= 0x0500)

        WS_EX_COMPOSITED = 0x02000000,
        WS_EX_NOACTIVATE = 0x08000000

        //#endif /* WIN32WINNT >= 0x0500 */

    }

    #endregion

    /// <summary>
    /// Defines a delegate for Message handling
    /// </summary>
    public delegate void MessageEventHandler(object Sender, ref Message msg, ref bool Handled);

    /// <summary>
    /// Inherits from System.Windows.Form.NativeWindow. Provides an Event for Message handling
    /// </summary>
    public class NativeWindowWithEvent : System.Windows.Forms.NativeWindow
    {
        public event MessageEventHandler ProcessMessage;
        protected override void WndProc(ref Message m)
        {
            if (ProcessMessage != null)
            {
                bool Handled = false;
                ProcessMessage(this, ref m, ref Handled);
                if (!Handled) base.WndProc(ref m);
            }
            else base.WndProc(ref m);
        }
    }

    /// <summary>
    /// Inherits from NativeWindowWithEvent and automatic creates/destroys of a dummy window
    /// </summary>
    public class DummyWindowWithEvent : NativeWindowWithEvent, IDisposable
    {
        public DummyWindowWithEvent()
        {
            CreateParams parms = new CreateParams();
            this.CreateHandle(parms);
        }

        public void Dispose()
        {
            if (this.Handle != (IntPtr)0)
            {
                this.DestroyHandle();
            }
        }
    }
}
