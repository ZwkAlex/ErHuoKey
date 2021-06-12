using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using lwplug;

namespace ErHuo
{
    static class KeyThread
    {
        private static int hwnd;
        private static int key_mode;
        private static lwsoft lw;
        private static int frequency;
        private static List<KeyEvent> key_list;
        private static STATUS status = STATUS.STOP;
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
            lw.DownCpu(50);
            while (true)
            {
                if (status == STATUS.STOP) break;
                switch (status)
                {
                    case STATUS.PAUSE:
                        Thread.Sleep(50);
                        continue;
                    case STATUS.START:
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
            lw.DownCpu(50);
            string revive_red = "ad5c4d-030303";
            string revive_yellow = "c9ab44-030303";
            string collect_yellow = "7c6324-030303";
            bool revive = ConfigUtil.Config.Config_Fish_Revive;
            Point point1 = ConfigUtil.Config.Config_Fish_Point1;
            Point point2 = ConfigUtil.Config.Config_Fish_Point2;
            Point point3 = ConfigUtil.Config.Config_Fish_Point3;
            while (true)
            {
                if (status == STATUS.STOP) break;
                switch (status)
                {
                    case STATUS.PAUSE:
                        Thread.Sleep(50);
                        continue;
                    case STATUS.START:
                        int count = 0;
                        lw.KeyPress(ConfigUtil.Config.Config_Key_Fish_Release);
                        Thread.Sleep(3000);
                        while (true) {
                            count++;
                            if (revive)
                            {
                                int num = lw.FindColorBlock(IntRound(point2.X - 30), IntRound(point2.Y - 25), point2.X + 30, point2.Y + 25, 60, 50, revive_red);
                                //930, 380, 990, 430, 60, 50
                                if ( num >= 200)
                                {
                                    System.Media.SystemSounds.Question.Play();
                                    Thread.Sleep(5000);
                                    while (true) {
                                        Thread.Sleep(50);
                                        //800, 500, 1000, 550,
                                        int locate = lw.FindColor(IntRound(point3.X - 25), IntRound(point3.Y - 10), point3.X + 25, point3.Y + 10, revive_yellow , (float)0.85, 4, 3000);
                                        if (locate != 0)
                                        {
                                            lw.MoveTo(lw.x(), lw.y());
                                            lw.LeftClick();
                                        }
                                        else
                                        {
                                            lw.MoveTo(point3.X, point3.Y);
                                            lw.LeftClick();
                                        }
                                        Thread.Sleep(500);
                                        //930, 380, 990, 430, 60, 50,
                                        if (lw.FindColorBlock(IntRound(point2.X - 30), IntRound(point2.Y - 25), point2.X + 30, point2.Y + 25, 60, 50, revive_red) < 50)
                                            break;
                                    }
                                    break;
                                }
                            }
                            //850, 650, 1050, 750, 200, 100
                            if (lw.FindColorBlock(IntRound(point1.X - 100), IntRound(point1.Y - 50), point1.X + 100, point1.Y + 50, 200, 100, collect_yellow) >= 150|| count>=500)
                            {
                                lw.KeyPress(ConfigUtil.Config.Config_Key_Fish_Collect);
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

        public static STATUS GetStatus()
        {
            return status;
        }
        public static void Pause()
        {
            status = STATUS.PAUSE;
        }
        public static void Start()
        {
            status = STATUS.START;
        }
        public static void Stop()
        {
            status = STATUS.STOP;
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
        
        public static int IntRound(int n)
        {
            return n <= 0 ? 0 : n;
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
