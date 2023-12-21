using ErHuo.Models;
using ErHuo.Plugin;
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
    public class NormalKeyService: IService
    {
        
        public void UpdateConfigSheet(NormalKeyConfigSheet _config)
        {
            config = _config;
        }

        public override void Service()
        {
            NormalKeyConfigSheet _config = (NormalKeyConfigSheet)config;
            try {
                while (!Token.IsCancellationRequested)
                {
                    foreach (KeyEvent key in _config.Keylist)
                    {
                        Token.ThrowIfCancellationRequested();
                        if (key.Activate)
                        {
                            int result = p.KeyPress(key.Code);
                            if (result == 0)
                            {
                                throw new KeyException("Key Press Failed");
                            }
                        }
                        Thread.Sleep(_config.Frequency);
                    }
                }
            }catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    public class NormalKeyConfigSheet: ConfigSheet
    {
        public List<KeyEvent> Keylist {  get; set; }
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
