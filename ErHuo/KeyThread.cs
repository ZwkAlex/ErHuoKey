using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using donet_ts;
using lwplug;

namespace ErHuo
{
    static class KeyThread
    {
        private static int foobar;
        private static int hwnd;
        private static int key_mode;
        private static lwsoft lw;
        private static int frequency;
        private static List<KeyEvent> key_list;
        private static int status = 2;
        private static ThreadStart threadstart;
        private static Thread keyThread;

        public static void Init(IntPtr _hwnd, int mode, int freq)
        {
            
            key_mode = mode;
            frequency = freq;
            hwnd = (int)_hwnd;
            switch (key_mode)
            {
                case 0:
                    threadstart = new ThreadStart(NormoalMode);
                    break;
                case 1:
                    threadstart = new ThreadStart(FishingMode);
                    break;
            }
            keyThread = new Thread(threadstart);
            keyThread.SetApartmentState(ApartmentState.STA);
            keyThread.Start();
        }

        private static void NormoalMode()
        {
            lw = new lwsoft();
            Bind_Window(hwnd);
            lw.SetSimMode(1);
            lw.DownCpu(80);
            while (true)
            {
                if (status == (int)STATUS.STOP) break;
                switch (status)
                {
                    case (int)STATUS.PAUSE:
                        Thread.Sleep(50);
                        continue;
                    case (int)STATUS.START:
                        foreach (KeyEvent key in key_list)
                        {
                            if (key.Activate)
                            {
                                lw.KeyPress(key.Code);
                            }
                        }
                        Thread.Sleep(frequency);
                        continue;
                }  
            }
        }


        private static void FishingMode()
        {
            lw = new lwsoft();
            Bind_JX3Window();
            lw.SetSimMode(1);
            lw.DownCpu(80);
            
            while (true)
            {
                if (status == (int)STATUS.STOP) break;
                switch (status)
                {
                    case (int)STATUS.PAUSE:
                        Thread.Sleep(50);
                        continue;
                    case (int)STATUS.START:
                        int count = 0;
                        lw.KeyPress((int)VK.KEY_3);
                        while (true) {
                            count++;
                            if (lw.FindColorBlock(850, 650, 1050, 750, 200, 100, "ffff00-03030303") >= 200|| count>=800)
                            {
                                lw.KeyPress((int)VK.KEY_4);
                                break;
                            }
                            Thread.Sleep(50);
                        }
                        Thread.Sleep(3000);            
                        continue;
                }
            }
        }

        public static Thread GetThread()
        {
            return keyThread;
        }
        public static void SetKeyList(List<KeyEvent> list)
        {
            key_list = list;

        }

        public static int GetStatus()
        {
            return status;
        }
        public static void Pause()
        {
            status = (int)STATUS.PAUSE;
        }
        public static void Start()
        {
            status = (int)STATUS.START;
        }
        public static void Stop()
        {
            status = (int)STATUS.STOP;
            try
            {
                keyThread.Abort();
            }
            catch
            {

            }
        }
        public static void Exit()
        {
            if(keyThread!=null)
                try
                {
                    keyThread.Abort();
                }
                catch
                {
                
                }
        }
        
        
        private static void Bind_Window(int hwnd)
        {
            lw.UnBindWindow();
            if (hwnd != 0)
            {
                if (lw.BindWindow((int)hwnd, 0, 0, 1) == 0)
                {
                    hwnd =  0;
                    lw.BindWindow(-1);
                    Stop();
                }
                Thread.Sleep(50);
            }
            else
            {
                lw.BindWindow(-1);
            }
            
        }

        private static void Bind_JX3Window()
        {
            lw.UnBindWindow();
            hwnd = lw.FindWindow("剑网3 -");
            if(true)
            //if (hwnd == 0)
            {
                //MessageBox.Show("找不到剑网三窗口，已经自动改为前台按键");
                lw.BindWindow(-1);
            }
            else
            {
                lw.BindWindow((int)hwnd, 0, 0, 1);
            }
        }
    }
}
