using ErHuo.Utilities;
using ErHuo.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace ErHuo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public KeyboardHook keyboardHook;
        private NotifyIcon _notifyIcon = null;
        //private MainWindowViewModel vm;
        public MainWindow()
        {
            ReleaseDLL(Properties.Resources.op_x64, "op_x64.dll");
            InitializeComponent();
            //vm = new MainWindowViewModel();
            //this.DataContext = vm;
            //vm.PlayStartRequested += PlayStartSound;
            //vm.PlayStopRequested += PlayStopSound;
            //vm.PlayPauseRequested += PlayPauseSound;
            //vm.KeyDownRequested += KeyDownHandler;
            //vm.KeyUpRequested += KeyUpHandler;
            //vm.MinWindowRequested += HideWindow;

            InitIcon();
        }
        void ReleaseDLL(byte[] byDll, string name)
        {
            string strPath = System.AppDomain.CurrentDomain.BaseDirectory + name;//设置释放路径
            //创建dll文件（覆盖模式）
            using (FileStream fs = new FileStream(strPath, FileMode.Create))
            {
                fs.Write(byDll, 0, byDll.Length);
            }
        }

        private void InitIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.BalloonTipTitle = "贰货按键";
            _notifyIcon.BalloonTipText = "已隐藏至任务栏";
            _notifyIcon.Text = "ErHuo按键";
            _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);//当前程序图标

            MenuItem open = new MenuItem("打开");
            open.Click += new EventHandler(ShowWindow);
            MenuItem exit = new MenuItem("退出");
            exit.Click += new EventHandler(Close);
            MenuItem[] childen = new MenuItem[] { open, exit };
            _notifyIcon.ContextMenu = new ContextMenu(childen);

            _notifyIcon.MouseClick += new MouseEventHandler((sender, e) => { ShowWindow(sender, e); });
        }

        private void ShowWindow(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private void HideWindow(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }


        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                _notifyIcon.ShowBalloonTip(1000);
                _notifyIcon.Visible = true;
            }
            if (WindowState == WindowState.Maximized || WindowState == WindowState.Normal)
            {
                _notifyIcon.Visible = false;
                Activate();
            }
        }

        //private void KeyUpHandler(object sender, KeyEventArgs e)
        //{
        //    vm.HandleKeyUp(e.KeyValue);
        //}
        //private void KeyDownHandler(object sender, KeyEventArgs e)
        //{
        //    vm.HandleKeyDown(e.KeyValue);
        //}
        //private void PlayStartSound(object sender, EventArgs e)
        //{
        //    startsound.Stop();
        //    startsound.Play();
        //}

        //private void PlayStopSound(object sender, EventArgs e)
        //{
        //    stopsound.Stop();
        //    stopsound.Play();
        //}

        //private void PlayPauseSound(object sender, EventArgs e)
        //{
        //    pausesound.Stop();
        //    pausesound.Play();
        //}
    }
}
