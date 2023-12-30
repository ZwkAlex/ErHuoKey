using ErHuo.Models;
using ErHuo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ErHuo.Service
{
    public class FishingService : IService
    {
        public FishingService(FishingConfigSheet _config, CancellationToken Token) : base(Token)
        {
            _config.WindowInfo = p.FindWindowJX3();
            config = _config;
        }

        public override void Service()
        {
            FishingConfigSheet _config = (FishingConfigSheet)config;
            while (!Token.IsCancellationRequested)
            {
                p.KeyPress(_config.KeyFishingRelease.Code);
                while (!Token.IsCancellationRequested)
                {
                    if (_config.FishingRevive)
                    {
                        if (p.FindPic(_config.FishingReviveUpperLeft, _config.FishingReviveBottomRight, FileManager.FindLocalFile(Constant.FishingReviveFile)).IsValid())
                        {
                            Thread.Sleep(1000);
                            p.MoveTo(_config.FishingRevivePoint);
                            p.LeftClick();
                        }
                    }
                    if (p.FindPic(_config.FishingNoticeUpperLeft, _config.FishingNoticeBottomRight, FileManager.FindLocalFile(Constant.FishingNoticeFile)).IsValid())
                    {
                        p.KeyPress(_config.KeyFishingFinish.Code);
                        Thread.Sleep(5000);
                        p.KeyPress(_config.KeyCollect.Code);
                    }
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
        public CursorPoint FishingNoticeUpperLeft { get; set; }
        public CursorPoint FishingNoticeBottomRight { get; set; }
        public CursorPoint FishingInjuredPoint { get; set; }
        public CursorPoint FishingRevivePoint { get; set; }
        public CursorPoint FishingReviveUpperLeft { get; set; }
        public CursorPoint FishingReviveBottomRight { get; set; }

        public FishingConfigSheet(EKey keyFishingRelease, EKey keyFishingFinish, EKey keyCollect, bool fishingRevive, CursorPoint fishingNoticePoint, CursorPoint fishingInjuredPoint, CursorPoint fishingRevivePoint)
        {
            KeyFishingRelease = keyFishingRelease;
            KeyFishingFinish = keyFishingFinish;
            KeyCollect = keyCollect;
            FishingRevive = fishingRevive;
            FishingNoticePoint = fishingNoticePoint;
            FishingNoticeUpperLeft = new CursorPoint(fishingNoticePoint.x - 300, fishingNoticePoint.y - 200);
            FishingNoticeBottomRight = new CursorPoint(fishingNoticePoint.x + 300, fishingNoticePoint.y + 200);
            FishingInjuredPoint = fishingInjuredPoint;
            FishingRevivePoint = fishingRevivePoint;
            FishingReviveUpperLeft = new CursorPoint(fishingRevivePoint.x - 300, fishingRevivePoint.y - 200);
            FishingReviveBottomRight = new CursorPoint(fishingRevivePoint.x + 300, fishingRevivePoint.y + 200);
        }
    }
}
