using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sa
{
    public partial class WinApi
    {
        //------------------------------------------------------------------------------------------------------------------------
        [DllImport("User32.dll", EntryPoint = "PostMessageA")]
        public static extern Boolean PostMessage(
            IntPtr hWnd,            // handle to destination window
            int Msg,                // message
            int wParam,             // first message parameter
            int lParam);            // second message parameter

        //------------------------------------------------------------------------------------------
        [DllImport("User32.dll", EntryPoint = "PostMessageA")]
        public static extern Boolean PostMessage(
            IntPtr hWnd,            // handle to destination window
            int Msg,                // message
            IntPtr wParam,          // first message parameter
            int lParam);            // second message parameter

        //------------------------------------------------------------------------------------------------------------------------
        [DllImport("User32.dll", EntryPoint = "SendMessageA")]
        public static extern Boolean SendMessage(
            IntPtr hWnd,            // handle to destination window
            int Msg,                // message
            int wParam,             // first message parameter
            int lParam);            // second message parameter

        //------------------------------------------------------------------------------------------
        [DllImport("User32.dll", EntryPoint = "SendMessageA")]
        public static extern Boolean SendMessage(
            IntPtr hWnd,            // handle to destination window
            int Msg,                // message
            IntPtr wParam,          // first message parameter
            int lParam);            // second message parameter

        //------------------------------------------------------------------------------------------
        [DllImport("User32.dll", EntryPoint = "SendMessageA")]
        public static extern Boolean SendMessage(
             IntPtr hWnd,
             int Msg,
             int wParam,
             ref CopyDataStruct lParam);

    }
}
