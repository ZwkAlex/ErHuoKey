using ErHuo.Plugin;
using ErHuo.Utilities;
using ErHuo.ViewModels;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo
{
    public class Bootstrapper : Bootstrapper<MainViewModel>
    {
        protected override void OnStart()
        {
            ConfigFactory.LoadConfigFile();
        }
        protected override void Configure()
        {
            base.Configure();
            Instances.Instantiate(Container);
        }

    }
}
