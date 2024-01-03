using ErHuo.Models;
using ErHuo.Utilities;
using HandyControl.Controls;
using Stylet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using IContainer = StyletIoC.IContainer;
using System.Threading;
using HandyControl.Data;
using Newtonsoft.Json.Linq;
using System.Windows.Media;
using System.Windows;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace ErHuo.ViewModels
{
    public class HotKeyViewModel : PropertyChangedBase
    {
        private RunningState runningState;
        private EKey _keyStart = KeyManager.KeyStart;
        private string _keyStartName = KeyManager.KeyStart.Name;
        public static CancellationTokenSource cts;
        public Tab CurrentTab { get; set; }
        public string KeyStartName
        {
            get
            {
                return _keyStartName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = _keyStartName;
                }
                else
                {
                    _keyStart = EKey.GetEKeyFromName(value);
                }
                SetAndNotify(ref _keyStartName, value);
            }
        }
        private EKey _keyStop = KeyManager.KeyStop;
        private string _keyStopName = KeyManager.KeyStop.Name;
        public string KeyStopName
        {
            get
            {
                return _keyStopName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = _keyStopName;
                }
                else
                {
                    _keyStop = EKey.GetEKeyFromName(value);
                }
                SetAndNotify(ref _keyStopName, value);
            }
        }
        private bool _modify;
        public bool ModifyKey
        {
            get => _modify;
            set
            {
                SetAndNotify(ref _modify, value);
                if (value)
                {
                    QueueBusy();
                }
                else
                {
                    DequeueBusy();
                }
            }

        }

        private bool _idle = true;
        public bool Idle
        {
            get => _idle;
            set
            {
                SetAndNotify(ref _idle, value);
            }

        }

        public string WaitButtonText
        {
            get
            {
                if (_busy)
                {
                    return Constant.WaitButtonTextBusy;
                }
                else
                {
                    return Constant.WaitButtonTextStart;
                }
            }
        }


        private int _busyLock = 0;

        private bool _busy = false;
        public bool Busy
        {
            get => _busy;
            set
            {
                SetAndNotify(ref _busy, value);
                NotifyOfPropertyChange(nameof(WaitButtonText));
            }
        }

        private static IContainer _container;
        public HotKeyViewModel(IContainer container)
        {
            _container = container;
            runningState = RunningState.Instance;
            runningState.IdleChanged += RunningState_IdleChanged;

            SoundPlayUtil.ChangeVolume(ConfigFactory.GetValue(ConfigKey.Volume, 20));

            Instances.GlobalHook.KeyUpEvent -= KeyUpEventHandler;
            Instances.GlobalHook.KeyUpEvent += KeyUpEventHandler;
            Instances.GlobalHook.MouseUp -= MouseUpEventHandler;
            Instances.GlobalHook.MouseUp += MouseUpEventHandler;
            Instances.GlobalHook.MouseWheel -= MouseWheelEventHandler;
            Instances.GlobalHook.MouseWheel += MouseWheelEventHandler;

            ModifyKey = !IsKeyValid(_keyStart) || !IsKeyValid(_keyStop);
            if (!ModifyKey)
            {
                Instances.GlobalHook.Start();
            }
        }

        public void KeyConfigAction()
        {
            if (!ModifyKey)
            {
                if (!IsKeyValid(_keyStart) || !IsKeyValid(_keyStop))
                {
                    ModifyKey = true;
                    Growl.Info(new GrowlInfo() { WaitTime = 2, Message = Constant.StartStopKeySetWarning, ShowDateTime = false });
                    KeyStartName = KeyManager.KeyStart.Name;
                    KeyStopName = KeyManager.KeyStop.Name;
                }
                else
                {
                    KeyManager.SetKey(_keyStart, _keyStop);
                }
            }
        }

        private void KeyUpEventHandler(object sender, KeyEventArgs e)
        {
            int keyCode = (int)e.KeyCode;
            SwitchByKeyCode(keyCode);
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs e)
        {
            int keyCode;
            if (e.Button == MouseButtons.XButton1)
            {
                keyCode = (int)VK.XBUTTON1;
            }
            else if (e.Button == MouseButtons.XButton2) 
            { 
                keyCode = (int)VK.XBUTTON2;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                keyCode = (int)VK.MBUTTON;
            }
            else
            {
                keyCode = -1;
            }
            SwitchByKeyCode(keyCode);
        }

        private void MouseWheelEventHandler(object sender, MouseEventArgs e)
        {
            int keyCode;
            if (e.Delta / Math.Abs(e.Delta) == 1)
            {
                keyCode = (int)VK.SCROLL_UP;
            }
            else 
            {
                keyCode = (int)VK.SCROLL_DOWN;
            }
            SwitchByKeyCode(keyCode);
        }

        private void SwitchByKeyCode(int keyCode)
        {
            if (!_modify)
            {
                if (keyCode == _keyStart.Code)
                {
                    if (_keyStart.Code != _keyStop.Code)
                    {
                        Start();
                    }
                    else
                    {
                        Switch();
                    }
                }
                else if (keyCode == _keyStop.Code)
                {
                    Stop();
                }
            }
        }

        public void Switch()
        {
            if (runningState.GetIdle())
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        public void Start()
        {
            if (!runningState.GetIdle() || Busy)
                return;
            runningState.SetIdle(false);
            Instances.ConfigDrawerViewModel.Off();
            cts = new CancellationTokenSource();
            CancellationToken Token = cts.Token;
            if (false)
            {
                Task task = Task.Run(() =>
                {
                    StartService(Token);
                },
                Token);
            }
            else
            {
                Thread t = new Thread(new ParameterizedThreadStart(StartService));
                t.SetApartmentState(ApartmentState.STA);
                t.Start(Token);
            }
            SoundPlayUtil.PlayStartSound();
        }

        private void StartService(object Token)
        {
            try
            {
                if (CurrentTab == Tab.NormalKey)
                {
                    Instances.NormalKeyViewModel.Start((CancellationToken)Token);
                }
                else if (CurrentTab == Tab.Fishing)
                {
                    Instances.FishingViewModel.Start((CancellationToken)Token);
                }
            }
            catch (Exception e)
            {
                if (e is WindowBindingException || e is KeyException || e is ServiceConfigException)
                {
                    Growl.Warning(e.Message);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            if (!runningState.GetIdle())
            {
                runningState.SetIdle(true);
                cts.Cancel();
                cts.Dispose();
                if (CurrentTab == Tab.NormalKey)
                {
                    Instances.NormalKeyViewModel.Stop();
                }
                else
                {
                    Instances.FishingViewModel.Stop();
                }
                SoundPlayUtil.PlayStopSound();
            }
        }


        public bool IsKeyValid(EKey key)
        {
            List<KeyEvent> keylist = ConfigFactory.GetListValue<KeyEvent>(ConfigKey.KeyList);
            if (key == null)
                return false;
            foreach (KeyEvent k in keylist)
            {
                if (k.IsSame(key))
                {
                    return false;
                }
            }
            return key.Code != (int)VK.LBUTTON && key.Code != (int)VK.RBUTTON;
        }

        private void RunningState_IdleChanged(object sender, bool e)
        {
            Idle = e;
        }

        public void QueueBusy()
        {
            _busyLock += 1;
            if (_busyLock == 1)
            {
                Busy = true;
                Instances.GlobalHook.Stop();
            }
        }

        public void DequeueBusy()
        {
            _busyLock -= 1;
            if (_busyLock < 0)
            {
                _busyLock = 0;
            }
            if (_busyLock == 0)
            {
                Busy = false;
                Instances.GlobalHook.Start();
            }
        }

    }
}
