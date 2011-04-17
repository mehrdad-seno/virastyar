using System;
using System.Windows.Forms;
using SCICT.Microsoft.Win32;

namespace VirastyarWordAddin.Verifiers.Basics
{
    public class WindowWrapper : IWin32Window
    {
        private static IntPtr GetWindowHandle()
        {
            IntPtr hActiveWindow = User32.GetActiveWindow();
            return hActiveWindow;
        }

        public static WindowWrapper GetWordActiveWindowWrapper()
        {
            return new WindowWrapper(GetWindowHandle());
        }

        public WindowWrapper(IntPtr handle)
        {
            m_hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return m_hwnd; }
        }

        private IntPtr m_hwnd;
    }
}
