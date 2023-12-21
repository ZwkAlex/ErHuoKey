using ErHuo.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ErHuo.Utilities
{
    public static class HwndUtil
    {

        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

        public static int GetWindow(string WindowName)
        {
            List<WindowInfo> AllWindows = GetAllDesktopWindows();
            foreach (WindowInfo win in AllWindows)
            {
                if (win.szWindowName.Contains(WindowName))
                    return win.hWnd;
            }
            return 0;
        }
        public static List<WindowInfo> GetAllDesktopWindows()
        {
            //用来保存窗口对象 列表
            List<WindowInfo> wndList = new List<WindowInfo>();
            WindowInfo default_win = new WindowInfo
            {
                hWnd = 0,
                szWindowName = "无(前置按键)"
            };
            wndList.Add(default_win);
            //enum all desktop windows 
            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                
                StringBuilder sb = new StringBuilder(32);
                //get window name  
                GetWindowTextW(hWnd, sb, sb.Capacity);

                WindowInfo wnd = new WindowInfo(hWnd, sb.ToString());
                if (sb.ToString() == "" || !IsWindowVisible(hWnd))
                {
                    return true;
                }
                //add it into list 
                wndList.Add(wnd);
                return true;
            }, 0);

            return wndList;
        }

    }
}
