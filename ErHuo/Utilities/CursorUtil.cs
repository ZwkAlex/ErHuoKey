using ErHuo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Utilities
{

    public static class CursorUtil
    {
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetCursorPos(out CursorPoint lpPoint);

        public static CursorPoint doGetCursorPos()
        {
            CursorPoint point;
            IntPtr result = GetCursorPos(out point);
            if (result.ToInt32() == 1)
            {
                return point;
            }
            return new CursorPoint(0, 0);
        }

    }
}
