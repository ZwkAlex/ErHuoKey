using ErHuo.Models;
using ErHuo.Plugin;
using ErHuo.Utilities;
using HandyControl.Controls;
using lw;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ErHuo.Service
{
    public abstract class IService
    {
        public ConfigSheet config; 
        public static P p = Instances.P;
        public CancellationToken Token;
        private bool Bind_Window(int hwnd, string display="normal", string mouse="normal", string keyboard="normal", int mode=1)
        {
            p.Init();
            p.UnBindWindow(); 
            if (hwnd != 0)
            {
                int bind_result = p.BindWindow(hwnd, display, mouse, keyboard, mode);
                return bind_result == 1;
            }
            else
            {
                p.BindWindow(-1);
            }
            return true;

        }
        public void StartService()
        {
            if(Token == null || !Token.CanBeCanceled)
            {
                Token = new CancellationToken();
            }
            bool bind_result = Bind_Window((int)config.WindowInfo.hWnd, config.Display, config.Mouse, config.Keyboard, config.Mode);
            if (!bind_result)
            {
                throw new WindowBindingException("绑定窗口：" + config.WindowInfo.szWindowName + " 失败");
            }
            Service();
        }
        public void StopService()
        {
        }

        public void SetCancellationToken(CancellationToken Token)
        {
            this.Token = Token;
        }

        public abstract void Service();
    }
}
