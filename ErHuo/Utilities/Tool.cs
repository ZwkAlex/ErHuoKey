using ErHuo.Models;
using ErHuo.Plugins;
using ErHuo.Properties;
using HandyControl.Themes;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml.Linq;

namespace ErHuo.Utilities
{
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
        public static int VKStringtoInt(string s)
        {
            if (s == null || s == string.Empty)
                return 70;
            return (int)Enum.Parse(typeof(VK), s);
        }

        public static string InttoVKString(int n)
        {
            return Enum.GetName(typeof(VK), n);
        }

        public static void Log(string logString)
        {
            string file = Path.Combine(Constant.BasePath, "log.txt");
            FileStream fs;
            StreamWriter sw;
            if (File.Exists(file))
            {
                fs = new FileStream(file, FileMode.Append, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            }
            sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + "   ---   " + logString);
            sw.Close();
        }

        public static void EnableDarkTheme(bool enable)
        {
            if (enable)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }
        }

        public static void Restart()
        {
            ProcessStartInfo Info = new ProcessStartInfo
            {
                Arguments = "/C choice /C Y /N /D Y /T 1 & START \"\" \"" + Assembly.GetEntryAssembly().Location + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };
            Process.Start(Info);
            Process.GetCurrentProcess().Kill();
        }

    }

    public static class JX3WindowFinder
    {
        private static CancellationTokenSource cts;
        private static Thread thread;

        public static void Start()
        {
            if (cts != null)
            {
                return;
            }
            cts = new CancellationTokenSource();
            thread = new Thread(() =>
            {
                P p = new P();
                try {
                    while (!cts.IsCancellationRequested)
                    {
                        WindowInfo jx3 = p.FindWindowJX3();
                        Application.Current.Dispatcher.Invoke(() => {
                            Instances.FishingViewModel.JX3 = jx3;
                        });
                        Thread.Sleep(500);
                    }
                }catch (OperationCanceledException)
                {

                }
                finally
                {
                    cts = null;
                    thread.Abort();
                    p.Dispose();
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        } 

        public static void Stop()
        {
            if (cts != null && cts.Token.CanBeCanceled)
            {
                cts.Cancel();
            }
        }

    }

    public static class FileManager
    {
        public static string FindLocalFile(string name)
        {
            if (!Directory.Exists(Constant.ResourceDirPath))
            {
                return null;
            }
            string targetPath = Path.Combine(Constant.ResourceDirPath, name);
            return File.Exists(targetPath) ? targetPath : null;
        }
        public static string SaveBytesToLocal(byte[] data, string name)
        {
            string resourceBasePath = CreateResourceBaseDir();
            string localPath = Path.Combine(resourceBasePath, name);
            SaveBytes(data, localPath);
            return localPath;
        }

        public static string SaveBytesToTemp(byte[] data, string extension)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString(), extension));
            SaveBytes(data, tempPath);
            return tempPath;
        }

        public static void SaveBytes(byte[] data, string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch (Exception)
            {

            }
        }

        public static string CreateResourceBaseDir()
        {
            string resourceBasePath = Constant.ResourceDirPath;
            if (!Directory.Exists(resourceBasePath))
            {
                Directory.CreateDirectory(resourceBasePath);
            }
            return resourceBasePath;
        }
    }

    public static class KeyManager
    {
        public static EKey KeyStart
        {
            get
            {
                return ConfigFactory.GetValue(ConfigKey.KeyStart, new EKey("F1"));
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.KeyStart, value);
            }
        }
        public static EKey KeyStop
        {
            get
            {
                return ConfigFactory.GetValue(ConfigKey.KeyStop, new EKey("F1"));
            }
            set
            {
                ConfigFactory.SetValue(ConfigKey.KeyStop, value);
            }
        }

        public static void SetKey(EKey keystart, EKey keystop)
        {
            KeyStart = keystart;
            KeyStop = keystop;
        }


    }
}
