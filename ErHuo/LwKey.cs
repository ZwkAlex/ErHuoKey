using System.Runtime.InteropServices;

namespace ErHuo
{
    public static class LwKey
    {
        [DllImport("lw.dll")]
        static extern int DllRegisterServer();
        [DllImport("lw.dll")]
        static extern int DllUnregisterServer();
        [DllImport("lw.dll")]
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
