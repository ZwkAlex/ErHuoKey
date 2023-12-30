using ErHuo.Plugins;
using ErHuo.ViewModels;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Utilities
{
    public static class Instances
    {
        public static KeyboardHook KeyboardHook { get; set; }
        public static NormalKeyViewModel NormalKeyViewModel { get; private set; }
        public static FishingViewModel FishingViewModel { get; private set; }
        public static TopMostViewModel TopMostViewModel { get; private set; }
        public static ConfigDrawerViewModel ConfigDrawerViewModel { get; private set; }
        public static HotKeyViewModel HotKeyViewModel { get; private set; }
        public static TaskbarViewModel TaskbarViewModel { get; private set; }
        public static NoClientAreaViewModel NoClientAreaViewModel { get; private set; }

        public static void Instantiate(IContainer container)
        {
            KeyboardHook = new KeyboardHook();
            NormalKeyViewModel = container.Get<NormalKeyViewModel>();
            FishingViewModel = container.Get<FishingViewModel>();
            TopMostViewModel = container.Get<TopMostViewModel>();
            ConfigDrawerViewModel = container.Get<ConfigDrawerViewModel>();
            HotKeyViewModel = container.Get<HotKeyViewModel>();
            TaskbarViewModel = container.Get<TaskbarViewModel>();
            NoClientAreaViewModel = container.Get<NoClientAreaViewModel>();
        }
    }
}
