﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CursorPoint
    {
        public int x;
        public int y;
        public CursorPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public override string ToString()
        {
            if (x == -1 && y == -1) return "无效点";
            return "( " + x + " , " + y + " )";
        }

        public string ToStringRelative(WindowRect screen)
        {
            return string.Format("( {0:0.00} , {1:0.00} )" , x * 1.0 / screen.W, y * 1.0 / screen.H);
        }
        public bool IsValid()
        {
            return (x != -1 || y != -1);
        }
    };
}
