using ErHuo.Plugins;
using ErHuo.Utilities;
using ErHuo.ViewModels;
using Stylet;
using System;
using System.Threading;
using System.Windows;

namespace ErHuo
{
    public class Bootstrapper : Bootstrapper<MainViewModel>
    {
        private static Mutex mutex;
        protected override void OnStart()
        {
            mutex = new Mutex(true, "ErHuoService");
            if (!mutex.WaitOne(0, false))
            {
                MessageBox.Show("程序已经在运行！", "提示");
                throw new Exception();
            }
            ConfigFactory.LoadConfigFile();
            Tool.EnableDarkTheme(ConfigFactory.GetValue<bool>(ConfigKey.DarkTheme));
        }
        protected override void Configure()
        {
            base.Configure();
            Instances.Instantiate(Container);
        }

    }
}
