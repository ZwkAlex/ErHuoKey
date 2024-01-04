using ErHuo.Models;
using ErHuo.Plugins;
using System.Threading;
using System.Windows;

namespace ErHuo.Services
{
    public abstract class IService
    {
        public ConfigSheet config;
        public P p;
        public CancellationToken Token;

        public IService(ConfigSheet config, CancellationToken Token)
        {
            this.config = config;
            this.Token = Token;
            p = new P();
        }
        ~IService() => p.Dispose();

        private bool Bind_Window(int hwnd, string display = "normal", string mouse = "normal", string keyboard = "normal", int mode = 1)
        {
            p.UnBindWindow();
            if (hwnd != 0)
            {
                return p.BindWindow(hwnd, display, mouse, keyboard, mode);
            }
            else
            {
                return p.BindWindow(-1);
            }
        }
        public void StartService()
        {
            if (Token == null || !Token.CanBeCanceled)
            {
                throw new WindowBindingException("意外情况：未正确设置CancelToken");
            }
            bool bind_result = Bind_Window(config.WindowInfo.hWnd, config.Display, config.Mouse, config.Keyboard, config.Mode);
            if (!bind_result)
            {
                throw new WindowBindingException("绑定窗口：" + config.WindowInfo.szWindowName + " 失败");
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            });
            Service();
        }
        public void StopService()
        {
            p.UnBindWindow();
            //Application.Current.Dispatcher.Invoke(() => {
            //    Application.Current.MainWindow.Show();
            //    Application.Current.MainWindow.WindowState = WindowState.Normal;
            //});
        }

        public abstract void Service();
    }
}
