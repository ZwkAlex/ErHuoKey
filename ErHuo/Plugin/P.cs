using ErHuo.Models;
using lw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ErHuo.Plugin
{
    public class P
    {
        private static readonly Dictionary<string, int> LWKEYMODE = new Dictionary<string, int>() { {"normal", 0}, {"windows", 1}};
        private lwsoft lw = new lwsoft();
        //private static readonly IOpinterface op = new IOpinterface();

        public P()
        {
            RegisterBase register = new LwRegister();
            register.TryRegister();
        }

        public void Init()
        {
            this.lw = new lwsoft();
        }

        public int BindWindow(int hwnd, string dispaly="normal", string mouse="windows", string keyboard="windows", int mode=0)
        {
            int result;
            if (hwnd == -1)
            {
                 result = lw.BindWindow(hwnd);
            }
            else
            {
                 result = lw.BindWindow(hwnd, LWKEYMODE[dispaly], LWKEYMODE[mouse], LWKEYMODE[keyboard]);
            }
            
            return result;
        }

        public int UnBindWindow()
        {
            int result = lw.UnBindWindow();
            return result;
        }

        public int KeyPress(int key)
        {
            int result = lw.KeyPress(key);
            return result;
        }

        public WindowInfo GetMousePointWindow()
        {
            int hwnd = lw.GetMousePointWindow();
            string windowTitle = lw.GetWindowTitle(hwnd);
            return new WindowInfo(hwnd, windowTitle);
        }

        public int WaitKey(int key, int time_out)
        {
            int result = lw.WaitKey(key, time_out);
            return result;
        }

    }
}
