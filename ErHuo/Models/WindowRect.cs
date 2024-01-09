using ErHuo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{
    public class WindowRect
    {
        public CursorPoint TopLeft;
        public CursorPoint BottomRight;
        public int H;
        public int W;

        public WindowRect(int x1, int y1, int x2, int y2): this(new CursorPoint(x1, y1), new CursorPoint(x2, y2))
        { 
        }

        public WindowRect(int x2, int y2): this(new CursorPoint(), new CursorPoint(x2, y2))
        {
        }

        public WindowRect(CursorPoint bottomRight): this(new CursorPoint(), bottomRight)
        {
        }

        public WindowRect(CursorPoint topLeft, CursorPoint bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            W = bottomRight.x - topLeft.x;
            H = bottomRight.y - topLeft.y;
        }

        public WindowRect SubRect(double xP, double yP, double wP, double hP)
        {
            int xC = (int)(W * xP);
            int yC = (int)(H * yP);
            int x1 = (int)(xC - W * wP);
            int y1 = (int)(yC - H * hP);
            int x2 = (int)(xC + W * wP);
            int y2 = (int)(yC + H * hP);
            return new WindowRect(x1, y1, x2, y2);
        }

        public WindowRect SubRect(CursorPoint anchor, double wP, double hP)
        {
            int x1 = (int)(anchor.x - W * wP);
            int y1 = (int)(anchor.y - H * hP);
            int x2 = (int)(anchor.x + W * wP);
            int y2 = (int)(anchor.y + H * hP);
            return new WindowRect(x1, y1, x2, y2);
        }

        public CursorPoint ReleventPointToAbsolute(double xP, double yP)
        {
            return new CursorPoint((int)(W * xP), (int)(H * yP));
        }
    }
}
