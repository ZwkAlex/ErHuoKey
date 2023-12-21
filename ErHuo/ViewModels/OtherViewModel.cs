using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.ViewModels
{
    public class OtherViewModel:Screen
    {
        private readonly IContainer _container;
        public OtherViewModel(IContainer container)
        {
            _container = container;
            DisplayName = "其他工具";
        }
        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();
        }
    }
}
