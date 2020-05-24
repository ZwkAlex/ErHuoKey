using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using SlaveClient;

namespace ErHuo
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public KeyboardHook keyboardHook;
        private ConfigSet config = ConfigUtil.Config;
        private string key_add_text;

        private string key_start;
        private string key_stop;
        private string key_pause;
        private bool is_use_same;
        private double volume;
        private int frequency;
        private bool _switch;
        private int key_mode;
        private string prompt_visibility;
        private int window_index;
        private HwndUtil.WindowInfo[] window_obj_list;

        public string Key_Start
        {
            get
            {
                key_start = Enum.GetName(typeof(VK), config.Config_Key_Start);
                return key_start;
            }
            set
            {
                ConfigUtil.Config.Config_Key_Start = (int)Enum.Parse(typeof(VK), value);
                OnPropertyChanged();
            }
        }
        public string Key_Stop
        {
            get
            {
                key_stop = Enum.GetName(typeof(VK), config.Config_Key_Stop);
                return key_stop;
            }
            set
            {
                ConfigUtil.Config.Config_Key_Stop = (int)Enum.Parse(typeof(VK), value);
                OnPropertyChanged();
            }
        }
        public string Key_Pause
        {
            get
            {
                key_pause = Enum.GetName(typeof(VK), config.Config_Key_Pause);
                return key_pause;
            }
            set
            {
                ConfigUtil.Config.Config_Key_Pause = (int)Enum.Parse(typeof(VK), value);
                OnPropertyChanged();
            }
        }
        public bool Is_Use_Same
        {
            get
            {
                is_use_same = config.Config_is_Use_Same_Key;
                return is_use_same;
            }
            set
            {
                ConfigUtil.Config.Config_is_Use_Same_Key = value;
                OnPropertyChanged();
            }
        }
        public double Volume
        {
            get
            {
                volume = config.Config_Volume;
                return volume;
            }
            set
            {
                ConfigUtil.Config.Config_Volume = Math.Round(value, 1);
                OnPropertyChanged();
            }
        }
        public bool Switch
        {
            get
            {
                _switch = config.Config_Switch;
                return _switch;
            }
            set
            {
                if (keyboardHook != null)
                {
                    if (value)
                    {
                        keyboardHook.Start();
                    }
                    else
                    {
                        keyboardHook.Stop();
                    }
                }
                ConfigUtil.Config.Config_Switch = value;
                OnPropertyChanged();
            }
        }
        public int Frequency
        {
            get
            {
                frequency = config.Config_Frequency;
                return frequency;
            }
            set
            {
                ConfigUtil.Config.Config_Frequency = value;
                OnPropertyChanged();
            }
        }
        public bool Status
        {
            get
            {
                if (KeyThread.GetStatus() == 2)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
                OnPropertyChanged();
            }
        }

        public int Key_Mode
        {
            get {
                key_mode = config.Config_Key_Mode;
                if (key_mode == 1)
                {
                    PromptVisibility = "Visible";
                }
                else
                {
                    PromptVisibility = "Hidden";
                }
                return key_mode;
            }
            set
            {
                ConfigUtil.Config.Config_Key_Mode = value;
                if (value == 1)
                {
                    MessageBox.Show("按压模式下，暂停键、结束键设置都将无效，按住《开始键》即连点按键，松开即结束");

                }
                OnPropertyChanged();
            }
        }

        public string PromptVisibility
        {
            get
            {
                return prompt_visibility;
            }
            set
            {
                prompt_visibility = value;
                OnPropertyChanged();
            }
        }

        public int WindowIndex
        {
            get
            {
                return window_index;
            }
            set
            {
                window_index = value;
                ChangeTargetWindow(value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> windowlist { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<KeyEvent> keylist { get; set; } = ConfigUtil.Config.Config_Key_List;
        public string Key_Add_Text { get { return key_add_text; } set { key_add_text = value; OnPropertyChanged(); } }
        public ConfigSet Config{ get{ return config;} }
        public DelegateCommand Window_Loaded_Command { get; set; }
        public DelegateCommand Start_Command { get; set; }
        public DelegateCommand Stop_Command { get; set; }
        public DelegateCommand AddKey_Command { get; set; }
        public DelegateCommand DeleteKey_Command { get; set; }
        public DelegateCommand ClickItem_Command { get; set; }
        public DelegateCommand Load_KeyControl_Command { get; set; }
        public DelegateCommand Window_Closing_Command { get; set; }
        public DelegateCommand UninstallDriver_Command { get; set; }
        public DelegateCommand SwitchDriver_Command { get; set; }
        public DelegateCommand SelectWindow_Command { get; set; }

        public event EventHandler PlayStartRequested;
        public event EventHandler PlayStopRequested;
        public event EventHandler PlayPauseRequested;
        public event EventHandler MinWindowRequested;
        public event KeyEventHandler KeyUpRequested;
        public event KeyEventHandler KeyDownRequested;


        public MainWindowViewModel()
        {
            if (config.Config_FristTime)
            {
                MessageBox.Show("本按键不含任何恶意联网、游戏注入功能，请添加防火墙/杀毒软件信任\n如加载驱动失败，请关闭本按键并用管理员身份开启");
                ConfigUtil.Config.Config_FristTime = false;
            }
            this.Load_KeyControl_Command = new DelegateCommand();
            this.Load_KeyControl_Command.ExecuteAction = new Action<object>(this.UserControl_Loaded);

            this.Window_Closing_Command = new DelegateCommand();
            this.Window_Loaded_Command = new DelegateCommand();
            this.AddKey_Command = new DelegateCommand();
            this.DeleteKey_Command = new DelegateCommand();
            this.ClickItem_Command = new DelegateCommand();
            this.Start_Command = new DelegateCommand();
            this.Stop_Command = new DelegateCommand();
            this.UninstallDriver_Command = new DelegateCommand();
            this.SwitchDriver_Command = new DelegateCommand();
            this.Window_Closing_Command.ExecuteActionWithoutParameter = new Action(this.Closing);
            this.Stop_Command.ExecuteActionWithoutParameter = new Action(()=> { Status = false; });
            this.Start_Command.ExecuteActionWithoutParameter = new Action(() => { Status = true; });
            this.ClickItem_Command.ExecuteActionStringParameter = new Action<string>(this.ClickItem);
            this.DeleteKey_Command.ExecuteActionStringParameter = new Action<string>(this.DeleteKey);
            this.AddKey_Command.ExecuteActionWithoutParameter = new Action(this.AddKey);
            this.Window_Loaded_Command.ExecuteActionWithoutParameter = new Action(this.Window_Loaded);
            this.UninstallDriver_Command.ExecuteActionWithoutParameter = new Action(this.Uninstall);
            this.SwitchDriver_Command.ExecuteActionWithoutParameter = new Action(this.SwitchDriver);
        }
       
        private void UserControl_Loaded(object parameter)
        {

        }

        private void Window_Loaded()
        {
            keyboardHook = new KeyboardHook();
            keyboardHook.KeyDownEvent += new KeyEventHandler(KeyDownRequested);
            keyboardHook.KeyUpEvent += new KeyEventHandler(KeyUpRequested);
            if (_switch)
            {
                keyboardHook.Start();
            }
            UpdateWindow();
            KeyThread.SetThread(keylist.ToList(), frequency);
            RegFunction((int)DriveCode.lw);
            if(window_obj_list[window_index].hWnd == IntPtr.Zero)
            {
                KeyThread.Init((int)DriveCode.lw);
            }
            else
            {
                KeyThread.Init((int)DriveCode.lw);
            }
        }

        private void RegFunction(int driver)
        {
            int regResult=-1;
            switch (driver)
            {
                case (int)DriveCode.lw:
                    {
                        regResult = LwKey.Register();
                        break;
                    }
                case (int)DriveCode.ts:
                    {
                        regResult = TsKey.Register();
                        break;
                    }
            }
            if (regResult < 0)
            {
                MessageBox.Show("注册驱动失败 code：" + regResult.ToString() + "，请重启软件");
            }
        }

        public void Uninstall()
        {
            if (MessageBox.Show("确认卸载驱动?卸载驱动将导致按键不可用", "确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (LwKey.CanUnload()&& LwKey.UnRegister() >= 0)
                {
                    MessageBox.Show("Lw卸载驱动成功,请手动删除C:\\Windows\\System32\\drivers下的DrvInDKB.sys 与DrvInDMU.sys 以免受到木马攻击");
                }
                else
                {
                    MessageBox.Show("卸载Lw驱动失败 或 并未安装驱动,请重启再次卸载驱动; CanUnload:" + TsKey.CanUnload().ToString());
                }
              
                if (TsKey.CanUnload()&& TsKey.UnRegister() >= 0)
                {
                    MessageBox.Show("Ts卸载驱动成功");
                }
                else
                {
                    MessageBox.Show("卸载Ts驱动失败 或 并未安装驱动,请重启再次卸载驱动; CanUnload:" + TsKey.CanUnload().ToString());
                }
                
               
            }
  
        }

        public void Closing()
        {
            KeyThread.Exit();
            Environment.Exit(0);
        }

        private void ClickItem(string parameter)
        {
            foreach(var key in keylist)
            {
                if (key.Key.Equals(parameter))
                {
                    if (key.Activate)
                    {
                        key.Activate = false;
                    }
                    else
                    {
                        key.Activate = true;
                    }
                }
            }
        }
        private void DeleteKey(string parameter)
        {
            
            foreach (var key in keylist)
            {
                if (key.Key.Equals(parameter))
                {
                    keylist.Remove(key);
                    break;
                }
            }
        }

        private void AddKey()
        {
            try
            {
                if (CheckKey(key_add_text))
                {
                    keylist.Add(
                        new KeyEvent
                        {
                            Activate = true,
                            Code = (int)Enum.Parse(typeof(VK), key_add_text),
                            Key = key_add_text
                        }
                    );
                }
                else
                {
                    MessageBox.Show("这个按键已经存在 或 你压根没输入按键");
                }
                Key_Add_Text = "";
            }
            catch
            {
                MessageBox.Show("key-isactivate 匹配发生错误");
            }
        }

        public void Start()
        {
            if (KeyThread.GetStatus() != KeyThread.START) {
                KeyThread.SetThread(keylist.ToList(),frequency);
                KeyThread.Start();
                System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PlayStartRequested?.Invoke(this, EventArgs.Empty);
                    MinWindowRequested?.Invoke(this, EventArgs.Empty);
                });
            }
        }
        
        public void Stop()
        {
            if (KeyThread.GetStatus() != KeyThread.STOP) {
                KeyThread.Stop();
                System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PlayStopRequested?.Invoke(this, EventArgs.Empty);
                });
            }
        }

        
        public void HandleKeyDown(int KeyCode)
        {
            switch (key_mode)
            {
                case 0:
                    if (KeyCode == config.Config_Key_Start)
                    {
                        if (!is_use_same)
                        {
                            Status = true;
                        }
                        else
                        {
                            if (KeyThread.GetStatus() != KeyThread.STOP)
                            {
                                Status = false;
                            }
                            else
                            {
                                Status = true;
                            }
                        }
                    }
                    else if (KeyCode == config.Config_Key_Stop)
                    {
                        if (!is_use_same)
                        {
                            Status = false;
                        }
                    }
                    else if (KeyCode == config.Config_Key_Pause)
                    {
                        if (KeyThread.GetStatus() == KeyThread.PAUSE)
                        {
                            KeyThread.Start();
                            System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                PlayStartRequested?.Invoke(this, EventArgs.Empty);
                            });
                        }
                        else if (KeyThread.GetStatus() == KeyThread.START)
                        {
                            KeyThread.Pause();
                            System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                PlayPauseRequested?.Invoke(this, EventArgs.Empty);
                            });
                        }
                    }
                    break;
                case 1:
                    if (KeyCode == config.Config_Key_Start)
                    {
                        Status = true;
                    }
                    break;
            }
            
        }

        public void HandleKeyUp(int KeyCode)
        {
            if(key_mode == 1&& KeyCode == config.Config_Key_Start)
            {
                Status = false;
            }
        }

        public void SwitchDriver()
        {
            MessageBox.Show("暂只有Lw插件驱动");
        }

        public bool CheckKey(string k)
        {
            if(keylist == null)
            {
                return true;
            }
            if (k == null || k.Equals(""))
                return false;
            if (k == key_pause || k == key_start || k == key_stop)
                return false;
            foreach (var key in keylist)
            {
                if (key.Key == k)
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateWindow()
        {
            windowlist.Clear();
            window_obj_list = HwndUtil.GetAllDesktopWindows();
            foreach (HwndUtil.WindowInfo win in window_obj_list)
            {
                windowlist.Add(win.szWindowName);
            }
            WindowIndex = 0;
        }

        public void ChangeTargetWindow(int index)
        {
            if(index != 0)
            {
                MessageBox.Show("请勿用于端游！不一定有效，且有封号风险！");
            }
            KeyThread.SetWindowHwnd(window_obj_list[index].hWnd);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]String property = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
