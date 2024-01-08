using ErHuo.Models;
using ErHuo.Plugins;
using ErHuo.Utilities;
using HandyControl.Controls;
using HandyControl.Data;
using Newtonsoft.Json.Linq;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ErHuo.Utilities.HwndUtil;
using MessageBox = HandyControl.Controls.MessageBox;

namespace ErHuo.ViewModels
{
    public class NormalKeyConfigViewModel : PropertyChangedBase
    {
        public int Frequency
        {
            get
            {
                return ConfigFactory.GetValue<int>(ConfigKey.Frequency);
            }
            set
            {
                value = Math.Max(value, 20);
                ConfigFactory.SetValue(ConfigKey.Frequency, value);
            }
        }
        public int KeyMode
        {
            get
            {
                return ConfigFactory.GetValue<int>(ConfigKey.KeyMode);
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.KeyMode, value);
            }
        }


        private string _keyAddName;
        public string KeyAddName
        {
            get => _keyAddName;
            set { SetAndNotify(ref _keyAddName, value); }
        }


        private static readonly WindowInfo DEFAULT_WINDOWINFO = new WindowInfo(0, "无（前台模式）");
        public string CurrentWindowTitle
        {
            get
            {
                string _currentWindowTitle = _currentWindow.szWindowName;
                if (CurrentWindow.Equals(DEFAULT_WINDOWINFO))
                {
                    _currentWindowTitle = DEFAULT_WINDOWINFO.szWindowName;
                }
                string title = _currentWindowTitle;
                if (title.Length > 20)
                {
                    title = title.Substring(0, 20);
                    title = title.PadRight(23, '.');
                }
                return title;
            }
        }
        private WindowInfo _currentWindow = DEFAULT_WINDOWINFO;
        public WindowInfo CurrentWindow
        {
            get
            {
                return _currentWindow;
            }
            set
            {
                SetAndNotify(ref _currentWindow, value);
                NotifyOfPropertyChange(nameof(CurrentWindowTitle));
            }
        }


        private bool _waitKey = false;
        public bool WaitKey
        {
            get { return _waitKey; }
            set
            {
                SetAndNotify(ref _waitKey, value);
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
        private IWindowManager _windowManager;
        public NormalKeyConfigViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public bool CheckKey(EKey k)
        {
            if (k == null)
                return false;
            if (k.IsSame(KeyManager.KeyStart) || k.IsSame(KeyManager.KeyStop))
                return false;
            if (_keyList != null && _keyList.Count != 0)
            {
                foreach (var key in _keyList)
                {
                    if (key.IsSame(k))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private List<KeyEvent> InitKeyList()
        {
            List<KeyEvent> _configlist = ConfigFactory.GetValue<List<KeyEvent>>("KeyList");
            foreach (KeyEvent item in _configlist)
            {
                item.PropertyChanged += KeyEventPropertyChanged;
            }
            return _configlist;
        }

        #region event handler
        public void AddKey()
        {
            if (string.IsNullOrEmpty(_keyAddName))
            {
                Growl.Info(new GrowlInfo() { WaitTime = 2, Message = Constant.AddKeyEmptyWarning, ShowDateTime = false });
                return;
            }
            EKey _keyAdd = EKey.GetEKeyFromName(_keyAddName);
            if (CheckKey(_keyAdd))
            {
                _keyList.Add(new KeyEvent(_keyAdd));
            }
            else
            {
                Growl.Info(new GrowlInfo() { WaitTime = 2, Message = Constant.AddKeyFailWarning, ShowDateTime = false });
            }
            KeyAddName = "";
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

        public void ClickItem(string parameter)
        {
            foreach (var key in _keyList)
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
            ConfigFactory.SetValue(ConfigKey.KeyList, new List<KeyEvent>(_keyList));
        }

        public void KeyEventPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ConfigFactory.SetValue(ConfigKey.KeyList, new List<KeyEvent>(_keyList));
        }

        public void FindWindow()
        {
            TopMostViewModel topMostViewModel = Instances.TopMostViewModel;
            if (topMostViewModel.IsRuning())
            {
                Growl.Info(Constant.FindPointUnfinish);
                return;
            }
            bool isFirstInform = ConfigFactory.GetValue<bool>(ConfigKey.IsFirstInformFindWindow);
            if (isFirstInform)
            {
                ConfigFactory.SetValue(ConfigKey.IsFirstInformFindWindow, false);
                MessageBox.Info(Constant.NormalKeyFirstInform);
            }
            P p = new P();
            WaitKey = true;
            Instances.HotKeyViewModel.QueueBusy();
            _windowManager.ShowWindow(topMostViewModel);
            int timeout = ConfigFactory.GetValue<int>(ConfigKey.WaitKeyTimeout);
            topMostViewModel.ShowCursorLocationAndWindowTitle("正在选择窗口", timeout);
            if (p.WaitKey(4, timeout) != -1)
            {
                WindowInfo windowInfo = p.GetMousePointWindow();
                Growl.Info(windowInfo.szWindowName);
                CurrentWindow = windowInfo;
            }
            topMostViewModel.RequestClose();
            WaitKey = false;
            Instances.HotKeyViewModel.DequeueBusy();
            p.Dispose();
        }

        public void ResetWindow()
        {
            CurrentWindow = DEFAULT_WINDOWINFO;
        }

        public void KeyModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KeyMode == 0)
            {
                Growl.Info(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "顺序模式：一次按一个按键，顺序按下。",
                    ShowDateTime = false
                });
            }
            else if (KeyMode == 1)
            {
                Growl.Info(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "连发模式：一次按一组按键，同时按下。",
                    ShowDateTime = false
                });
            }
        }

        #endregion

    }
}
