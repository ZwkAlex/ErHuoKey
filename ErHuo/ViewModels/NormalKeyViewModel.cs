
using Stylet;
using StyletIoC;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Screen = Stylet.Screen;
using IContainer = StyletIoC.IContainer;
using ErHuo.Utilities;
using System;
using HandyControl.Controls;
using ErHuo.Services;
using static ErHuo.Utilities.HwndUtil;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using ErHuo.Models;

namespace ErHuo.ViewModels
{
    public class NormalKeyViewModel : Screen
    {
        private NormalKeyService _normalKeyService;
        private readonly IContainer _container;
        public NormalKeyConfigViewModel NormalKeyConfig { get; set; }

        public NormalKeyViewModel(IContainer container)
        {
            _container = container;
            NormalKeyConfig = container.Get<NormalKeyConfigViewModel>();
            DisplayName = Constant.NormalKeyTabTitle;
        }

        public NormalKeyConfigSheet GetConfig()
        {
            List<KeyEvent> keylist = new List<KeyEvent>(NormalKeyConfig.KeyList);
            int frequency = NormalKeyConfig.Frequency;
            int keymode = NormalKeyConfig.KeyMode;
            WindowInfo windowInfo = NormalKeyConfig.CurrentWindow;
            NormalKeyConfigSheet config = new NormalKeyConfigSheet(keylist, frequency, keymode, windowInfo);
            return config;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();
        }

        public void DeleteKey(string deletekey)
        {
            NormalKeyConfig.DeleteKey(deletekey);
        }

        public void ClickItem(string parameter)
        {
            NormalKeyConfig.ClickItem(parameter);
        }

        public void Start(CancellationToken Token)
        {
            NormalKeyConfigSheet _config = GetConfig();
            _normalKeyService = new NormalKeyService(_config, Token);
            _normalKeyService.StartService();
        }

        public void Stop()
        {
            _normalKeyService?.StopService();
        }
    }

}
