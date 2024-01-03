using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MouseWheelEventArgs = System.Windows.Input.MouseWheelEventArgs;

namespace ErHuo.Utilities
{
    /// <summary>
    /// 键盘钩子    
    /// </summary>
    /// 


    public class GlobalHook
    {
        public event KeyEventHandler KeyDownEvent;
        public event KeyPressEventHandler KeyPressEvent;
        public event KeyEventHandler KeyUpEvent;

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseWheel;

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        static int hKeyboardHook = 0; //声明键盘钩子处理的初始值
        static int hMouseHook = 0;
        //值在Microsoft SDK的Winuser.h里查询

        public const int WH_MOUSE_LL = 14;

        public const int WH_KEYBOARD_LL = 13;   //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13
        HookProc KeyboardHookProcedure; //声明KeyboardHookProcedure作为HookProc类型
        HookProc MouseHookProcedure;
        //键盘结构
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode;  //定一个虚拟键码。该代码必须有一个价值的范围1至254
            public int scanCode; // 指定的硬件扫描码的关键
            public int flags;  // 键标志
            public int time; // 指定的时间戳记的这个讯息
            public int dwExtraInfo; // 指定额外信息相关的信息
        }


        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt; // 鼠标位置
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        /// <summary>
        /// 鼠标位置结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public UIntPtr dwExtraInfo;
        }


        //使用此功能，安装了一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


        //调用此函数卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);


        //使用此功能，通过信息钩子继续下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        // 取得当前线程编号（线程钩子需要用到）
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        public void Start()
        {
            // 安装键盘钩子
            if (hKeyboardHook == 0)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
                //hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                //************************************
                //键盘线程钩子
                //SetWindowsHookEx( 2,KeyboardHookProcedure, IntPtr.Zero, GetCurrentThreadId());//指定要监听的线程idGetCurrentThreadId(),
                //键盘全局钩子,需要引用空间(using System.Reflection;)
                //SetWindowsHookEx( 13,MouseHookProcedure,Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),0);
                //
                //关于SetWindowsHookEx (int idHook, HookProc lpfn, IntPtr hInstance, int threadId)函数将钩子加入到钩子链表中，说明一下四个参数：
                //idHook 钩子类型，即确定钩子监听何种消息，上面的代码中设为2，即监听键盘消息并且是线程钩子，如果是全局钩子监听键盘消息应设为13，
                //线程钩子监听鼠标消息设为7，全局钩子监听鼠标消息设为14。lpfn 钩子子程的地址指针。如果dwThreadId参数为0 或是一个由别的进程创建的
                //线程的标识，lpfn必须指向DLL中的钩子子程。 除此以外，lpfn可以指向当前进程的一段钩子子程代码。钩子函数的入口地址，当钩子钩到任何
                //消息后便调用这个函数。hInstance应用程序实例的句柄。标识包含lpfn所指的子程的DLL。如果threadId 标识当前进程创建的一个线程，而且子
                //程代码位于当前进程，hInstance必须为NULL。可以很简单的设定其为本应用程序的实例句柄。threaded 与安装的钩子子程相关联的线程的标识符
                //如果为0，钩子子程与所有的线程关联，即为全局钩子
                //************************************
                //如果SetWindowsHookEx失败
                if (hKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("安装键盘钩子失败");
                }
            }

            //if (hMouseHook == 0)
            //{
            //    MouseHookProcedure = new HookProc(MouseHookProc);
            //    hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProcedure, GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
            //    if (hMouseHook == 0)
            //    {
            //        Stop();
            //        throw new Exception("安装鼠标钩子失败");
            //    }
            //}
        }
        public void Stop()
        {
            if (hKeyboardHook != 0)
            {
                bool retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
                if (!retKeyboard) throw new Exception("卸载钩子失败！");
            }

            if (hMouseHook != 0)
            {
                bool retMouse = UnhookWindowsHookEx(hMouseHook);
                hMouseHook = 0;
                if (!retMouse) throw new Exception("卸载钩子失败！");
            }
        }
        //ToAscii职能的转换指定的虚拟键码和键盘状态的相应字符或字符
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, //[in] 指定虚拟关键代码进行翻译。
                                         int uScanCode, // [in] 指定的硬件扫描码的关键须翻译成英文。高阶位的这个值设定的关键，如果是（不压）
                                         byte[] lpbKeyState, // [in] 指针，以256字节数组，包含当前键盘的状态。每个元素（字节）的数组包含状态的一个关键。如果高阶位的字节是一套，关键是下跌（按下）。在低比特，如果设置表明，关键是对切换。在此功能，只有肘位的CAPS LOCK键是相关的。在切换状态的NUM个锁和滚动锁定键被忽略。
                                         byte[] lpwTransKey, // [out] 指针的缓冲区收到翻译字符或字符。
                                         int fuState); // [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise.

        //获取按键的状态
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);


        private const int WM_KEYDOWN = 0x100;//KEYDOWN
        private const int WM_KEYUP = 0x101;//KEYUP
        private const int WM_SYSKEYDOWN = 0x104;//SYSKEYDOWN
        private const int WM_SYSKEYUP = 0x105;//SYSKEYUP


        public const int WM_MOUSEMOVE = 0x200; // 鼠标移动
        public const int WM_LBUTTONDOWN = 0x201;// 鼠标左键按下
        public const int WM_RBUTTONDOWN = 0x204;// 鼠标右键按下
        public const int WM_MBUTTONDOWN = 0x207;// 鼠标中键按下
        public const int WM_XBUTTONDOWN = 0x020B;// 鼠标侧键按下
        public const int WM_LBUTTONUP = 0x202;// 鼠标左键抬起
        public const int WM_RBUTTONUP = 0x205;// 鼠标右键抬起
        public const int WM_MBUTTONUP = 0x208;// 鼠标中键抬起
        public const int WM_XBUTTONUP = 0x020C;// 鼠标侧键抬起
        public const int WM_MOUSEWHEEL = 0x020A;// 鼠标侧键抬起

        private int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // 侦听键盘事件
            if (nCode >= 0 && (KeyDownEvent != null || KeyUpEvent != null || KeyPressEvent != null))
            {
                int msg = (int)wParam;
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                // raise KeyDown
                if (KeyDownEvent != null && (msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyDownEvent(this, e);
                }

                //键盘按下
                else if (KeyPressEvent != null && msg == WM_KEYDOWN)
                {
                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);

                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode, MyKeyboardHookStruct.scanCode, keyState, inBuffer, MyKeyboardHookStruct.flags) == 1)
                    {
                        KeyPressEventArgs e = new KeyPressEventArgs((char)inBuffer[0]);
                        KeyPressEvent(this, e);
                    }
                }

                // 键盘抬起
                else if (KeyUpEvent != null && (msg == WM_KEYUP || msg == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyUpEvent(this, e);
                }

            }
            //如果返回1，则结束消息，这个消息到此为止，不再传递。
            //如果返回0或调用CallNextHookEx函数则消息出了这个钩子继续往下传递，也就是传给消息真正的接受者
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {   
            int msg = (int)wParam;
            if (nCode < 0 || msg == WM_MOUSEMOVE) { return 0; }
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseEventArgs e;

                switch (msg)
                {
                    case WM_LBUTTONDOWN:
                        e = new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseDown?.Invoke(this, e);
                        break;
                    case WM_RBUTTONDOWN:
                        e = new MouseEventArgs(MouseButtons.Right, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseDown?.Invoke(this, e);
                        break;
                    case WM_MBUTTONDOWN:
                        e = new MouseEventArgs(MouseButtons.Middle, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseDown?.Invoke(this, e);
                        break;
                    case WM_XBUTTONDOWN:
                        e = new MouseEventArgs(MouseButtons.XButton1, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseDown?.Invoke(this, e);
                        break;
                    case WM_LBUTTONUP:
                        e = new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseUp?.Invoke(this, e);
                        break;
                    case WM_RBUTTONUP:
                        e = new MouseEventArgs(MouseButtons.Right, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseUp?.Invoke(this, e);
                        break;
                    case WM_MBUTTONUP:
                        e = new MouseEventArgs(MouseButtons.Middle, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseUp?.Invoke(this, e);
                        break;
                    case WM_XBUTTONUP:
                        e = new MouseEventArgs(MouseButtons.XButton1, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseUp?.Invoke(this, e);
                        break;
                    case WM_MOUSEWHEEL:
                        int delta = ((short)((uint)hookStruct.mouseData >> 16));
                        e = new MouseEventArgs(MouseButtons.None, 1, hookStruct.pt.x, hookStruct.pt.y, delta);
                        MouseWheel?.Invoke(this, e);
                        break;
                    default:
                        break;
                }
            }

            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }

        //~GlobalHook()
        //{
        //    Stop();
        //}
    }

}