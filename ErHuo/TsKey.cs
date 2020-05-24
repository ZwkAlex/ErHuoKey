using System.Runtime.InteropServices;
using System.Windows.Forms;
using donet_ts;

namespace ErHuo
{
    public static class TsKey
    {
        [DllImport("ts.dll")]
        static extern int DllRegisterServer();
        [DllImport("ts.dll")]
        static extern int DllUnregisterServer();
        [DllImport("ts.dll")]
        static extern bool DllCanUnloadNow();

        public static int Register()
        {
            int result = DllRegisterServer();
            return result;
        }

        public static int UnRegister()
        {
            return DllUnregisterServer();
        }

        public static bool CanUnload()
        {  
            return DllCanUnloadNow();
        } 
    }
}
