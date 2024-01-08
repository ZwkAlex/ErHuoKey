using ErHuo.Utilities;
using Newtonsoft.Json.Linq;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ErHuo.ViewModels
{
    public class NoClientAreaViewModel : PropertyChangedBase
    {
        public string VersionDescription
        {
            get
            {
                return Constant.Version;
            }
        }

        private static SolidColorBrush _configDefaultBrush = Brushes.Transparent;

        private SolidColorBrush _configColor = _configDefaultBrush;
        public SolidColorBrush ConfigColor
        {
            get => _configColor;
            set
            {
                SetAndNotify(ref _configColor, value);
            }
        }

        public SolidColorBrush _configBgColor = Brushes.Transparent;

        public SolidColorBrush ConfigBgColor
        {
            get => _configBgColor;
            set
            {
                SetAndNotify(ref _configBgColor, value);
            }
        }

        private IContainer _container;
        private CancellationTokenSource cts;
        public NoClientAreaViewModel(IContainer container)
        {
            _container = container;
            if (ConfigFactory.GetValue<bool>(ConfigKey.ConfigNeverOpen))
            {
                cts = new CancellationTokenSource();
                Task.Run(() =>
                {
                    NotifyColor();
                }, cts.Token);
            }
        }

        public void OpenDrawer()
        {
            Instances.ConfigDrawerViewModel.SwitchDrawer();
            ConfigFactory.SetValue(ConfigKey.ConfigNeverOpen, false);
            try
            {
                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                }
            }
            catch (ObjectDisposedException)
            {

            }
        }

        public void NotifyColor()
        {
            int count = 0;
            bool flag = false;
            try
            {
                while (count < 50 && !cts.Token.IsCancellationRequested)
                {
                    if (flag)
                    {
                        ConfigColor = Brushes.Coral;
                        flag = false;
                    }
                    else
                    {
                        ConfigColor = _configDefaultBrush;
                        flag = true;
                    }
                    Thread.Sleep(600);
                    count++;
                }
            }
            catch (ObjectDisposedException)
            {

            }
            ConfigColor = _configDefaultBrush;
        }
    }
}
