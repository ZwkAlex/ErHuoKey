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
        public NormalKeyService(NormalKeyConfigSheet config, CancellationToken Token) : base(config,Token)
        {
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
                            bool result = false;
                            if (CheckKeyIsMouse(key))
                            {
                                if (key.Code == (int)VK.LBUTTON)
                                {
                                    result = p.LeftClick();
                                }
                                else if (key.Code == (int)VK.RBUTTON)
                                {
                                    result = p.RightClick();
                                }
                                else if (key.Code == (int)VK.MBUTTON)
                                {
                                    result = p.MiddleClick();
                                }
                                else if (key.Code == (int)VK.XBUTTON1)
                                {
                                    result = p.KeyPress((int)VK.XBUTTON1);
                                }
                                else if (key.Code == (int)VK.XBUTTON2)
                                {
                                    result = p.KeyPress((int)VK.XBUTTON2);
                                }
                                else if (key.Code == (int)VK.SCROLL_UP)
                                {
                                    result = p.WheelDown();
                                }
                                else if (key.Code == (int)VK.SCROLL_DOWN)
                                {
                                    result = p.WheelUp();
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                result = p.KeyPress(key.Code);
                            }
                            if (!result)
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

        public bool CheckKeyIsMouse(EKey key)
        {
            return key.Code == (int)VK.LBUTTON || key.Code == (int)VK.RBUTTON || key.Code == (int)VK.MBUTTON || key.Code == (int)VK.XBUTTON1 || key.Code == (int)VK.XBUTTON2 || key.Code == (int)VK.SCROLL_UP || key.Code == (int)VK.SCROLL_DOWN;
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
