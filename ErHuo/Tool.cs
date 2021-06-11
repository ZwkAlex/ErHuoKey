using System.Threading;
using System.Windows.Forms;
using lwplug;
using System.Drawing;
using System.Media;

namespace ErHuo
{

    public static class GetPointClass{
        private static lwsoft lw;
        public static Point point;
        public static void Get_Point()
        {
            lw = new lwsoft();
            lw.BindWindow(-1);
            SystemSounds.Hand.Play();
            if (lw.WaitKey(4, 0) != -1)
            {
                SystemSounds.Hand.Play();
                point = Control.MousePosition;
            }
            lw.UnBindWindow();
        }


    }

    static class Tool
    {
        private static ThreadStart threadstart;
        private static Thread ToolThread;
        private static bool Process =false;

        public static Point GetPoint()
        {
            threadstart = new ThreadStart(GetPointClass.Get_Point);
            ToolThread = new Thread(threadstart);
            ToolThread.SetApartmentState(ApartmentState.STA);
            Process = true;
            ToolThread.Start();
            ToolThread.Join();
            Process = false;
            return GetPointClass.point;
        }
        
        public static bool isProcess()
        {
            return Process;
        }
        
    }



}
