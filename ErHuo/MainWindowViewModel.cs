using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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
        private string key_fish_release;
        private string key_fish_collect;
        private bool fish_revive;
        private string fish_point1, fish_point2, fish_point3;

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
        private int tab_on;

        public string Key_Fish_Release
        {
            get
            {
                key_fish_release = Enum.GetName(typeof(VK), config.Config_Key_Fish_Release);
                return key_fish_release;
            }
            set
            {
                ConfigUtil.Config.Config_Key_Fish_Release = (int)Enum.Parse(typeof(VK), value);
                OnPropertyChanged();
            }
        }
        public string Key_Fish_Collect
        {
            get
            {
                key_fish_collect = Enum.GetName(typeof(VK), config.Config_Key_Fish_Collect);
                return key_fish_collect;
            }
            set
            {
                ConfigUtil.Config.Config_Key_Fish_Collect = (int)Enum.Parse(typeof(VK), value);
                OnPropertyChanged();
            }
        }

        public bool Fish_Revive
        {
            get
            {
                fish_revive = config.Config_Fish_Revive;
                return fish_revive;
            }
            set
            {
                ConfigUtil.Config.Config_Fish_Revive = value;
                OnPropertyChanged();
            }
        }

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
                if (KeyThread.GetStatus() == STATUS.STOP)
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

        public int TabOn
        {
            get
            {
                return tab_on;
            }
            set
            {
                tab_on = value;
                if(value == 1)
                {
                    Fish_Point1 = PointToString(Config.Config_Fish_Point1);
                    Fish_Point2 = PointToString(Config.Config_Fish_Point2);
                    Fish_Point3 = PointToString(Config.Config_Fish_Point3);
                }
                OnPropertyChanged();
            }
        }

        public string Fish_Point1
        {
            get
            {
                return fish_point1;
            }
            set
            {
                fish_point1 = value;
                OnPropertyChanged();
            }
        }

        public string Fish_Point2
        {
            get
            {
                return fish_point2;
            }
            set
            {
                fish_point2 = value;
                OnPropertyChanged();
            }
        }
        public string Fish_Point3
        {
            get
            {
                return fish_point3;
            }
            set
            {
                fish_point3 = value;
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
        public DelegateCommand UpdateAPPList_Command { get; set; }
        public DelegateCommand Get_Point_Command { get; set; }
        public DelegateCommand Inform_Command { get; set; }

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
            Load_KeyControl_Command = new DelegateCommand();
            Load_KeyControl_Command.ExecuteAction = new Action<object>(this.UserControl_Loaded);

            Window_Closing_Command = new DelegateCommand();
            Window_Loaded_Command = new DelegateCommand();
            AddKey_Command = new DelegateCommand();
            DeleteKey_Command = new DelegateCommand();
            ClickItem_Command = new DelegateCommand();
            Start_Command = new DelegateCommand();
            Stop_Command = new DelegateCommand();
            UninstallDriver_Command = new DelegateCommand();
            SwitchDriver_Command = new DelegateCommand();
            UpdateAPPList_Command = new DelegateCommand();
            Get_Point_Command = new DelegateCommand();
            Inform_Command = new DelegateCommand();
            Window_Closing_Command.ExecuteActionWithoutParameter = new Action(this.Closing);
            Stop_Command.ExecuteActionWithoutParameter = new Action(()=> { Status = false; });
            Start_Command.ExecuteActionWithoutParameter = new Action(() => { Status = true; });
            ClickItem_Command.ExecuteActionStringParameter = new Action<string>(this.ClickItem);
            DeleteKey_Command.ExecuteActionStringParameter = new Action<string>(this.DeleteKey);
            AddKey_Command.ExecuteActionWithoutParameter = new Action(this.AddKey);
            Window_Loaded_Command.ExecuteActionWithoutParameter = new Action(this.Window_Loaded);
            UninstallDriver_Command.ExecuteActionWithoutParameter = new Action(this.Uninstall);
            SwitchDriver_Command.ExecuteActionWithoutParameter = new Action(this.SwitchDriver);
            UpdateAPPList_Command.ExecuteActionWithoutParameter = new Action(this.UpdatAPPList);
            Get_Point_Command.ExecuteActionStringParameter = new Action<string>(this.GetPoint);
            Inform_Command.ExecuteActionWithoutParameter = new Action(this.InformWindow);
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
            UpdatAPPList();
            RegFunction((int)DriveCode.lw);
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
                    MessageBox.Show("卸载Lw驱动失败 或 并未安装驱动,请重启再次卸载驱动; CanUnload:" + LwKey.CanUnload().ToString());
                }
            }
  
        }

        public void Closing()
        {
            KeyThread.Exit();
            if (LwKey.CanUnload() && LwKey.UnRegister() >= 0)
            {
                MessageBox.Show("Lw卸载驱动成功,请手动删除C:\\Windows\\System32\\drivers下的DrvInDKB.sys 与DrvInDMU.sys 以免受到木马攻击");
            }
            else
            {
                MessageBox.Show("卸载Lw驱动失败 或 并未安装驱动,请重启再次卸载驱动; CanUnload:" + LwKey.CanUnload().ToString());
            }
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
            if (KeyThread.GetStatus() != STATUS.START) {
                switch (tab_on)
                {
                    case 0:
                        KeyThread.SetKeyList(keylist.ToList());
                        break;
                    case 1: break;
                }
                KeyThread.Init(window_obj_list[window_index].hWnd, tab_on, frequency);
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
            if (KeyThread.GetStatus() != STATUS.STOP) {
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
                            if (KeyThread.GetStatus() != STATUS.STOP)
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
                        if (KeyThread.GetStatus() == STATUS.PAUSE)
                        {
                            KeyThread.Start();
                            System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                PlayStartRequested?.Invoke(this, EventArgs.Empty);
                            });
                        }
                        else if (KeyThread.GetStatus() == STATUS.START)
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
            if(keylist != null&&keylist.Count != 0)
            {
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
            }
            return true;
        }

        private void UpdatAPPList()
        {
            try { 
                windowlist.Clear();
                window_obj_list = HwndUtil.GetAllDesktopWindows();
                foreach (HwndUtil.WindowInfo win in window_obj_list)
                {
                    windowlist.Add(win.szWindowName);
                }
                WindowIndex = 0;
            }
            catch
            {
                MessageBox.Show("获取窗口列表失败，可能被杀毒软件拦截，后台按键功能将不可用。");
            }
        }

        public void ChangeTargetWindow(int index)
        {
            if(index != 0 && index != -1)
            {
                MessageBox.Show("请勿用于端游！不一定有效，且有封号风险！");
            }
        }

        public void GetPoint(string info)
        {
            switch (info)
            {
                case "Point1":
                    new Thread(()=> {
                        if (Tool.isProcess()) {
                            MessageBox.Show("正在进行另一项定点，请先按一次鼠标中键。");
                            return;
                        }
                        Config.Config_Fish_Point1 = Tool.GetPoint();
                        Fish_Point1 = PointToString(Config.Config_Fish_Point1);
                    }).Start();
                    break;
                case "Point2":
                    new Thread(() => {
                        if (Tool.isProcess())
                        {
                            MessageBox.Show("正在进行另一项定点，请先按一次鼠标中键。");
                            return;
                        }
                        Config.Config_Fish_Point2 = Tool.GetPoint();
                        Fish_Point2 = PointToString(Config.Config_Fish_Point2);
                    }).Start();
                    break;
                case "Point3":
                    new Thread(() => {
                        if (Tool.isProcess())
                        {
                            MessageBox.Show("正在进行另一项定点，请先按一次鼠标中键。");
                            return;
                        }
                        Config.Config_Fish_Point3 = Tool.GetPoint();
                        Fish_Point3 = PointToString(Config.Config_Fish_Point3);
                    }).Start();
                    break;
            }
        }

        private string PointToString(Point p)
        {
            return p.X + "," + p.Y;
        }

        private void InformWindow()
        {
            Infrom iw = new Infrom();
            iw.Show();
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
