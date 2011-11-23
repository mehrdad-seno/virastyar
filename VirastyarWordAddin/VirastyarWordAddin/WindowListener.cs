using System;
using System.Collections.Generic;
using System.Diagnostics;
using SCICT.Microsoft.Windows;
using SCICT.Utility.Windows;

namespace VirastyarWordAddin
{
    public class WindowListener : IDisposable
    {
        /// <summary>
        /// handle to a Get-Message hook
        /// </summary>
        private IntPtr m_gmHookHandle = IntPtr.Zero;

        /// <summary>
        /// handle to a Call Windows Procedure which monitors messages sent to windows porcedures
        /// </summary>
        private IntPtr m_cwpHookHandle = IntPtr.Zero;

        /// <summary>
        /// Handle to the window being hooked
        /// </summary>
        private IntPtr m_windowHandle = IntPtr.Zero;

        private User32.GetMsgHookHandlerDelegate m_gmHookCallback;
        private User32.CWPMsgHookHandlerDelegate m_cwpHookCallback;

        private const int PM_NOREMOVE = 0;

        private readonly HashSet<int> m_setKeyboardIgnoreCodes = new HashSet<int>();


        public delegate void WinMessageEventHandler(object sender, WindowListenerEventArgs e);
        public event WinMessageEventHandler KeyPressed;
        public event WinMessageEventHandler KeyUp;
        public event WinMessageEventHandler KeyDown;
        public event WinMessageEventHandler LostFocus;
        public event WinMessageEventHandler GetFocus;
        public event WinMessageEventHandler PositionChanging;
        public event WinMessageEventHandler Paint;

        public event WinMessageEventHandler OtherMessage;

        //public bool Enabled { get; set; }

        public bool CheckIgnoreCodes { get; set; }

        public void AddKeyboardIgnoreCode(int code)
        {
            if (!m_setKeyboardIgnoreCodes.Contains(code))
                m_setKeyboardIgnoreCodes.Add(code);
        }

        public IntPtr Handle
        {
            get
            {
                return m_windowHandle;
            }
        }

        /// <summary>
        /// Sets the window handle to the hanlde provided and
        /// unhooks previous handles
        /// </summary>
        /// <param name="hWnd">The window handle to establish hook for</param>
        /// <returns><c>true</c> if the window handle has been set successfully; otherwise <c>false</c>.</returns>
        public bool SetWindowHandle(IntPtr hWnd)
        {
            if (m_windowHandle != hWnd)
            {
                Unhook();
                m_windowHandle = hWnd;
                return EstablishHook();
            }

            return true;
        }

        /// <summary>
        /// Determines whether the window associated with the window handle has focus.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the window associated with the window handle has focus; otherwise, <c>false</c>.
        /// </returns>
        public bool HasFocus()
        {
            return User32.GetFocus() == m_windowHandle;
        }

        /// <summary>
        /// Focuses the window associated with this instance.
        /// </summary>
        public void Focus()
        {
            if(!this.HasFocus())
                User32.SetFocus(m_windowHandle);
        }

        private IntPtr CwpHookCallBack(int nCode, IntPtr wParam, ref CWPSTRUCT lParam)
        {
            if (nCode < 0)
            {
                return User32.CallNextHookEx(m_gmHookHandle, nCode, wParam, ref lParam);
            }

            if (true) //(wParam.ToInt32() == PM_NOREMOVE)
            {
                if (lParam.hwnd == this.m_windowHandle)
                {
                    WindowListenerEventArgs eventArgs = new WindowListenerEventArgs();
                    eventArgs.handled = false;
                    eventArgs.message = (int)lParam.message;
                    eventArgs.lparam = lParam.lParam;
                    eventArgs.wparam = lParam.wParam;

                    Msgs wmMessage = (Msgs)lParam.message;

                    switch (wmMessage)
                    {
                        case Msgs.WM_KILLFOCUS:
                            if (LostFocus != null)
                            {
                                LostFocus(this, eventArgs);
                            }
                            break;
                        case Msgs.WM_SETFOCUS:
                            if (GetFocus != null)
                            {
                                GetFocus(this, eventArgs);
                            }
                            break;
                        case Msgs.WM_WINDOWPOSCHANGING:
                            if (PositionChanging != null)
                            {
                                PositionChanging(this, eventArgs);
                            }
                            break;
                        case Msgs.WM_PAINT:
                            if (this.Paint != null)
                            {
                                Paint(this, eventArgs);
                            }
                            break;
                        default:
                            if (OtherMessage != null)
                            {
                                OtherMessage(this, eventArgs);
                            }
                            break;
                    }

                    if (eventArgs.handled)
                    {
                        lParam.message = (uint)Msgs.WM_NULL;
                        lParam.lParam = 0;
                        lParam.wParam = 0;

                        return (IntPtr)1;
                    }
                }
            }

            return User32.CallNextHookEx(m_cwpHookHandle, nCode, wParam, ref lParam);
        }

        private int prevMsgType = -1;
        private int prevMsgTime = -1;
        private Msgs hndldMsgType = Msgs.WM_NULL;
        private int hndldMsgTime = -1;
        private int hndldMsgWP = -1;

        private IntPtr MsgHookCallBack(int nCode, IntPtr wParam, ref MSG lParam)
        {
            if (nCode < 0)
            {
                return User32.CallNextHookEx(m_gmHookHandle, nCode, wParam, ref lParam);
            }

            if (true) //(wParam.ToInt32() == PM_NOREMOVE)
            {
                bool isMsgHandled = false;

                if (lParam.hwnd == this.m_windowHandle)
                {
                    Msgs wmMessage = (Msgs)lParam.message;

                    // check for the messages that should be handled (, and ignored).
                    if (hndldMsgType != Msgs.WM_NULL)
                    {
                        if (lParam.wParam == hndldMsgWP)
                        {
                            switch (hndldMsgType)
                            {
                                case Msgs.WM_KEYUP:
                                case Msgs.WM_SYSKEYUP:
                                    hndldMsgType = Msgs.WM_NULL;
                                    break;
                                case Msgs.WM_KEYDOWN:
                                case Msgs.WM_SYSKEYDOWN:
                                    hndldMsgType = Msgs.WM_CHAR;
                                    break;
                                case Msgs.WM_CHAR:
                                case Msgs.WM_SYSCHAR:
                                    hndldMsgType = Msgs.WM_KEYUP;
                                    break;
                                default:
                                    hndldMsgType = Msgs.WM_NULL;
                                    break;
                            }

                            lParam.message = (uint)Msgs.WM_NULL;
                            lParam.wParam = 0;
                            lParam.lParam = 0;
                            isMsgHandled = true;
                        }
                        else
                        {
                            hndldMsgType = Msgs.WM_NULL;
                        }
                    }

                    if (!isMsgHandled &&
                        !(lParam.message == prevMsgType && lParam.time == prevMsgTime))
                    {
                        WindowListenerEventArgs eventArgs = new WindowListenerEventArgs();
                        eventArgs.handled = false;
                        eventArgs.message = (int)lParam.message;
                        eventArgs.lparam = lParam.lParam;
                        eventArgs.wparam = lParam.wParam;

                        prevMsgType = (int)lParam.message;
                        prevMsgTime = lParam.time;

                        switch (wmMessage)
                        {
                            case Msgs.WM_KEYUP:
                            case Msgs.WM_SYSKEYUP:
                                if (KeyUp != null)
                                {
                                    KeyUp(this, eventArgs);
                                }
                                break;
                            case Msgs.WM_KEYDOWN:
                            case Msgs.WM_SYSKEYDOWN:
                                if (KeyDown != null)
                                {
                                    KeyDown(this, eventArgs);
                                }
                                break;
                            case Msgs.WM_CHAR:
                            case Msgs.WM_SYSCHAR:
                                if (KeyPressed != null)
                                {
                                    KeyPressed(this, eventArgs);
                                }
                                break;
                            default:
                                if (OtherMessage != null)
                                {
                                    OtherMessage(this, eventArgs);
                                }
                                break;
                        }

                        if (eventArgs.handled)
                        {
                            hndldMsgTime = lParam.time;
                            hndldMsgType = wmMessage;
                            hndldMsgWP = lParam.time;

                            lParam.message = (uint)Msgs.WM_NULL;
                            lParam.lParam = 0;
                            lParam.wParam = 0;
                        }
                    }

                    if (!isMsgHandled && CheckIgnoreCodes)
                    {
                        if (wmMessage == Msgs.WM_KEYDOWN || wmMessage == Msgs.WM_KEYUP || wmMessage == Msgs.WM_CHAR ||
                            wmMessage == Msgs.WM_SYSKEYDOWN || wmMessage == Msgs.WM_SYSKEYUP || wmMessage == Msgs.WM_SYSCHAR)
                        {
                            if (m_setKeyboardIgnoreCodes.Contains(lParam.wParam))
                            {
                                lParam.message = (uint)Msgs.WM_NULL;
                                lParam.lParam = 0;
                                lParam.wParam = 0;
                            }
                        }
                    }
                }

                if (lParam.message == (uint)Msgs.WM_NULL && lParam.lParam == 0 && lParam.wParam == 0)
                    return (IntPtr)1;
            }

            return User32.CallNextHookEx(m_gmHookHandle, nCode, wParam, ref lParam);
        }

        #region Commented out (but do not delete)
        /// <summary>
        /// The old method which caused a lot of problems
        /// </summary>
        //private IntPtr MsgHookCallBackNoRepeatCheck(int nCode, IntPtr wParam, ref MSG lParam)
        //{
        //    if (nCode < 0)
        //    {
        //        return User32.CallNextHookEx(m_gmHookHandle, nCode, wParam, ref lParam);
        //    }

        //    if (lParam.hwnd == this.m_windowHandle)
        //    {
        //        WindowListenerEventArgs eventArgs = new WindowListenerEventArgs();
        //        eventArgs.handled = false;
        //        eventArgs.message = (int)lParam.message;
        //        eventArgs.lparam = lParam.lParam;
        //        eventArgs.wparam = lParam.wParam;

        //        Msgs wmMessage = (Msgs)lParam.message;

        //        switch (wmMessage)
        //        {
        //            case Msgs.WM_KEYUP:
        //            case Msgs.WM_SYSKEYUP:
        //                if (KeyUp != null)
        //                {
        //                    KeyUp(this, eventArgs);
        //                }
        //                break;
        //            case Msgs.WM_KEYDOWN:
        //            case Msgs.WM_SYSKEYDOWN:
        //                if (KeyDown != null)
        //                {
        //                    KeyDown(this, eventArgs);
        //                }
        //                break;
        //            case Msgs.WM_CHAR:
        //            case Msgs.WM_SYSCHAR:
        //                if (KeyPressed != null)
        //                {
        //                    KeyPressed(this, eventArgs);
        //                }
        //                break;
        //            default:
        //                if (OtherMessage != null)
        //                {
        //                    OtherMessage(this, eventArgs);
        //                }
        //                break;
        //        }

        //        if (eventArgs.handled)
        //        {
        //            lParam.message = (uint)Msgs.WM_NULL;
        //            lParam.lParam = 0;
        //            lParam.wParam = 0;
        //        }
        //    }

        //    //if (!isMsgHandled && CheckIgnoreCodes)
        //    //{
        //    //    if (wmMessage == Msgs.WM_KEYDOWN || wmMessage == Msgs.WM_KEYUP || wmMessage == Msgs.WM_CHAR ||
        //    //        wmMessage == Msgs.WM_SYSKEYDOWN || wmMessage == Msgs.WM_SYSKEYUP || wmMessage == Msgs.WM_SYSCHAR)
        //    //    {
        //    //        if (m_setKeyboardIgnoreCodes.Contains(lParam.wParam))
        //    //        {
        //    //            lParam.message = (uint)Msgs.WM_NULL;
        //    //            lParam.lParam = 0;
        //    //            lParam.wParam = 0;
        //    //        }
        //    //    }
        //    //}

        //    return User32.CallNextHookEx(m_gmHookHandle, nCode, wParam, ref lParam);
        //}

        #endregion

        private bool EstablishHook()
        {
            m_gmHookCallback  = new User32.GetMsgHookHandlerDelegate(MsgHookCallBack);
            m_cwpHookCallback = new User32.CWPMsgHookHandlerDelegate(CwpHookCallBack);

            using (Process curProc = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProc.MainModule)
            {
                IntPtr moduleHandle = Kernel32.GetModuleHandle(curModule.ModuleName);
                // TODO: this method is deprecated find another one
                uint curThreadID = (uint)AppDomain.GetCurrentThreadId();

                m_gmHookHandle = User32.SetWindowsHookEx((int)WindowsHooks.WH_GETMESSAGE, m_gmHookCallback,
                    moduleHandle, curThreadID);

                m_cwpHookHandle = User32.SetWindowsHookEx((int)WindowsHooks.WH_CALLWNDPROC, m_cwpHookCallback,
                    moduleHandle, curThreadID);
            }

            if (m_gmHookHandle == IntPtr.Zero || m_cwpHookHandle == IntPtr.Zero)
            {
                Unhook();
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Unhooks all hook handles and sets them to IntPtr.Zero
        /// </summary>
        private void Unhook()
        {
            if (m_gmHookHandle != IntPtr.Zero)
                User32.UnhookWindowsHookEx(m_gmHookHandle);

            m_gmHookHandle = IntPtr.Zero;

            if (m_cwpHookHandle != IntPtr.Zero)
                User32.UnhookWindowsHookEx(m_cwpHookHandle);
            m_cwpHookHandle = IntPtr.Zero;
        }

        #region Disposing stuff

        bool isDisposed = false;
        public void Dispose()
        {
            Unhook();
            isDisposed = true;
        }

        ~WindowListener()
        {
            if (!isDisposed)
                Dispose();
        }

        #endregion
    }

    public class WindowListenerEventArgs
    {
        public int message;
        public int lparam;
        public int wparam;
        public bool handled;
        public KeyState KeyStateMonitor;

        public WindowListenerEventArgs()
        {
            message = lparam = wparam = 0;
            handled = false;
            KeyStateMonitor.UpdateToCurrentStates();
        }
    }
}
