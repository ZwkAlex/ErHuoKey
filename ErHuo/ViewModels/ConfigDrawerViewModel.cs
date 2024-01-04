using ErHuo.Plugins;
using ErHuo.Utilities;
using HandyControl.Themes;
using Newtonsoft.Json.Linq;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace ErHuo.ViewModels
{
    public class ConfigDrawerViewModel : PropertyChangedBase
    {
        private bool _drawerSwitch = false;
        public bool DrawerSwitch
        {
            get { return _drawerSwitch; }
            set
            {
                SetAndNotify(ref _drawerSwitch, value);
                if (value)
                {
                    RegisterState = RegisterBase.Instance.GetRegisterStatus();
                }
            }
        }

        private int _volume = ConfigFactory.GetValue(ConfigKey.Volume, 20);
        public int Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                SetAndNotify(ref _volume, value);
                SetVolume(value);
            }
        }

        public bool MinimizeToTray
        {
            get
            {
                return ConfigFactory.GetValue(ConfigKey.MinimizeToTray, false);
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.MinimizeToTray, value);
            }
        }


        public int WaitKeyTimeout
        {
            get
            {
                return ConfigFactory.GetValue(ConfigKey.WaitKeyTimeout, 30000) / 1000;
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.WaitKeyTimeout, value * 1000);
            }
        }


        public bool DarkTheme
        {
            get
            {
                bool enable = ConfigFactory.GetValue(ConfigKey.DarkTheme, false);
                Tool.EnableDarkTheme(enable);
                return enable;
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.DarkTheme, value);
            }
        }


        private bool _registerState;

        public bool RegisterState
        {
            get { return _registerState; } 
            set { SetAndNotify(ref _registerState, value); }
        }

        private static IContainer _container;
        public ConfigDrawerViewModel(IContainer container)
        {
            _container = container;
        }

        public void SwitchDrawer()
        {
            if (_drawerSwitch)
            {
                Off();
            }
            else
            {
                On();
            }
        }

        public void SetVolume(int _volume)
        {
            SoundPlayUtil.ChangeVolume(_volume);
            ConfigFactory.SetValue(ConfigKey.Volume, _volume);
        }

        public void UnregisterPluginAndDelete()
        {
            RegisterBase.Instance.TryUnRegister();
            RegisterState = RegisterBase.Instance.GetRegisterStatus();
        }

        public void On()
        {
            DrawerSwitch = true;
        }

        public void Off()
        {
            DrawerSwitch = false;
        }
    }
}
