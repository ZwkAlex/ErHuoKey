using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CursorPoint
    {
        public int x;
        public int y;
        public CursorPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    };
}
