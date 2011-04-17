using System;
using System.Text;

namespace SCICT.Microsoft.Win32
{
    public struct KeyState
    {
        public bool Alt;
        public bool Shift;
        public bool Control;

        public bool CapsLock;
        public bool ScrollLock;
        public bool NumLock;

        public bool LeftShift;
        public bool LeftAlt;
        public bool LeftControl;

        public bool RightShift;
        public bool RightAlt;
        public bool RightControl;

        public void UpdateToCurrentStates()
        {
            Shift = User32.GetKeyState((int)VK.SHIFT) < 0;
            Alt = User32.GetKeyState((int)VK.MENU) < 0;
            Control = User32.GetKeyState((int)VK.CONTROL) < 0;

            CapsLock = Math.Abs(User32.GetKeyState((int)VK.CAPITAL)) > 0;
            ScrollLock = Math.Abs(User32.GetKeyState((int)VK.SCROLL)) > 0;
            NumLock = Math.Abs(User32.GetKeyState((int)VK.NUMLOCK)) > 0;

            LeftShift = User32.GetKeyState((int)VK.LSHIFT) < 0;
            LeftAlt = User32.GetKeyState((int)VK.LMENU) < 0;
            LeftControl = User32.GetKeyState((int)VK.LCONTROL) < 0;

            RightShift = User32.GetKeyState((int)VK.RSHIFT) < 0;
            RightAlt = User32.GetKeyState((int)VK.RMENU) < 0;
            RightControl = User32.GetKeyState((int)VK.RCONTROL) < 0;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("{0,-15}: {1}", "Alt", Alt));
            sb.AppendLine(String.Format("{0,-15}: {1}", "Control", Control));
            sb.AppendLine(String.Format("{0,-15}: {1}", "Shift", Shift));
            sb.AppendLine();
            sb.AppendLine(String.Format("{0,-15}: {1}", "CapsLock", CapsLock));
            sb.AppendLine(String.Format("{0,-15}: {1}", "ScrollLock", ScrollLock));
            sb.AppendLine(String.Format("{0,-15}: {1}", "NumLock", NumLock));
            sb.AppendLine();
            sb.AppendLine(String.Format("{0,-15}: {1}", "LeftAlt", LeftAlt));
            sb.AppendLine(String.Format("{0,-15}: {1}", "LeftControl", LeftControl));
            sb.AppendLine(String.Format("{0,-15}: {1}", "LeftShift", LeftShift));
            sb.AppendLine();
            sb.AppendLine(String.Format("{0,-15}: {1}", "RightAlt", RightAlt));
            sb.AppendLine(String.Format("{0,-15}: {1}", "RightControl", RightControl));
            sb.AppendLine(String.Format("{0,-15}: {1}", "RightShift", RightShift));

            return sb.ToString();
        }
    }
}
