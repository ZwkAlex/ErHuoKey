using ErHuo.Models;
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
            }
        }

        private int _volume = ConfigFactory.GetValue<int>(ConfigKey.Volume);
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
                return ConfigFactory.GetValue<bool>(ConfigKey.MinimizeToTray);
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
                return ConfigFactory.GetValue<int>(ConfigKey.WaitKeyTimeout) / 1000;
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
                bool enable = ConfigFactory.GetValue<bool>(ConfigKey.DarkTheme);
                Tool.EnableDarkTheme(enable);
                return enable;
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.DarkTheme, value);
            }
        }


        private bool _isReg;

        public bool IsReg
        {
            get { return _isReg; } 
            set { 
                SetAndNotify(ref _isReg, value); 
            }
        }

        private static IContainer _container;
        public ConfigDrawerViewModel(IContainer container)
        {
            _container = container;
            RegisterState.Instance.RegisterStateChanged += RegisterChangeHandler;
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
            IRegister.Instance.TryUnRegister();
        }

        public void RegisterPlugin()
        {
            IRegister.Instance.TryRegister();
        }

        public void ResetConfig()
        {
            ConfigFactory.TryClear();
        }

        private void RegisterChangeHandler(object sender, bool newRegValue)
        {
            IsReg = newRegValue;
            if (newRegValue)
            {
                Instances.NoClientAreaViewModel.ConfigBgColor = Brushes.Transparent;
            }
            else
            {
                Instances.NoClientAreaViewModel.ConfigBgColor = Brushes.Red;
            }
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
