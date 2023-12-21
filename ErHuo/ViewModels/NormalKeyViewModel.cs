
using Stylet;
using StyletIoC;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Screen = Stylet.Screen;
using IContainer = StyletIoC.IContainer;
using ErHuo.Utilities;
using System;
using HandyControl.Controls;
using ErHuo.Service;
using static ErHuo.Utilities.HwndUtil;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using ErHuo.Models;

namespace ErHuo.ViewModels
{
    public class NormalKeyViewModel : Screen
    {
        private NormalKeyService normalKeyService;
        private readonly IContainer _container;
        public NormalKeyConfigViewModel NormalKeyConfig { get; set; }

        public NormalKeyViewModel(IContainer container)
        {
            _container = container;
            NormalKeyConfig = container.Get<NormalKeyConfigViewModel>();
            normalKeyService = new NormalKeyService();
            DisplayName = "循环按键";
        }

        public NormalKeyConfigSheet GetConfig()
        {
            List<KeyEvent> keylist = new List<KeyEvent>(NormalKeyConfig.KeyList);
            int frequency = NormalKeyConfig.Frequency;
            int keymode = NormalKeyConfig.KeyMode;
            WindowInfo windowInfo = NormalKeyConfig.CurrentSelectedWindow;
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

        public void Start(CancellationToken Token)
        {
            normalKeyService.UpdateConfigSheet(GetConfig());
            normalKeyService.SetCancellationToken(Token);
            normalKeyService.StartService();
        }

        public void Stop()
        {
            normalKeyService.StopService();
        }
    }

}
