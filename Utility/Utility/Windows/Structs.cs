using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SCICT.Microsoft.Windows
{
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
