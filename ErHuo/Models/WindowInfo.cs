using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{
    public struct WindowInfo
    {
        public int hWnd;
        public string szWindowName;
        public WindowInfo(IntPtr hWnd, string szWindowName)
        {
            this.hWnd = (int)hWnd;
            this.szWindowName = szWindowName;
        }
        public WindowInfo(int hWnd, string szWindowName)
        {
            this.hWnd = hWnd;
            this.szWindowName = szWindowName;
        }
    }
}
