using ErHuo.Models;
using ErHuo.Plugin;
using ErHuo.Utilities;
using HandyControl.Controls;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ErHuo.Utilities.HwndUtil;

namespace ErHuo.ViewModels
{
    public class NormalKeyConfigViewModel : PropertyChangedBase
    {
        public int Frequency
        {
            get
            {
                return ConfigFactory.GetValue("Frequency", 50);
            }
            set
            {
                ConfigFactory.SetValue("Frequency", value);
            }
        }
        public int KeyMode
        {
            get
            {
                return ConfigFactory.GetValue("KeyMode", 0);
            }
            set
            {
                ConfigFactory.SetValue("KeyMode", value);
            }
        }

        public double Volume
        {
            get
            {
                return ConfigFactory.GetValue("Volume", 0.2);
            }
            set
            {
                ConfigFactory.SetValue("Volume", value);
            }
        }

        private string _keyadd = String.Empty;
        public string KeyAdd
        {
            get => _keyadd;
            set
            {
                if (_keyadd != value)
                {
                    SetAndNotify(ref _keyadd, value);
                }
            }

        }

        public WindowInfo CurrentSelectedWindow
        {
            get
            {
                if(windowInfoList != null && _windowIndex >= 0 && _windowIndex < windowInfoList.Count) {
                    return windowInfoList[_windowIndex];
                }
                return new WindowInfo(0, "无");
            }
        }
        private List<WindowInfo> windowInfoList;
        private List<string> windowList;
        public List<string> WindowList
        {
            get
            {
                if (windowList == null)
                {
                    windowList = GetAPPList();
                }
                return windowList;
            }
            set
            {
                SetAndNotify(ref windowList, value);
            }
        }
        private int _windowIndex = 0;
        public int WindowIndex
        {
            get { return _windowIndex; }
            set
            {
                if (_windowIndex != value)
                {
                    SetAndNotify(ref _windowIndex, value);
                }
            }
        }

        public ObservableCollection<KeyEvent> _keyList;

        public ObservableCollection<KeyEvent> KeyList
        {
            get
            {
                if (_keyList == null)
                {
                    _keyList = new ObservableCollection<KeyEvent>(InitKeyList());
                    _keyList.CollectionChanged += ContentCollectionChanged;
                }
                return _keyList;
            }
            set
            {
                SetAndNotify(ref _keyList, value);
            }
        }

        public List<string> GetAPPList()
        {
            List<string> _windowList = new List<string>();
            try
            {
                windowInfoList = GetAllDesktopWindows();
                foreach (WindowInfo win in windowInfoList)
                {
                    _windowList.Add(win.szWindowName);
                }
            }
            catch
            {
                Growl.Info("获取窗口列表失败，可能被杀毒软件拦截，后台按键功能将不可用。");
            }
            return _windowList;
        }
        public void UpdateAPPList()
        {
            WindowList = GetAPPList();
            WindowIndex = 0;
        }    

        public bool CheckKey(string k)
        {
            if (k == null || k.Equals(""))
                return false;
            if (k == KeyManager.KeyStart || k == KeyManager.KeyStop)
                return false;
            if (_keyList != null && _keyList.Count != 0)
            {
                foreach (var key in _keyList)
                {
                    if (key.Key == k)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private List<KeyEvent> InitKeyList()
        {
            List<KeyEvent> _configlist = ConfigFactory.GetListValue<KeyEvent>("KeyList");
            foreach (KeyEvent item in _configlist)
            {
                item.PropertyChanged += KeyEventPropertyChanged;
            }
            return _configlist;
        }

        #region event handler
        public void AddKey()
        {
            try
            {
                if (CheckKey(KeyAdd))
                {
                    KeyEvent newKey = new KeyEvent(KeyAdd);
                    _keyList.Add(newKey);
                }
                else
                {
                    Growl.Info("按键添加失败：检查是否与开启按键与结束按键冲突");
                }
                KeyAdd = "";
            }
            catch
            {
                Growl.Info("key-isactivate 匹配发生错误");
            }
        }

        public void DeleteKey(string keydelete)
        {
            foreach (var key in _keyList)
            {
                if (key.Key.Equals(keydelete))
                {
                    _keyList.Remove(key);
                    break;
                }
            }
        }
        public void WindowSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Growl.Info(CurrentSelectedWindow.szWindowName.ToString());
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (KeyEvent item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= KeyEventPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (KeyEvent item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += KeyEventPropertyChanged;
                }
            }
            ConfigFactory.SetValue("KeyList", new List<KeyEvent>(_keyList));
        }

        public void KeyEventPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ConfigFactory.SetValue("KeyList", new List<KeyEvent>(_keyList));
        }

        public void FindWindow()
        {
            P p = Instances.P;

            if(p.WaitKey(4, 30000) != -1)
            {
                WindowInfo windowInfo = p.GetMousePointWindow();
                Growl.Info(windowInfo.szWindowName);
                CurrentSelectedWindow = windowInfo;
            }
        }

        public void MouseUp(object sender, MouseEventArgs e)
        {
            CursorPoint c = CursorUtil.doGetCursorPos();
            Growl.Info("keyup" + c.x + "," + c.y);
        }


        #endregion

    }
}
