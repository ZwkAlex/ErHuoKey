using ErHuo.Models;
using ErHuo.Plugins;
using ErHuo.Utilities;
using HandyControl.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ErHuo.Utilities.HwndUtil;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ErHuo.Service
{
    public class NormalKeyService : IService
    {
        public NormalKeyService(NormalKeyConfigSheet _config, CancellationToken Token) : base(Token)
        {
            config = _config;
        }

        public override void Service()
        {
            NormalKeyConfigSheet _config = (NormalKeyConfigSheet)config;
            try
            {
                while (!Token.IsCancellationRequested)
                {
                    foreach (KeyEvent key in _config.Keylist)
                    {
                        Token.ThrowIfCancellationRequested();
                        if (key.Activate)
                        {
                            if (p.KeyPress(key.Code))
                            {
                                throw new KeyException("Key Press Failed");
                            }
                            if (_config.KeyMode == 0)
                            {
                                Thread.Sleep(_config.Frequency);
                            }
                        }
                    }
                    if (_config.KeyMode == 1)
                    {
                        Thread.Sleep(_config.Frequency);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    public class NormalKeyConfigSheet : ConfigSheet
    {
        public List<KeyEvent> Keylist { get; set; }
        public int Frequency { get; set; }
        public int KeyMode { get; set; }
        public NormalKeyConfigSheet(List<KeyEvent> keylist, int frequency, int keyMode, WindowInfo windowInfo)
        {
            Keylist = keylist;
            Frequency = frequency;
            KeyMode = keyMode;
            WindowInfo = windowInfo;
        }
    }
}
