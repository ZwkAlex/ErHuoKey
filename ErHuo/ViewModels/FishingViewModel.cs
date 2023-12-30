﻿using ErHuo.Models;
using ErHuo.Plugins;
using ErHuo.Service;
using ErHuo.Utilities;
using HandyControl.Controls;
using HandyControl.Data;
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
using System.Windows.Media.Imaging;

namespace ErHuo.ViewModels
{
    public class FishingViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private EKey _keyFishingRelease = ConfigFactory.GetValue(ConfigKey.KeyFishingRelease, new EKey("KEY_1"));
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
        private EKey _keyFishingFinish = ConfigFactory.GetValue(ConfigKey.KeyFishingFinish, new EKey("KEY_2"));
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
        private EKey _keyCollect = ConfigFactory.GetValue(ConfigKey.KeyCollect, new EKey("KEY_F"));
        public string KeyCollectName
        {
            get
            {
                return _keyCollect.Name;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && KeyManager.KeyStart.Key != value && KeyManager.KeyStop.Key != value)
                {
                    EKey newKey = EKey.GetEKeyFromName(value);
                    ConfigFactory.SetValue(ConfigKey.KeyCollect, newKey);
                    SetAndNotify(ref _keyCollect, newKey);
                }
                else
                {
                    Growl.Info(new GrowlInfo() { WaitTime = 2, Message = Constant.FishingKeySetError, ShowDateTime = false });
                    SetAndNotify(ref _keyCollect, _keyCollect);
                }
            }
        }
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
                _fishingRevive = ConfigFactory.GetValue(ConfigKey.FishingRevive, false);
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
                return ConfigFactory.GetValue(ConfigKey.FishingNoticePoint, new CursorPoint());
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

        private CursorPoint FishingInjuredPoint
        {
            get
            {
                return ConfigFactory.GetValue(ConfigKey.FishingInjuredPoint, new CursorPoint());
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.FishingInjuredPoint, value);
                NotifyOfPropertyChange(nameof(FishingInjuredPointText));
            }
        }

        public string FishingInjuredPointText
        {
            get
            {
                return FishingInjuredPoint.ToString();
            }
        }

        private CursorPoint FishingRevivePoint
        {
            get
            {
                return ConfigFactory.GetValue(ConfigKey.FishingRevivePoint, new CursorPoint());
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

        private FishingService _fishingService;

        public FishingViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            DisplayName = Constant.FishingTabTitle;
        }

        public FishingConfigSheet GetConfig()
        {
            FishingConfigSheet config = new FishingConfigSheet(
                keyFishingRelease: _keyFishingRelease,
                keyFishingFinish: _keyFishingFinish,
                keyCollect: _keyCollect,
                fishingRevive: _fishingRevive,
                fishingNoticePoint: FishingNoticePoint,
                fishingInjuredPoint: FishingInjuredPoint,
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
            if (config.FishingRevive)
            {
                if (!config.FishingRevivePoint.IsValid() || FileManager.FindLocalFile(Constant.FishingReviveFile) == null)
                {
                    return false;
                }
            }
            return true;
        }

        public void Start(CancellationToken Token)
        {
            FishingConfigSheet config = GetConfig();
            if (!CheckConfig(config))
            {
                throw new ServiceConfigException("错误的设置，请进行钓鱼上钩提示点选取");
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
            P p = new P();
            TopMostViewModel topMostViewModel = Instances.TopMostViewModel;
            Instances.HotKeyViewModel.QueueBusy();
            _windowManager.ShowWindow(topMostViewModel);
            topMostViewModel.ShowCursorLocation("正在等待找点完成", 300000, xpadding: 200, ypadding: 100);
            if (p.WaitKey(4, 30000) != -1)
            {
                CursorPoint cursorPoint = CursorUtil.doGetCursorPos();
                if (param == "Notice")
                {
                    FileManager.SaveBytesToLocal(topMostViewModel.ImageBytes, Constant.FishingNoticeFile);
                    FishingNoticePoint = cursorPoint;
                }
                else if (param == "Revive")
                {
                    FileManager.SaveBytesToLocal(topMostViewModel.ImageBytes, Constant.FishingReviveFile);
                    FishingRevivePoint = cursorPoint;
                }
            }
            topMostViewModel.RequestClose();
            Instances.HotKeyViewModel.DequeueBusy();
            p.Dispose();
        }

        public void OpenManual()
        {
            _windowManager.ShowWindow(new FishingManualViewModel());
        }
    }
}
