using ErHuo.Models;
using ErHuo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ErHuo.Services
{
    public class FishingService : IService
    {
        public FishingService(ConfigSheet config, CancellationToken Token) : base(config, Token)
        {
        }

        public override void Service()
        {
            FishingConfigSheet _config = (FishingConfigSheet)config;
            WindowRect _fullRect = Tool.GetFullScreenRect();
            WindowRect _noticeRect = _fullRect.SubRect(_config.FishingNoticePoint, 0.01, 0.014);
            WindowRect _fishingRect = _fullRect.SubRect(_config.FishingNoticePoint, 0.01, 0.014);
            WindowRect _reviveRect = _fullRect.SubRect(0.47, 0.48, 0.1, 0.02);
            WindowRect _collectRect = _fullRect.SubRect(0.5, 0.6, 0.1, 0.02);
            FileManager.SaveBitmapToLocal(Properties.Resources.FishingCollect, Constant.FishingCollectFile);
            FileManager.SaveBitmapToLocal(Properties.Resources.ReviveDisable, Constant.ReviveDisable);
            FileManager.SaveBitmapToLocal(Properties.Resources.ReviveEnable, Constant.ReviveEnable);
            while (!Token.IsCancellationRequested)
            {
                if (!p.FindPic(_fishingRect, FileManager.FindLocalFile(Constant.FishingBuffFile)).IsValid())
                {
                    p.KeyPress(_config.KeyFishingRelease.Code);
                    Thread.Sleep(500);
                }
                if (_config.FishingRevive)
                {
                    while (true)
                    {
                        bool IsReviveEnableVisible = p.FindPic(_reviveRect, FileManager.FindLocalFile(Constant.ReviveEnable)).IsValid();
                        bool IsReviveDisableVisible = p.FindPic(_reviveRect, FileManager.FindLocalFile(Constant.ReviveDisable)).IsValid();
                        if (!IsReviveDisableVisible && !IsReviveEnableVisible)
                        {
                            break;
                        }
                        if (IsReviveDisableVisible)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        if (IsReviveEnableVisible)
                        {
                            p.MoveTo(_fullRect.ReleventPointToAbsolute(0.48, 0.48));
                            Thread.Sleep(500);
                            p.LeftClick();
                            break;
                        }
                    }
                }
                CursorPoint _findPoint = p.FindPic(_collectRect, FileManager.FindLocalFile(Constant.FishingCollectFile));
                if (_findPoint.IsValid())
                {
                    p.MoveTo(_findPoint);
                    Thread.Sleep(500);
                    p.LeftClick();
                    Thread.Sleep(500);
                    p.MoveTo(new CursorPoint(0, 0));
                    if (!p.FindPic(_collectRect, FileManager.FindLocalFile(Constant.FishingCollectFile)).IsValid())
                    {
                        continue;
                    }
                }
                if (p.FindPic(_noticeRect, FileManager.FindLocalFile(Constant.FishingNoticeFile)).IsValid())
                {
                    p.KeyPress(_config.KeyFishingFinish.Code);
                    Thread.Sleep(5000);
                }
                Thread.Sleep(1000);
            }
        }

    }

    public class FishingConfigSheet : ConfigSheet
    {
        public EKey KeyFishingRelease { get; set; }
        public EKey KeyFishingFinish { get; set; }
        public EKey KeyCollect { get; set; }
        public bool FishingRevive { get; set; }
        public CursorPoint FishingNoticePoint { get; set; }
        public CursorPoint FishingPoint { get; set; }
        public CursorPoint FishingRevivePoint { get; set; }

        public FishingConfigSheet(WindowInfo JX3, EKey keyFishingRelease, EKey keyFishingFinish, bool fishingRevive, CursorPoint fishingNoticePoint, CursorPoint fishingPoint, CursorPoint fishingRevivePoint)
        {
            WindowInfo = JX3;
            KeyFishingRelease = keyFishingRelease;
            KeyFishingFinish = keyFishingFinish;
            FishingRevive = fishingRevive;
            FishingNoticePoint = fishingNoticePoint;
            FishingPoint = fishingPoint;
            FishingRevivePoint = fishingRevivePoint;
        }
    }
}
