using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo
{
    static class HwndUtil
    {

        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

        public struct WindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
        }

        public static IntPtr GetWindow(string WindowName)
        {
            WindowInfo[] AllWindows = GetAllDesktopWindows();
            foreach (WindowInfo win in AllWindows)
            {
                if (win.szWindowName.Contains(WindowName))
                    return win.hWnd;
            }
            return IntPtr.Zero;
        }
        public static WindowInfo[] GetAllDesktopWindows()
        {

            //用来保存窗口对象 列表
            List<WindowInfo> wndList = new List<WindowInfo>();
            WindowInfo default_win = new WindowInfo();
            default_win.hWnd = IntPtr.Zero;
            default_win.szWindowName = "无(前置按键)";
            wndList.Add(default_win);
            //enum all desktop windows 
            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);

                //get hwnd 
                wnd.hWnd = hWnd;

                //get window name  
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();
                if(sb.ToString() == "")
                {
                    return true;
                }
                //add it into list 
                wndList.Add(wnd);
                return true;
            }, 0);

            return wndList.ToArray();
        }

    }
}
