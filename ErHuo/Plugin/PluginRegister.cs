using lw;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ErHuo.Plugin
{

    public abstract class RegisterBase
    {
        public static readonly string REGISTEMESSAGE = "检测到首次加载，即将进行插件注册，点击确认接受注册，点击取消拒绝注册。";
        public static readonly string REGISTETITLE = "首次加载";
        public void ReleaseDLL(byte[] byDll, string name)
        {
            string strPath = System.AppDomain.CurrentDomain.BaseDirectory + name;//设置释放路径
                                                                                //创建dll文件（覆盖模式）
            using (FileStream fs = new FileStream(strPath, FileMode.Create))
            {
                fs.Write(byDll, 0, byDll.Length);
            }
        }
        public static void LaunchCommandLineApp(string commmand)
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
        public static bool GetRegisterStatus(string clsid)
        {
            bool hasRegister = false;
            RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32);
            string cld = String.Format("\\CLSID\\{0}{1}{2}", "{", clsid, "}");
            RegistryKey comKey = root.OpenSubKey(cld);
            hasRegister = comKey != null;
            return hasRegister;
        }
        public abstract void TryRegister();
        public abstract int Register();
        public abstract int UnRegister();
    }

    public class LwRegister: RegisterBase
    {
        [DllImport("lw.dll")]
        static extern int DllRegisterServer();
        [DllImport("lw.dll")]
        static extern int DllUnregisterServer();
        [DllImport("lw.dll")]
        static extern bool DllCanUnloadNow();

        public override void TryRegister()
        {
            if (!GetRegisterStatus("F6C2EA3D-2A5A-4B63-AFDB-5E24BD1D39A0"))
            { 
                if (MessageBox.Show(REGISTEMESSAGE, REGISTETITLE, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    Register();
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public override int Register()
        {
            ReleaseDLL(Properties.Resources.lw, "lw.dll");
            int result = DllRegisterServer();
            return result;
        }

        public override int UnRegister()
        {
            ReleaseDLL(Properties.Resources.lw, "lw.dll");
            return DllUnregisterServer();
        }

        public bool CanUnload()
        {
            return DllCanUnloadNow();
        }
    }
    //public class OpCom: RegisterBase
    //{
    //    public static IOpInterface Op { get; private set; }
    //    private readonly string REGISTERCOMMAND = "regsvr32 ./op_x64.dll";
    //    private readonly string UNREGISTERCOMMAND = "regsvr32 ./op_x64.dll /u";
    //    public OpCom()
    //    {
    //        try
    //        {
    //            Op = new OpInterface();
    //        }
    //        catch (Exception)
    //        {
    //            if (MessageBox.Show("检测到首次加载，即将进行插件注册，点击确认接受注册，点击取消拒绝注册。", "首次注册", MessageBoxButtons.OKCancel) == DialogResult.OK)
    //            {
    //                Register();
    //                Op = new OpInterface();
    //            }
    //            else
    //            {
    //                throw new Exception();
    //            }
    //        }
    //    }

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
