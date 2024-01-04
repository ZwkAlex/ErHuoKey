using ErHuo.Models;
using ErHuo.Utilities;
using lw;
using Newtonsoft.Json.Linq;

//using opLib;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ErHuo.Plugins
{
    public class P : IDisposable
    {
        private lwsoft lw;
        private bool disposed = false;
        //private IOpInterface op = new OpInterface();

        public P()
        {
            try
            {
                lw = new lwsoft();
            }
            catch
            {
                lw = null;
            }
        }
        public bool BindWindow(int hwnd, string dispaly = "normal", string mouse = "windows", string keyboard = "windows", int mode = 0)
        {
            int? result;
            if (hwnd == -1)
            {
                result = lw?.BindWindow(hwnd);
            }
            else
            {
                result = lw?.BindWindow(hwnd, Constant.LwKeyMode[dispaly], Constant.LwKeyMode[mouse], Constant.LwKeyMode[keyboard], mode);
            }

            return result != null && result != 0;
        }

        public WindowInfo FindWindow(string title)
        {
            int hWnd = lw?.FindWindow(title) ?? -1;
            string whole_title = lw?.GetWindowTitle(hWnd);
            return new WindowInfo(hWnd, whole_title);
        }

        public WindowInfo FindWindowJX3()
        {
            return FindWindow(Constant.JX3);
        }

        public bool UnBindWindow()
        {
            int? result;
            result = lw?.UnBindWindow();
            return result != null && result != 0;
        }

        public bool KeyPress(int key)
        {
            int? result;
            result = lw?.KeyPress(key);
            return result != null && result != 0;
        }

        public WindowInfo GetMousePointWindow()
        {
            int hwnd = lw?.GetMousePointWindow() ?? -1;
            string windowTitle = lw?.GetWindowTitle(hwnd);
            if (windowTitle == null || windowTitle == string.Empty)
            {
                windowTitle = lw?.GetWindowClass(hwnd);
            }
            return new WindowInfo(hwnd, windowTitle);
        }

        public int WaitKey(int key, int time_out)
        {
            int? result;
            try
            {
                result = lw?.WaitKey(key, time_out);
            }
            catch (Exception)
            {
                return 0;
            }

            return result ?? 0;
        }

        public bool LeftClick()
        {
            int? result;
            result = lw?.LeftClick();
            return result != null && result != 0;
        }

        public bool RightClick()
        {
            int? result;
            result = lw?.RightClick();
            return result != null && result != 0;
        }

        public bool MiddleClick()
        {
            int? result;
            result = lw?.MiddleClick();
            return result != null && result != 0;
        }

        public bool WheelDown()
        {
            int? result;
            result = lw?.WheelDown();
            return result != null && result != 0;
        }

        public bool WheelUp()
        {
            int? result;
            result = lw?.WheelUp();
            return result != null && result != 0;
        }

        public bool MoveTo(CursorPoint cursorPoint)
        {
            int? result;
            result = lw?.MoveTo(cursorPoint.x, cursorPoint.y);
            return result != null && result != 0;
        }

        public CursorPoint FindPic(CursorPoint point1, CursorPoint point2, string picPath, double sim = 0.95)
        {
            return FindPic(point1.x, point1.y, point2.x, point2.y, picPath: picPath, sim: Convert.ToSingle(sim));
        }

        public CursorPoint FindPic(int x1, int y1, int x2, int y2, string picPath, double sim)
        {
            if (File.Exists(picPath))
            {
                int? result;
                result = lw?.FindPic(x1, y1, x2, y2, picname: picPath, sim: Convert.ToSingle(sim));
                if (result == 1)
                {
                    return new CursorPoint(lw?.x() ?? -1, lw?.y() ?? -1);
                }
            }
            return new CursorPoint(-1, -1);
        }

        public bool Capture(CursorPoint point1, CursorPoint point2, string picPath)
        {
            return Capture(point1.x, point1.y, point2.x, point2.y, picPath);
        }

        public bool Capture(int x1, int y1, int x2, int y2, string picPath)
        {
            int? result;
            result = lw?.Capture(x1, y1, x2, y2, picPath);
            return result != null && result != 0;
        }


        public byte[] CaptureToBuffer(int x1, int y1, int x2, int y2)
        {
            if (lw == null)
            {
                throw new Exception();
            }
            int length = lw.CaptureToBuffer(x1, y1, x2, y2);
            IntPtr dataPtr = (IntPtr)lw.x();
            byte[] dataBytes = new byte[length];
            Marshal.Copy(dataPtr, dataBytes, 0, length);
            return dataBytes;
        }

        public CursorPoint GetLastLocated()
        {
            return new CursorPoint(lw?.x() ?? -1, lw?.y() ?? -1);
        }

        public void Dispose()
        {
            if (disposed)
            {
                lw = null;
            }
            disposed = true;
        }
    }
}
