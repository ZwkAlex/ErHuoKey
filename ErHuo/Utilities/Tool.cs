using HandyControl.Controls;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using System.Windows.Data;

namespace ErHuo.Utilities
{

    public static class GetPointClass
    {
        public static Point point;
        //public static OpSoft op = Instances.Op;
        public static void Get_Point()
        {

            //lw.BindWindow(-1);
            //SystemSounds.Hand.Play();
            //if (lw.WaitKey(4, 0) != -1)
            //{
            //    SystemSounds.Hand.Play();
            //    point = Control.MousePosition;
            //}
            //lw.UnBindWindow();
        }


    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }


    public static class Tool
    {
        private static ThreadStart threadstart;
        private static Thread ToolThread;
        private static bool Process = false;

        public static Point GetPoint()
        {
            return new Point();
            //threadstart = new ThreadStart(GetPointClass.Get_Point);
            //ToolThread = new Thread(threadstart);
            //ToolThread.SetApartmentState(ApartmentState.STA);
            //Process = true;
            //ToolThread.Start();
            //ToolThread.Join();
            //Process = false;
            //return GetPointClass.point;
        }

        public static bool isProcess()
        {
            return Process;
        }

        public static int VKStringtoInt(string s)
        {
            if(s == null || s == string.Empty)
                return 70;
            return (int)Enum.Parse(typeof(VK), s);
        }

        public static string InttoVKString(int n)
        {
            return Enum.GetName(typeof(VK), n);
        }

    }

    public static class KeyManager
    {
        public static string KeyStart
        {
            get
            {
                return ConfigFactory.GetValue("KeyStart", "F1");
            }
            set
            {
                ConfigFactory.SetValue("KeyStart", value);
            }
        }
        public static string KeyStop
        {
            get
            {
                return ConfigFactory.GetValue("KeyStop", "F1");
            }
            set
            {
                ConfigFactory.SetValue("KeyStop", value);
            }
        }
       

        public static void SetKey(string keystart, string keystop)
        {
            KeyStart = keystart;
            KeyStop = keystop;
        }

        
    }
}
