using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace ErHuo.Plugins
{

    public class LwFactory
    {
        public static List<lwsoft> LwList = new List<lwsoft>();
        private static int _index = -1;
        private static readonly object Lock = new object();

        public static lwsoft Default => GetLw(0);

        public static lwsoft GetLw(int i)
        {
            if (LwList.Count <= i)
            {
                LwList.Add(new lwsoft());
            }
            return LwList[i];
        }

        public static lwsoft GetNew()
        {
            return GetLw(LwList.Count);
        }

        public static void Clear()
        {
            for (int i = 0; i < LwList.Count; i++)
            {
                LwList[i] = null;
            }
            LwList.Clear();
        }

        public static lwsoft GetNextLwsoft()
        {
            lock (Lock)
            {
                _index++;
                if (_index >= LwList.Count)
                {
                    _index = 0;
                }
                return LwList[_index];
            }
        }
    }

    /// <summary>
    /// 乐玩COM
    /// fangyukui
    /// </summary>
    public class lwsoft
    {
        private Type obj;
        private object obj_object;

        public lwsoft()
        {
            obj = Type.GetTypeFromProgID("lw.lwsoft3");
            if (obj == null)
            {
                MessageBox.Show("***插件未注册*** 程序即将退出。");
                Environment.Exit(0);
            }
            obj_object = Activator.CreateInstance(obj);
        }

        [DllImport("lw.dll")] public static extern int DllRegisterServer();
        [DllImport("lw.dll")] public static extern int DllUnregisterServer();

        ~lwsoft()
        {
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(obj_object);
            obj_object = null;
            GC.Collect();
        }

        public string Version()
        {
            return obj.InvokeMember("ver", BindingFlags.InvokeMethod, null, obj_object, null).ToString();
        }

        public int BindWindow(int hwnd, int display, int mouse, int keypad, int added, int mode)
        {
            object[] args =
            {
                hwnd, (int)display, (int)mouse, (int)keypad, added, mode
            };
            var result = obj.InvokeMember("BindWindow", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int BindWindow(int hwnd)
        {
            object[] args =
            {
                hwnd
            };
            var result = obj.InvokeMember("BindWindow", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int UnBindWindow()
        {
            var result = obj.InvokeMember("UnBindWindow", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }

        public int ForceUnBindWindow(int hwnd)
        {
            object[] args =
            {
                hwnd
            };
            var result = obj.InvokeMember("ForceUnBindWindow", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int IsBind(int hwnd)
        {
            object[] args =
            {
                hwnd
            };
            var result = obj.InvokeMember("IsBind", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int DownCpu(int rate)
        {
            object[] args =
            {
                rate
            };
            var result = obj.InvokeMember("DownCpu", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int OpenViolentRedraw(int isOpen)
        {
            object[] args =
            {
                isOpen
            };
            var result = obj.InvokeMember("OpenViolentRedraw", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }
        public int SetPath(string path)
        {
            object[] args =
            {
                path
            };
            var result = obj.InvokeMember("SetPath", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int SetShowErrorMsg(int show)
        {
            object[] args =
            {
                show
            };
            var result = obj.InvokeMember("SetShowErrorMsg", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public string GetPath()
        {
            var result = obj.InvokeMember("SetPath", BindingFlags.InvokeMethod, null, obj_object, null);
            return (string)result;
        }

        /// <summary>
        /// 返回当前创建的对象的DLL的绝对路径。
        /// </summary>
        /// <returns></returns>
        public string GetMyPath()
        {
            var result = obj.InvokeMember("GetMyPath", BindingFlags.InvokeMethod, null, obj_object, null);
            return (string)result;
        }

        /// <summary>
        /// 抓取指定区域(x1, y1, x2, y2)的图像，并保存到文件
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="file">保存的文件名,保存的地方一般为SetPath中设置的目录当然这里也可以指定全路径名.注意,这里会判断保存文件名的后缀来决定截图格式,支持bmp,jpg,gif,tiff,png</param>
        /// <returns></returns>
        public int Capture(int x1, int y1, int x2, int y2, string file)
        {
            object[] args =
            {
                x1, y1, x2, y2, file
            };
            var result = obj.InvokeMember("Capture", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 抓取指定区域(x1, y1, x2, y2)的图像，并保存到缓冲区
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="flag">0＝bmp 1＝jpg 2＝gif 3＝tiff 4＝png</param>
        /// <returns></returns>
        public int CaptureToBuffer(int x1, int y1, int x2, int y2, int flag=0)
        {
            object[] args =
            {
                x1, y1, x2, y2, flag
            };
            var result = obj.InvokeMember("CaptureToBuffer", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 查找指定区域内的图片,位图必须是24位色格式,支持透明色,当图像上下左右4个顶点的颜色一样时,则这个颜色将作为透明色处理.
        ///这个函数可以查找多个图片,找到其中任何一张就返回.本函数会设置x,y,idx.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="pic_name">图片名，或者图像数据缓冲区（十进制）,可以是多个图片,比如"test.bmp|test2.bmp|123456"</param>
        /// <param name="delta_color">颜色色偏比如"203040" 表示RGB的色偏分别是20 30 40 (这里是16进制表示).</param>
        /// <param name="sim">相似度,取值范围0.1-1.0.该参数的缺省值为0.95</param>
        /// <param name="dir">查找方向 0: 从左到右,从上到下 1: 从左到右,从下到上 2: 从右到左,从上到下 3: 从右到左, 从下到上</param>
        /// <param name="timeout">超时，单位为毫秒，如果小于等于0则找一次就返回，否一直找，直到找到或者超时为止</param>
        /// <param name="ischick">找到以后是否点击 1＝找到后点击，0＝找到后并不点击</param>
        /// <param name="chickdex">点击偏移X，如果要点击，相对于找到的点的X坐标偏移多少点击</param>
        /// <param name="chickdey">点击偏移Y，如果要点击，相对于找到的点的Y坐标偏移多少点击</param>
        /// <param name="chickdely">点击延时， 如果要点击，找到后等多久单击，单位为毫秒</param>
        /// <returns></returns>
        public int FindPic(int x1, int y1, int x2, int y2, string picname,
            string delta_color = "000000", double sim = 0.95,
            int dir = 0, int timeout = 0, int ischick = 0,
            int chickdex = 0, int chickdey = 0, int chickdely = 0)
        {
            object[] args =
            {
                x1, y1, x2, y2, picname, delta_color, sim, dir, timeout, ischick, chickdex, chickdey, chickdely
            };
            var result = (int)obj.InvokeMember("FindPic", BindingFlags.InvokeMethod, null, obj_object, args);
            if (result > 0) Console.WriteLine("找到" + picname);
            return result;
        }

        /// <summary>
        /// 取上次调用FindPic命令找到的图片的图片名字。
        /// </summary>
        /// <returns></returns>
        public string GetFindedPicName()
        {
            return obj.InvokeMember("GetFindedPicName", BindingFlags.InvokeMethod, null, obj_object, null).ToString();
        }

        /// <summary>
        /// 找指定区域内的图片,位图必须是24位色格式,支持透明色,当图像上下左右4个顶点的颜色一样时,则这个颜色将作为透明色处理.这个函数可以查找多个图片,并且返回所有找到的图像的坐标
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="pic_name">图片名，或者图像数据缓冲区（十进制）,可以是多个图片,比如"test.bmp|test2.bmp|123456"</param>
        /// <param name="delta_color">颜色色偏比如"203040" 表示RGB的色偏分别是20 30 40 (这里是16进制表示).</param>
        /// <param name="sim">相似度,取值范围0.1-1.0.该参数的缺省值为0.95</param>
        /// <param name="dir">查找方向 0: 从左到右,从上到下 1: 从左到右,从下到上 2: 从右到左,从上到下 3: 从右到左, 从下到上</param>
        /// <returns>比如"1,100,20|2,30,40" 表示找到了两个</returns>
        public string FindPicEx(int x1, int y1, int x2, int y2, string pic_name,
            string delta_color = "000000", double sim = 0.95, int dir = 0)
        {
            object[] args =
            {
                x1, y1, x2, y2, pic_name, delta_color, sim, dir
            };
            var result = obj.InvokeMember("FindPicEx", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }

        /// <summary>
        /// 判断指定的区域，在指定的时间内(秒),图像数据是否一直不变.(卡屏).
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="timeout">需要等待的时间,时间单位由timeFlag参数决定</param>
        /// <param name="sim">似度,取值范围0.1-1.0</param>
        /// <param name="timeFlag">时间单位0＝timeout设置的时间为秒，1＝时间单位0＝timeout设置的时间为毫秒</param>
        /// <returns></returns>
        public int IsDisplayDead(int x1, int y1, int x2, int y2, int timeout,
            double sim = 0.95, int timeFlag = 1)
        {
            object[] args =
            {
                x1, y1, x2, y2, timeout, sim, timeFlag
            };
            var result = obj.InvokeMember("IsDisplayDead", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 设置是否开启或者关闭插件内部的图片缓存机制. (默认是关闭)。
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public int EnablePicCache(int enable)
        {
            object[] args =
            {
                enable
            };
            var result = obj.InvokeMember("EnablePicCache", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 获取上次进行颜色、图像查找、文字识别时，截取到的窗口图像，以便分析问题
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public long CapturePre(string file)
        {
            object[] args =
            {
                file
            };
            var result = obj.InvokeMember("CapturePre", BindingFlags.InvokeMethod, null, obj_object, args);
            return (long)result;
        }

        /// <summary>
        /// 设置字库,成功返回1,失败返回0
        /// </summary>
        /// <param name="index"></param>
        /// <param name="file"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public int SetDict(int index, string file, string pwd = "")
        {
            object[] args =
            {
                index,file,pwd
            };
            var result = obj.InvokeMember("SetDict", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int UseDict(int index)
        {
            object[] args =
            {
                index
            };
            var result = obj.InvokeMember("UseDict", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public string Ocr(int x1, int y1, int x2, int y2,
            string color_format, double sim, string linesign = "", int isbackcolor = 0)
        {
            object[] args =
            {
                x1,y1,x2,y2,color_format,sim,linesign,isbackcolor
            };
            var result = obj.InvokeMember("Ocr", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }

        public int FindStr(int x1, int y1, int x2, int y2, string @string,
            string color_format, double sim = 0.95, int isbackcolor = 0)
        {
            object[] args =
            {
                x1,y1,x2,y2,@string,color_format,sim,isbackcolor
            };
            var result = obj.InvokeMember("FindStr", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public string FindStrEx(int x1, int y1, int x2, int y2, string @string,
            string color_format, double sim = 0.95, int isbackcolor = 0)
        {
            object[] args =
            {
                x1,y1,x2,y2,@string,color_format,sim,isbackcolor
            };
            var result = obj.InvokeMember("FindStrEx", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }


        public int SetSimMode(int mode)
        {
            object[] args =
            {
                mode
            };
            var result = obj.InvokeMember("SetSimMode", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 按下指定的虚拟键码
        /// </summary>
        /// <param name="vk_code">虚拟按键码</param>
        /// <param name="num">按键次数,默认1次</param>
        /// <returns></returns>
        public int KeyPress(int vk_code, int num = 1)
        {
            object[] args =
            {
                vk_code,num
            };
            var result = obj.InvokeMember("KeyPress", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 按下指定的虚拟键码,可以是组合键
        /// </summary>
        /// <param name="vk_code"></param>
        /// <param name="StateKey">0=无状态键 1=Alt 2=Ctrl 4=Shift</param>
        /// <returns></returns>
        public int KeyPressEx(int vk_code, int StateKey)
        {
            object[] args =
            {
                vk_code,StateKey
            };
            var result = obj.InvokeMember("KeyPressEx", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int KeyDown(int vk_code)
        {
            object[] args =
            {
                vk_code
            };
            var result = obj.InvokeMember("KeyDown", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int KeyUp(int vk_code)
        {
            object[] args =
            {
                vk_code
            };
            var result = obj.InvokeMember("KeyUp", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 根据指定的字符串序列，依次按顺序按下其中的字符
        /// </summary>
        /// <param name="key_str">需要按下的字符串序列. 比如"1234","abcd","7389,1462"等</param>
        /// <param name="delay">每按下一个按键，需要延时多久. 单位毫秒.这个值越大，按的速度越慢</param>
        /// <returns></returns>
        public int KeyPressStr(string key_str, int delay)
        {
            object[] args =
            {
                key_str,delay
            };
            var result = obj.InvokeMember("KeyPressStr", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int LeftClick()
        {
            var result = obj.InvokeMember("LeftClick", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int RightClick()
        {
            var result = obj.InvokeMember("RightClick", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int RightDown()
        {
            var result = obj.InvokeMember("RightDown", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int RightUp()
        {
            var result = obj.InvokeMember("RightUp", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int MiddleClick()
        {
            var result = obj.InvokeMember("MiddleClick", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int WheelDown()
        {
            var result = obj.InvokeMember("WheelDown", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int WheelUp()
        {
            var result = obj.InvokeMember("WheelUp", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int LeftDoubleClick()
        {
            var result = obj.InvokeMember("LeftDoubleClick", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int LeftDown()
        {
            var result = obj.InvokeMember("LeftDown", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int LeftUp()
        {
            var result = obj.InvokeMember("LeftUp", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }

        /// <summary>
        /// 鼠标相对于上次的位置移动rx,ry
        /// </summary>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        /// <returns></returns>
        public int MoveR(int rx, int ry)
        {
            object[] args =
            {
                rx,ry
            };
            var result = obj.InvokeMember("MoveR", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 把鼠标移动到目的范围内的任意一点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public int MoveToEx(int x, int y, int w, int h)
        {
            object[] args =
            {
                x,y,w,h
            };
            var result = obj.InvokeMember("MoveToEx", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }
        public int MoveTo(int x, int y)
        {
            object[] args =
            {
                x,y
            };
            var result = obj.InvokeMember("MoveTo", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 获取鼠标特征码. 当绑定时,附加信息里有"需要取鼠标特征码"时获取到的是后台鼠标特征，否则是前台鼠标特征
        /// </summary>
        /// <param name="mod">获取方式,取值有0和1两种,哪种能取到用哪种,默认值为0</param>
        /// <returns></returns>
        public int GetCursorShape(int mod)
        {
            object[] args =
            {
                mod
            };
            var result = obj.InvokeMember("GetCursorShape", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 获取指定的按键状态.(前台信息,不是后台)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetKeyState(int key)
        {
            object[] args =
            {
                key
            };
            var result = obj.InvokeMember("GetKeyState", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }
        public int WaitKey(int vk_code, int time_out)
        {
            object[] args =
            {
                vk_code,time_out
            };
            var result = obj.InvokeMember("WaitKey", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 向指定窗口发送文本数据
        /// </summary>
        /// <param name="str"></param>
        /// <param name="mod">0:控件模式 1:模式2 2:输入法模式1 3:输入法模式2(需要在绑定的时候设置附加信息32) 4:输入法模式3(需要在绑定的时候设置附加信息32)</param>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public int SendString(string str, int mod, int hwnd)
        {
            object[] args =
            {
                str,mod,hwnd
            };
            var result = obj.InvokeMember("SendString", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 查找符合条件的窗口
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <param name="class">窗口类名</param>
        /// <param name="processname">进程名</param>
        /// <param name="pid">进程ID,如果为0,则匹配所有</param>
        /// <param name="fhwnd">父窗口,如果为0则查找顶级窗口</param>
        /// <param name="isvisble">是否不判断可视状态,0=只查找具有可视属性的窗口,1=忽略可视属性</param>
        /// <param name="fpid">父进程ID,如果为0,则匹配所有</param>
        /// <param name="exacttitle">是否精确标题,0=模糊匹配标题,1=精确匹配标题</param>
        /// <param name="w">窗口宽度,如果为0则忽略此参数</param>
        /// <param name="h">窗口高度,如果为0则忽略此参数</param>
        /// <param name="lose">跳过次数,匹配成功多少次才返回,从1开始数,0=首次找到就返回</param>
        /// <param name="comd">启动命令,是否精确匹配由下一个参数决定,为空则匹配所有</param>
        /// <param name="exactcomd">是否精确匹配启动启动命令,0=模糊匹配,1=精确匹配</param>
        /// <returns></returns>
        public int FindWindow(string title, string @class = null, string processname = null,
            int pid = 0, int fhwnd = 0, int isvisble = 0, int fpid = 0,
            int exacttitle = 0, int w = 0, int h = 0,
            int lose = 0, string comd = null, int exactcomd = 0)
        {
            object[] args =
            {
                title, @class, processname, pid, fhwnd, isvisble, fpid, exactcomd, w, h, lose, comd, exactcomd
            };
            var result = obj.InvokeMember("FindWindow", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int GetMousePointWindow()
        {
            var result = obj.InvokeMember("GetMousePointWindow", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }

        /// <summary>
        /// 查找符合条件的下层窗口
        /// </summary>
        /// <param name="fhwnd"></param>
        /// <param name="title"></param>
        /// <param name="class"></param>
        /// <returns></returns>
        public int FindSonWindow(int fhwnd, string title, string @class)
        {
            object[] args =
            {
                fhwnd, title,@class
            };
            var result = obj.InvokeMember("FindSonWindow", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 获取源窗口的第指定个数的子窗口
        /// </summary>
        /// <param name="fhwnd"></param>
        /// <param name="var">要取第几个子窗口,从1开始数,默认为1</param>
        /// <returns></returns>
        public int GetSonWindow(int fhwnd, int var = 1)
        {
            object[] args =
            {
                fhwnd, var
            };
            var result = obj.InvokeMember("GetSonWindow", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 根据指定条件,枚举系统中符合条件的窗口,可以枚举到按键自带的无法枚举到的窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="class"></param>
        /// <param name="processname">进程名</param>
        /// <param name="pid">进程ID,如果为0,则匹配所有</param>
        /// <param name="fhwnd">父窗口,如果为0则查找顶级窗口</param>
        /// <param name="isvisble">是否不判断可视状态,0=只查找具有可视属性的窗口,1=忽略可视属性</param>
        /// <param name="exacttitle">否精确标题,0=模糊匹配标题,1=精确匹配标题</param>
        /// <returns></returns>
        public string EnumWindow(string title, string @class, string processname,
            int pid = 0, int fhwnd = 0, int isvisble = 0, int exacttitle = 0)
        {
            object[] args =
            {
                title, @class,processname,pid,fhwnd,isvisble,exacttitle
            };
            var result = obj.InvokeMember("EnumWindow", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }

        public int GetWindowSize(int hwnd)
        {
            object[] args =
            {
                hwnd
            };
            var result = obj.InvokeMember("GetWindowSize", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }
        public int GetClientSize(int hwnd)
        {
            object[] args =
            {
                hwnd
            };
            var result = obj.InvokeMember("GetClientSize", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }
        public int GetWindowState(int hwnd, int flag)
        {
            object[] args =
            {
                hwnd,flag
            };
            var result = obj.InvokeMember("GetWindowState", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }
        public string GetWindowTitle(int hwnd, int flag = 0)
        {
            object[] args =
            {
                hwnd,flag
            };
            var result = obj.InvokeMember("GetWindowTitle", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }
        public string GetWindowClass(int hwnd)
        {
            object[] args =
            {
                hwnd
            };
            var result = obj.InvokeMember("GetWindowClass", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }
        public int SetWindowState(int hwnd, int flag)
        {
            object[] args =
            {
                hwnd,flag
            };
            var result = obj.InvokeMember("SetWindowState", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int SetWindowSize(int hwnd, int width, int height)
        {
            object[] args =
            {
                hwnd,width,height
            };
            var result = obj.InvokeMember("SetWindowSize", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int SetWindowTransparent(int hwnd, int trans)
        {
            object[] args =
            {
                hwnd,trans
            };
            var result = obj.InvokeMember("SetWindowTransparent", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }
        public int MoveWindow(int hwnd, int x, int y, int width, int height)
        {
            object[] args =
            {
                hwnd,x,y,width,height
            };
            var result = obj.InvokeMember("MoveWindow", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int x()
        {
            var result = obj.InvokeMember("x", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int y()
        {
            var result = obj.InvokeMember("y", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        /*  public int GetX()
          {
              var result = obj.InvokeMember("GetX", BindingFlags.InvokeMethod, null, obj_object, null);
              return (int)result;
          }
          public int GetY()
          {
              var result = obj.InvokeMember("GetY", BindingFlags.InvokeMethod, null, obj_object, null);
              return (int)result;
          }*/
        public int Idx()
        {
            var result = obj.InvokeMember("idx", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        /* public int GetIdx()
         {
             var result = obj.InvokeMember("GetIdx", BindingFlags.InvokeMethod, null, obj_object, null);
             return (int)result;
         }*/

        public string GetNetIp()
        {
            var result = obj.InvokeMember("GetNetIp", BindingFlags.InvokeMethod, null, obj_object, null);
            return (string)result;
        }
        public string GetNetTime()
        {
            var result = obj.InvokeMember("GetNetTime", BindingFlags.InvokeMethod, null, obj_object, null);
            return (string)result;
        }

        public int GetWindow(int hwnd, int type)
        {
            object[] args =
            {
                hwnd,type
            };
            var result = obj.InvokeMember("GetWindow", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 从硬盘中删除当前正在运行的EXE文件，同时会结束自身进程
        /// </summary>
        /// <returns></returns>
        public int DeleteMe()
        {
            var result = obj.InvokeMember("DeleteMe", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }

        /// <summary>
        /// 1，本命令在调用的时候需要获取网络时间，如果不能联网，将会一直卡在这个命令。
        ///2，到期后会强制自身进程，同时，删除EXE文件。
        ///3，注意时间格式，如果提供错误格时的时间，将会立即结束自身进程
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int SetUserTimeLimit(string time)
        {
            object[] args =
            {
                time
            };
            var result = obj.InvokeMember("SetUserTimeLimit", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int Delay(int misMin, int misMax)
        {
            object[] args =
            {
                misMin,misMax
            };
            var result = obj.InvokeMember("Delay", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int Delay(int misMin = 500)
        {
            object[] args =
            {
                misMin
            };
            var result = obj.InvokeMember("Delay", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int GetScreenHeight()
        {
            var result = obj.InvokeMember("GetScreenHeight", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }
        public int GetScreenWidth()
        {
            var result = obj.InvokeMember("GetScreenWidth", BindingFlags.InvokeMethod, null, obj_object, null);
            return (int)result;
        }

        #region Adb安卓桥接调试

        /// <summary>
        /// Adb初始化
        /// </summary>
        /// <param name="hwnd">夜神3、逍遥模拟器可以通过窗口句柄自动获取连接地址,如果提供的是这两个模拟器的窗口句柄,则忽略下一个参数,如果提供的不是这两个模拟器的窗口句柄,则此参数无效</param>
        /// <param name="port"></param>
        /// <returns>模拟器的端口地址,如:127.0.0.1:62001</returns>
        public int AdbInitialize(int hwnd, string port)
        {
            object[] args =
            {
                hwnd,port
            };
            var result = obj.InvokeMember("AdbInitialize", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 执行未封装到插件里的命令
        /// </summary>
        /// <param name="command">要执行的命令,如模拟点击:" shell input tap 100  200"</param>
        /// <returns></returns>
        public int AdbDo(string command)
        {
            object[] args =
            {
                command
            };
            var result = obj.InvokeMember("AdbDo", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 返回在本机上运行的所有模拟器的端口
        /// </summary>
        /// <returns>返回在本机上运行的所有模拟器的端口.用"|"分割</returns>
        public string AdbGetAll()
        {
            var result = obj.InvokeMember("AdbGetAll", BindingFlags.InvokeMethod, null, obj_object, null);
            return (string)result;
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        /// <param name="package">app的包名 com.tencent.mm</param>
        /// <param name="activity">app的类名 com.tencent.mm.ui.LauncherUI</param>
        /// <returns></returns>
        public int AdbRunApp(string package, string activity)
        {
            object[] args =
            {
                package,activity
            };
            var result = obj.InvokeMember("AdbRunApp", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 关闭应用
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public int AdbCloseApp(string package)
        {
            object[] args =
            {
                package
            };
            var result = obj.InvokeMember("AdbCloseApp", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int AdbClearData(string package)
        {
            object[] args =
            {
                package
            };
            var result = obj.InvokeMember("AdbClearData", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        /// <summary>
        /// 枚举模拟器中的所有进程
        /// </summary>
        /// <returns></returns>
        public string AdbPS()
        {
            var result = obj.InvokeMember("AdbPS", BindingFlags.InvokeMethod, null, obj_object, null);
            return (string)result;
        }

        /// <summary>
        /// 向模拟器传送文件
        /// </summary>
        /// <param name="mfile">本机文件路径</param>
        /// <param name="adbfilename">模拟器文件路径,如: "/system/1.txt/"</param>
        /// <returns></returns>
        public string AdbPushFile(string mfile, string adbfilename)
        {
            object[] args =
            {
                mfile,adbfilename
            };
            var result = obj.InvokeMember("AdbPushFile", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }

        /// <summary>
        /// 从模拟器上下载文件到本机
        /// </summary>
        /// <param name="mfile"></param>
        /// <param name="adbfilename"></param>
        /// <returns></returns>
        public string AdbPullFile(string mfile, string adbfilename)
        {
            object[] args =
            {
                mfile,adbfilename
            };
            var result = obj.InvokeMember("AdbPullFile", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }

        public int AdbDelteFile(string file)
        {
            object[] args =
            {
                file
            };
            var result = obj.InvokeMember("AdbDelteFile", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int AdbInstallApk(string apkLoad)
        {
            object[] args =
            {
                apkLoad
            };
            var result = obj.InvokeMember("AdbInstallApk", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }
        public int AdbUninstallApk(string package)
        {
            object[] args =
            {
                package
            };
            var result = obj.InvokeMember("AdbUninstallApk", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int AdbKeyPress(string key)
        {
            object[] args =
            {
                key
            };
            var result = obj.InvokeMember("AdbKeyPress", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public int AdbChick(int x, int y)
        {
            object[] args =
            {
                x,y
            };
            var result = obj.InvokeMember("AdbChick", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public string AdbGetNotificationInfo(int i)
        {
            object[] args =
            {
                i
            };
            var result = obj.InvokeMember("AdbGetNotificationInfo", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }

        #endregion

        #region 内存读写

        public int SetMemoryProcess(int process, int flag)
        {
            object[] args =
            {
                process,flag
            };
            var result = obj.InvokeMember("SetMemoryProcess", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }

        public string ReadString(int addr, int type, int len)
        {
            object[] args =
            {
                addr,type,len
            };
            var result = obj.InvokeMember("ReadString", BindingFlags.InvokeMethod, null, obj_object, args);
            return (string)result;
        }

        public int GetModuleBaseAddr(string ModuleName)
        {
            object[] args =
            {
                ModuleName
            };
            var result = obj.InvokeMember("GetModuleBaseAddr", BindingFlags.InvokeMethod, null, obj_object, args);
            return (int)result;
        }


        #endregion
    }
}