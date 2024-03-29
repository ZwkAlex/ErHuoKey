﻿using ErHuo.Models;
using ErHuo.Plugins;
using ErHuo.Properties;
using ErHuo.Services;
using ErHuo.Utilities;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Themes;
using HandyControl.Tools;
using Newtonsoft.Json.Linq;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ErHuo.ViewModels
{
    public class FishingViewModel : Screen
    {
        private readonly BusyState busyState = BusyState.Instance;

        private readonly IWindowManager _windowManager;
        private EKey _keyFishingRelease = ConfigFactory.GetValue<EKey>(ConfigKey.KeyFishingRelease);
        public string KeyFishingReleaseName
        {
            get
            {
                return _keyFishingRelease.Name;
            }
            set
            {
                if (KeyManager.KeyStart.Key != value && KeyManager.KeyStop.Key != value)
                {
                    EKey newKey = EKey.GetEKeyFromName(value);
                    ConfigFactory.SetValue(ConfigKey.KeyFishingRelease, newKey);
                    SetAndNotify(ref _keyFishingRelease, newKey);
                }
                else
                {
                    Growl.Info(new GrowlInfo() { WaitTime = 2, Message = Constant.FishingKeySetError, ShowDateTime = false });
                    SetAndNotify(ref _keyFishingRelease, _keyFishingRelease);
                }
            }
        }
        private EKey _keyFishingFinish = ConfigFactory.GetValue<EKey>(ConfigKey.KeyFishingFinish);
        public string KeyFishingFinishName
        {
            get
            {
                return _keyFishingFinish.Name;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && KeyManager.KeyStart.Key != value && KeyManager.KeyStop.Key != value)
                {
                    EKey newKey = EKey.GetEKeyFromName(value);
                    ConfigFactory.SetValue(ConfigKey.KeyFishingFinish, newKey);
                    SetAndNotify(ref _keyFishingFinish, newKey);
                }
                else
                {
                    Growl.Info(new GrowlInfo() { WaitTime = 2, Message = Constant.FishingKeySetError, ShowDateTime = false });
                    SetAndNotify(ref _keyFishingFinish, _keyFishingFinish);
                }
            }
        }
        //private EKey _keyCollect = ConfigFactory.GetValue<EKey>(ConfigKey.KeyCollect);
        //public string KeyCollectName
        //{
        //    get
        //    {
        //        return _keyCollect.Name;
        //    }
        //    set
        //    {
        //        if (!string.IsNullOrEmpty(value) && KeyManager.KeyStart.Key != value && KeyManager.KeyStop.Key != value)
        //        {
        //            EKey newKey = EKey.GetEKeyFromName(value);
        //            ConfigFactory.SetValue(ConfigKey.KeyCollect, newKey);
        //            SetAndNotify(ref _keyCollect, newKey);
        //        }
        //        else
        //        {
        //            Growl.Info(new GrowlInfo() { WaitTime = 2, Message = Constant.FishingKeySetError, ShowDateTime = false });
        //            SetAndNotify(ref _keyCollect, _keyCollect);
        //        }
        //    }
        //}
        public Visibility ReviveVisiblity
        {
            get
            {
                return _fishingRevive ? Visibility.Visible : Visibility.Hidden;
            }
        }
        private bool _fishingRevive;
        public bool FishingRevive
        {
            get
            {
                _fishingRevive = ConfigFactory.GetValue<bool>(ConfigKey.FishingRevive);
                NotifyOfPropertyChange(nameof(ReviveVisiblity));
                return _fishingRevive;
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.FishingRevive, value);
                SetAndNotify(ref _fishingRevive, value);
            }
        }

        private CursorPoint FishingNoticePoint
        {
            get
            {
                return ConfigFactory.GetValue<CursorPoint>(ConfigKey.FishingNoticePoint);
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.FishingNoticePoint, value);
                NotifyOfPropertyChange(nameof(FishingNoticePointText));
            }
        }
        public string FishingNoticePointText
        {
            get
            {
                return FishingNoticePoint.ToString();
            }
        }

        private CursorPoint FishingPoint
        {
            get
            {
                return ConfigFactory.GetValue<CursorPoint>(ConfigKey.FishingPoint);
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.FishingPoint, value);
                NotifyOfPropertyChange(nameof(FishingPointText));
            }
        }

        public string FishingPointText
        {
            get
            {
                return FishingPoint.ToString();
            }
        }

        private CursorPoint FishingRevivePoint
        {
            get
            {
                return ConfigFactory.GetValue<CursorPoint>(ConfigKey.FishingRevivePoint);
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.FishingRevivePoint, value);
                NotifyOfPropertyChange(nameof(FishingRevivePointText));
            }
        }

        public string FishingRevivePointText
        {
            get
            {
                return FishingRevivePoint.ToString();
            }
        }

        private WindowInfo _jx3;
        public WindowInfo JX3
        {
            get
            {
                return _jx3;
            }
            set
            {
                SetAndNotify(ref _jx3, value);
            }
        }


        private FishingService _fishingService;

        public FishingViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            DisplayName = Constant.FishingTabTitle;
        }

        public FishingConfigSheet GetConfig()
        {
            FishingConfigSheet config = new FishingConfigSheet(
                JX3: new WindowInfo(),
                keyFishingRelease: _keyFishingRelease,
                keyFishingFinish: _keyFishingFinish,
                fishingRevive: _fishingRevive,
                fishingNoticePoint: FishingNoticePoint,
                fishingPoint: FishingPoint,
                fishingRevivePoint: FishingRevivePoint
            );
            return config;
        }

        public bool CheckConfig(FishingConfigSheet config)
        {
            if (_keyFishingRelease == null || _keyFishingFinish == null)
            {
                return false;
            }
            if (!config.FishingNoticePoint.IsValid() || FileManager.FindLocalFile(Constant.FishingNoticeFile) == null)
            {
                return false;
            }
            if (!config.FishingPoint.IsValid() || FileManager.FindLocalFile(Constant.FishingBuffFile) == null)
            {
                return false;
            }
            //if (config.FishingRevive)
            //{
            //    if (!config.FishingRevivePoint.IsValid() || FileManager.FindLocalFile(Constant.FishingReviveFile) == null)
            //    {
            //        return false;
            //    }
            //}
            return true;
        }

        public void Start(CancellationToken Token)
        {
            FishingConfigSheet config = GetConfig();
            if (!CheckConfig(config))
            {
                throw new ServiceConfigException(Constant.FishingBadConfig);
            }
            _fishingService = new FishingService(config, Token);
            _fishingService.StartService();
        }

        public void Stop()
        {
            _fishingService?.StopService();
        }

        public void GetPoint(string param)
        {
            TopMostViewModel topMostViewModel = Instances.TopMostViewModel;
            if (topMostViewModel.IsRuning())
            {
                Growl.Info(Constant.FindPointUnfinish);
                return;
            }
            P p = new P();
            busyState.QueueBusy();
            _windowManager.ShowWindow(topMostViewModel);
            int timeout = ConfigFactory.GetValue<int>(ConfigKey.WaitKeyTimeout);
            topMostViewModel.ShowCursorLocation("正在等待找点完成", timeout, xPad: 0.005, yPad: 0.007);
            if (p.WaitKey(4, timeout) != -1)
            {
                CursorPoint cursorPoint = CursorUtil.doGetCursorPos();
                if (param == "Notice")
                {
                    FileManager.SaveBytesToLocal(topMostViewModel.ImageBytes, Constant.FishingNoticeFile);
                    FishingNoticePoint = cursorPoint;
                }
                else if (param == "Fishing")
                {
                    FileManager.SaveBytesToLocal(topMostViewModel.ImageBytes, Constant.FishingBuffFile);
                    FishingPoint = cursorPoint;
                }
            }
            topMostViewModel.RequestClose();
            busyState.DequeueBusy();
            p.Dispose();
        }

        public void OpenManual()
        {
            _windowManager.ShowWindow(new FishingManualViewModel());
        }
    }
}
