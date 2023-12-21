using ErHuo.Plugin;
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
        public static P P {  get; private set; }
        public static KeyboardHook KeyboardHook { get; set; }
        public static NormalKeyViewModel NormalKeyViewModel { get; private set; }
        public static OtherViewModel OtherViewModel { get; private set; }
        //public static HotKeyViewModel HotKeyViewModel { get; private set; }
        //public static NormalKeyConfigViewModel NormalKeyConfigViewModel { get; private set; }

        public static void Instantiate(IContainer container)
        {
            P = new P();
            KeyboardHook = new KeyboardHook();
            NormalKeyViewModel = container.Get<NormalKeyViewModel>();
            OtherViewModel = container.Get<OtherViewModel>();
            //HotKeyViewModel = container.Get<HotKeyViewModel>();
            //NormalKeyConfigViewModel = container.Get<NormalKeyConfigViewModel>();
        }
    }
}
