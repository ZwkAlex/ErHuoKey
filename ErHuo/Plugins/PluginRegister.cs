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

    public abstract class IRegister
    {
        private static string _registerKey;
        private static IRegister _instance;
        public static IRegister Instance
        {
            get
            {
                if (_instance == null)
                {
                    Plugin plugin = ConfigFactory.GetValue<Plugin>(ConfigKey.Plugin);
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
                    try
                    {
                        P p = new P();
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message, Constant.RegisterFailMessage);
                    }
                }
                else
                {
                    MessageBox.Show(Constant.CancelRegisterMessage, Constant.RegisterTitle);
                }
            }
            RegisterState.Instance.SetReg(GetRegisterStatus());
        }

        public void TryUnRegister()
        {
            if (MessageBox.Show(Constant.UnRegisterMessage, Constant.UnRegisterTitle, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                UnRegister();
                if (!GetRegisterStatus())
                {
                    MessageBox.Show(Constant.UnRegisterSuccess, Constant.UnRegisterTitle);
                }
                else
                {
                    MessageBox.Show(Constant.UnRegisterFail, Constant.UnRegisterTitle);
                }
            }
            RegisterState.Instance.SetReg(GetRegisterStatus());
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
            string cld = string.Format("\\CLSID\\{0}{1}{2}\\InProcServer32", "{", clsid, "}");
            RegistryKey comKey = root.OpenSubKey(cld);
            if (comKey == null)
            {
                return false;
            }
            else if (File.Exists(comKey.GetValue("").ToString()))
            {
                return true;
            }
            else
            {
                UnRegister();
                return false;
            }
        }

        public bool GetRegisterStatus()
        {
            return GetRegisterStatus(_registerKey);
        }

        public abstract void Register();
        public abstract void UnRegister();
    }

    public class LWRegister : IRegister
    {
        private const string LwFileName = "lw.dll";
        [DllImport(LwFileName)]
        static extern int DllRegisterServer();
        [DllImport(LwFileName)]
        static extern int DllUnregisterServer();
        [DllImport(LwFileName)]
        static extern bool DllCanUnloadNow();

        public override void Register()
        {
            ExtractDLL(Properties.Resources.lw, LwFileName);
            DllRegisterServer();
        }

        public override void UnRegister()
        {
            ExtractDLL(Properties.Resources.lw, LwFileName);
            DllUnregisterServer();
        }

        public bool CanUnload()
        {
            return DllCanUnloadNow();
        }
    }

    public class OPRegister : IRegister
    {
        private const string OPFileName = "op_x86.dll";
        [DllImport(OPFileName)]
        static extern int DllRegisterServer();
        [DllImport(OPFileName)]
        static extern int DllUnregisterServer();
        [DllImport(OPFileName)]
        static extern bool DllCanUnloadNow();
        public override void Register()
        {
            //ReleaseDLL(Properties.Resources.op_x86, OPFileName);
            DllRegisterServer();
        }

        public override void UnRegister()
        {
            //ReleaseDLL(Properties.Resources.op_x86, OPFileName);
            DllUnregisterServer();
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
