using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Utilities
{
    public static class Constant
    {
        public static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string ConfigFilePath = Path.Combine(BasePath, "config.json");

        public static readonly string ResourceDirPath = Path.Combine(BasePath, "assets");

        public static readonly string AppTtitle = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false).OfType<AssemblyTitleAttribute>().FirstOrDefault().Title;

        public static readonly string Version = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).OfType<AssemblyDescriptionAttribute>().FirstOrDefault().Description;

        public static readonly string AppTip = AppTtitle + " ( " + Version + " ) ";

        public const string LWRegistryKey = "F6C2EA3D-2A5A-4B63-AFDB-5E24BD1D39A0";

        public const string OpRegistryKey = "66B9C175-82F2-45E9-AF86-58AD5DED5ADC";

        public const string FishingNoticeFile = "FishingNotice.bmp";

        public const string FishingReviveFile = "FishingRevive.bmp";

        public const string JX3 = "剑网三";

        public const string WaitButtonTextStart = "开始按键";

        public const string WaitButtonTextBusy = "修改设置中";

        public const string RegisterStateSuccess = "已注册";

        public const string RegisterStateFail = "未注册";

        public const string ConfigClearMessage = "将清除当前所有设置，是否确认。";

        public const string ConfigClearTitle = "清除设置";

        public const string ConfigClearSuccessMessage = "清除成功，软件将重启。";

        public const string ConfigClearFailMessage = "清除失败，请手动清除同目录下文件。";

        public const string RegisterMessage = "检测到插件***未注册***，即将进行插件注册。点击“确认”接受注册，点击“取消”拒绝注册。";

        public const string RegisterTitle = "插件注册";

        public const string CancelRegisterMessage = "拒绝注册，按键将无法启动。";

        public const string RegisterFailMessage = "注册失败";

        public const string UnRegisterMessage = "卸载插件并删除，卸载完成后将造成按键不可用。";

        public const string UnRegisterTitle = "卸载插件";

        public const string UnRegisterSuccess = "卸载插件成功, 手动删除本按键即可。";

        public const string UnRegisterFail = "卸载插件失败，请手动删除注册表及本地lw.dll文件。";

        public const string NormalKeyTabTitle = "基础按键";

        public const string StartStopKeySetWarning = "无效设置，不能设置为左键或右键，并且不能与基础按键中的设置一致。";

        public const string NormalKeyFirstInform = "鼠标移动至目标窗口后按***鼠标中键*** 注意：后台功能对***绝大多数游戏***无效,请使用前台功能。";

        public const string AddKeyEmptyWarning = "请输入要添加的按键。";

        public const string AddKeyFailWarning = "按键添加失败：检查是否已经添加或与开启热键与结束热键冲突。";

        public const string FishingTabTitle = "钓鱼";

        public const string FishingKeySetError = "此设置***无法*** 设为空 或者 与开始热键及停止热键相同";

        public const string FindPointUnfinish = "上一次找点未结束";

        public const string FishingBadConfig = "错误的设置，请进行钓鱼上钩提示点选取";

        public static readonly Dictionary<string, int> LwKeyMode = new Dictionary<string, int>() { { "normal", 0 }, { "windows", 1 } };

        public static readonly Dictionary<string, string> KeyTranslate = new Dictionary<string, string>()
        {
            {"KEY_Q", "Q" },
            {"KEY_W", "W" },
            {"KEY_E", "E" },
            {"KEY_R", "R" },
            {"KEY_T", "T" },
            {"KEY_Y", "Y" },
            {"KEY_U", "U" },
            {"KEY_I", "I" },
            {"KEY_O", "O" },
            {"KEY_P", "P" },
            {"KEY_A", "A" },
            {"KEY_S", "S" },
            {"KEY_D", "D" },
            {"KEY_F", "F" },
            {"KEY_G", "G" },
            {"KEY_H", "H" },
            {"KEY_J", "J" },
            {"KEY_K", "K" },
            {"KEY_L", "L" },
            {"KEY_Z", "Z" },
            {"KEY_X", "X" },
            {"KEY_C", "C" },
            {"KEY_V", "V" },
            {"KEY_B", "B" },
            {"KEY_N", "N" },
            {"KEY_M", "M" },

            {"KEY_1", "1" },
            {"KEY_2", "2" },
            {"KEY_3", "3" },
            {"KEY_4", "4" },
            {"KEY_5", "5" },
            {"KEY_6", "6" },
            {"KEY_7", "7" },
            {"KEY_8", "8" },
            {"KEY_9", "9" },
            {"KEY_0", "0" },
            {"OEM_3", "`" },
            {"OEM_COMMA", "," },
            {"OEM_PERIOD", "." },
            {"OEM_2", "/" },
            {"OEM_1", ";" },
            {"OEM_7", "'" },
            {"OEM_4", "[" },
            {"OEM_6", "]" },
            {"OEM_5", "\\" },
            {"OEM_MINUS", "-" },
            {"OEM_PLUS", "=" },

            {"LBUTTON", "左键" },
            {"RBUTTON", "右键" },
            {"MBUTTON", "中键" },
            {"XBUTTON1", "侧键1" },
            {"XBUTTON2", "侧键2" },
            {"SCROLL_UP", "上滑滚轮" },
            {"SCROLL_DOWN", "下滑滚轮" },
        };

        public static readonly Dictionary<string, string> KeyNameTranslate = KeyTranslate.ToDictionary((i) => i.Value, (i) => i.Key);

    }

    public class ConfigKey
    {
        public const string Volume = "Volume";

        public const string KeyList = "KeyList";

        public const string KeyStart = "KeyStart";

        public const string KeyStop = "KeyStop";

        public const string MinimizeToTray = "MinimizeToTray";

        public const string KeyFishingRelease = "KeyFishingRelease";

        public const string KeyFishingFinish = "KeyFishingFinish";

        public const string FishingRevive = "FishingRevive";

        public const string FishingNoticePoint = "FishingNoticePoint";

        public const string FishingInjuredPoint = "FishingInjuredPoint";

        public const string FishingRevivePoint = "FishingRevivePoint";

        public const string Frequency = "Frequency";

        public const string KeyMode = "KeyMode";

        public const string IsFirstInformFindWindow = "IsFirstInformFindWindow";

        public const string KeyCollect = "KeyCollect";

        public const string InitialStart = "InitialStart";

        public const string ConfigNeverOpen = "ConfigNeverOpen";

        public const string Plugin = "Plugin";

        public const string WaitKeyTimeout = "WaitKeyTimeout";

        public const string DarkTheme = "DarkTheme";
    }

}
