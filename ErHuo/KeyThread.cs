using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using donet_ts;
using lwplug;

namespace ErHuo
{
    static class KeyThread
    {
        public static int EXIT = 0;
        public static int START = 1;
        public static int STOP = 2;
        public static int PAUSE = 3;
        private static bool ChangeWindow;
        private static IntPtr hwnd;
        private static lwsoft lw;
        private static TSPlugInterFace ts;
        private static int frequency;
        private static List<KeyEvent> key_list;
        private static int stauts = 2;
        private static ThreadStart threadstart;
        private static Thread keyThread;

        public static void Init(int DriveCode)
        {
            threadstart = new ThreadStart(KeyLoop_lw);
            keyThread = new Thread(threadstart);
            keyThread.SetApartmentState(ApartmentState.STA);
            ChangeWindow = false;
            keyThread.Start();
        }

        private static void KeyLoop_lw()
        {
            lw = new lwsoft();
            lw.BindWindow(-1);
            lw.SetSimMode(1);
            lw.DownCpu(80);
            while (stauts != EXIT)
            {
                
                while (stauts != START && stauts != EXIT) { 
                    if (ChangeWindow)
                    {
                        if (hwnd != IntPtr.Zero && hwnd != null)
                        {
                            lw.BindWindow((int)hwnd, 0, 0, 1);
                        }
                        else
                        {
                            lw.BindWindow(-1);
                        }
                        ChangeWindow = false;
                    }
                    Thread.Sleep(50);
                }
                if (stauts == EXIT)
                    break;
                if (frequency <= 10)
                    Thread.Sleep(10);
                else
                    Thread.Sleep(frequency);
                foreach (KeyEvent key in key_list)
                {
                    if (key.Activate)
                    {
                        lw.KeyPress(key.Code);
                    }
                }
            }
        }

        private static void KeyLoop_ts()
        {
            ts = new TSPlugInterFace();
            ts.SetSimMode(1);
            while (stauts != EXIT)
            {
                while (stauts != START && stauts != EXIT)
                    Thread.Sleep(50);
                if (stauts == EXIT)
                    break;
                if (frequency <= 10)
                    Thread.Sleep(10);
                else
                    Thread.Sleep(frequency);
                foreach (KeyEvent key in key_list)
                {
                    if (key.Activate)
                    {
                        ts.KeyPress(key.Code);
                    }
                }
            }
        }
        public static Thread GetThread()
        {
            return keyThread;
        }
        public static void SetThread(List<KeyEvent> list,int freq)
        {
            key_list = list;
            frequency = freq;
        }
        public static int GetStatus()
        {
            return stauts;
        }
        public static void Pause()
        {
            stauts = PAUSE;
        }
        public static void Start()
        {
            stauts = START;
        }
        public static void Stop()
        {
            stauts = STOP;
        }
        public static void Exit()
        {
            stauts = EXIT;
        }
        public static void SetWindowHwnd(IntPtr _hwnd)
        {
            hwnd = _hwnd;
            ChangeWindow = true;
        }
    }
}
