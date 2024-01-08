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
            WindowRect _noticeRect = _fullRect.SubRect(_config.FishingNoticePoint, 0.1, 0.1);
            WindowRect _reviveRect = _fullRect.SubRect(0.45, 0.45, 0.8, 0.02);
            WindowRect _collectRect = _fullRect.SubRect(0.45, 0.45, 0.8, 0.02);
            FileManager.SaveBitmapToLocal(Properties.Resources.FishingCollect, Constant.FishingCollectFile);
            FileManager.SaveBitmapToLocal(Properties.Resources.FishingRevive, Constant.FishingReviveFile);
            while (!Token.IsCancellationRequested)
            {
                p.KeyPress(_config.KeyFishingRelease.Code);
                while (!Token.IsCancellationRequested)
                {
                    if (_config.FishingRevive)
                    {
                        if (p.FindPic(_reviveRect, FileManager.FindLocalFile(Constant.FishingReviveFile)).IsValid())
                        {
                            Thread.Sleep(1000);
                            p.MoveTo(_config.FishingRevivePoint);
                            p.LeftClick();
                        }
                    }
                    if (p.FindPic(_noticeRect, FileManager.FindLocalFile(Constant.FishingNoticeFile)).IsValid())
                    {
                        p.KeyPress(_config.KeyFishingFinish.Code);
                        Thread.Sleep(3000);
                        while (!Token.IsCancellationRequested)
                        {
                            CursorPoint _findPoint = p.FindPic(_collectRect, FileManager.FindLocalFile(Constant.FishingCollectFile));
                            if(_findPoint.IsValid())
                            {
                                p.MoveTo(_findPoint);
                                p.LeftClick();
                            }
                            Thread.Sleep(500);
                        }

                    }
                    Thread.Sleep(500);
                }
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
        public CursorPoint FishingInjuredPoint { get; set; }
        public CursorPoint FishingRevivePoint { get; set; }

        public FishingConfigSheet(WindowInfo JX3, EKey keyFishingRelease, EKey keyFishingFinish, bool fishingRevive, CursorPoint fishingNoticePoint, CursorPoint fishingInjuredPoint, CursorPoint fishingRevivePoint)
        {
            WindowInfo = JX3;
            KeyFishingRelease = keyFishingRelease;
            KeyFishingFinish = keyFishingFinish;
            FishingRevive = fishingRevive;
            FishingNoticePoint = fishingNoticePoint;
            FishingInjuredPoint = fishingInjuredPoint;
            FishingRevivePoint = fishingRevivePoint;
        }
    }
}
