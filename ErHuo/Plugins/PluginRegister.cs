using ErHuo.Models;
using ErHuo.Utilities;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Windows.Forms;

namespace ErHuo.Plugins
{

    public abstract class RegisterBase
    {
        private static string _registerKey;
        private static RegisterBase _instance;
        public static RegisterBase Instance
        {
            get
            {
                if (_instance == null)
                {
                    Plugin plugin = ConfigFactory.GetValue(ConfigKey.Plugin, Plugin.LW);
                    if (plugin == Plugin.LW)
                    {
                        _instance = new LWRegister();
                        _registerKey = Constant.LWRegistryKey;
                    }
                    else if (plugin == Plugin.OP)
                    {
                        _instance = new OPRegister();
                        _registerKey = Constant.OpRegistryKey;
                    }
                }
                return _instance;
            }
        }

        public void TryRegister()
        {
            if (!GetRegisterStatus())
            {
                if (MessageBox.Show(Constant.RegisterMessage, Constant.RegisterTitle, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    Register();
                }
                else
                {
                    MessageBox.Show(Constant.CancelRegisterMessage, Constant.RegisterTitle);
                }
            }
        }

        public void TryUnRegister()
        {
            if (MessageBox.Show(Constant.UnRegisterMessage, Constant.UnRegisterTitle, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (!GetRegisterStatus() || UnRegister())
                {
                    MessageBox.Show(Constant.UnRegisterSuccess, Constant.UnRegisterTitle);
                }
                else
                {
                    MessageBox.Show(Constant.UnRegisterFail, Constant.UnRegisterTitle);
                }
            }
        }

        public void ExtractDLL(byte[] byDll, string name)
        {
            FileManager.SaveBytes(byDll, Path.Combine(Constant.BasePath, name));
        }
        public void LaunchCommandLineApp(string commmand)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C " + commmand;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.Verb = "runas";
            process.Start();
            process.WaitForExit();
            process.Close();

        }
        public bool GetRegisterStatus(string clsid)
        {
            RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32);
            string cld = string.Format("\\CLSID\\{0}{1}{2}", "{", clsid, "}");
            RegistryKey comKey = root.OpenSubKey(cld);
            return comKey != null;
        }

        public bool GetRegisterStatus()
        {
            return GetRegisterStatus(_registerKey);
        }

        public abstract bool Register();
        public abstract bool UnRegister();
    }

    public class LWRegister : RegisterBase
    {
        private const string LwFileName = "lw.dll";
        [DllImport(LwFileName)]
        static extern int DllRegisterServer();
        [DllImport(LwFileName)]
        static extern int DllUnregisterServer();
        [DllImport(LwFileName)]
        static extern bool DllCanUnloadNow();

        public override bool Register()
        {
            ExtractDLL(Properties.Resources.lw, LwFileName);
            int result = DllRegisterServer();
            return result == 1;
        }

        public override bool UnRegister()
        {
            ExtractDLL(Properties.Resources.lw, LwFileName);
            DllUnregisterServer();
            return true;
            //return CanUnload() && (DllUnregisterServer() == 1);
        }

        public bool CanUnload()
        {
            return DllCanUnloadNow();
        }
    }

    public class OPRegister : RegisterBase
    {
        private const string OPFileName = "op_x86.dll";
        [DllImport(OPFileName)]
        static extern int DllRegisterServer();
        [DllImport(OPFileName)]
        static extern int DllUnregisterServer();
        [DllImport(OPFileName)]
        static extern bool DllCanUnloadNow();
        public override bool Register()
        {
            //ReleaseDLL(Properties.Resources.op_x86, OPFileName);
            int result = DllRegisterServer();
            return result == 1;
        }

        public override bool UnRegister()
        {
            //ReleaseDLL(Properties.Resources.op_x86, OPFileName);
            DllUnregisterServer();
            return true;
            //return CanUnload() && (DllUnregisterServer() == 1);
        }

    }

    //public class OPRegister: RegisterBase
    //{
    //    public static IOpInterface Op { get; private set; }
    //    private readonly string REGISTERCOMMAND = "regsvr32 ./op_x64.dll";
    //    private readonly string UNREGISTERCOMMAND = "regsvr32 ./op_x64.dll /u";

    //    public void Register()
    //    {
    //        ReleaseDLL(Properties.Resources.op_x64, "op_x64.dll");
    //        LaunchCommandLineApp(REGISTERCOMMAND);
    //    }
    //    public void UnRegister()
    //    {
    //        LaunchCommandLineApp(UNREGISTERCOMMAND);
    //    }
    //}
}
