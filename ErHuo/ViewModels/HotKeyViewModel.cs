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
        private readonly RunningState runningState;
        private readonly BusyState busyState;
        public static CancellationTokenSource cts;
        private Thread t;
        private Tab CurrentTab;

        private EKey _keyStart = KeyManager.KeyStart;
        private string _keyStartName = KeyManager.KeyStart.Name;
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

        public Visibility KeyStopVisiblity
        {
            get
            {
                return PressToActivate ? Visibility.Hidden : Visibility.Visible;
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
                    busyState.QueueBusy();
                }
                else
                {
                    busyState.DequeueBusy();
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

        public bool PressToActivate
        {
            get 
            {
                return ConfigFactory.GetValue<bool>(ConfigKey.PressToActivate);
            }
            set
            {
                if (value && (_keyStart.Code == (int)VK.SCROLL_UP || _keyStart.Code == (int)VK.SCROLL_DOWN))
                {
                    value = false;
                    Growl.Info(Constant.PressToActivateError);
                }
                ConfigFactory.SetValue(ConfigKey.PressToActivate, value);
                NotifyOfPropertyChange(nameof(KeyStopVisiblity));
            }
        }

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
            busyState = BusyState.Instance;
            runningState.IdleChanged += RunningStateChangedHandler;
            TabSelection.Instance.CurrentTabChanged += TabChangedHandler;
            busyState.BusyStateChanged += BusyStateChangedHandler;

            SoundPlayUtil.ChangeVolume(ConfigFactory.GetValue<int>(ConfigKey.Volume));

            Instances.GlobalHook.KeyDownEvent -= KeyDownEventHandler;
            Instances.GlobalHook.KeyDownEvent += KeyDownEventHandler;
            Instances.GlobalHook.MouseDown -= MouseDownEventHandler;
            Instances.GlobalHook.MouseDown += MouseDownEventHandler;

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

        public void TabChangedHandler(object sender, Tab e)
        {
            CurrentTab = e;
        }

        private void KeyUpEventHandler(object sender, KeyEventArgs e)
        {
            int keyCode = (int)e.KeyCode;
            SwitchByKeyCode(keyCode, PressType.Up);
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs e)
        {
            int keyCode;
            bool result = Constant.MouseButtonMapper.TryGetValue(e.Button, out keyCode);
            SwitchByKeyCode(keyCode, PressType.Up);
        }

        private void KeyDownEventHandler(object sender, KeyEventArgs e)
        {
            int keyCode = (int)e.KeyCode;
            SwitchByKeyCode(keyCode);
        }

        private void MouseDownEventHandler(object sender, MouseEventArgs e)
        {
            int keyCode;
            bool result = Constant.MouseButtonMapper.TryGetValue(e.Button, out keyCode);
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

        private void SwitchByKeyCode(int keyCode, PressType pressType = PressType.Down)
        {
            if (!_modify)
            {
                if (PressToActivate)
                {
                    if (keyCode == _keyStart.Code)
                    {
                        if (pressType == PressType.Down)
                        {
                            Start();
                        }
                        else
                        {
                            Stop();
                        }
                    }
                }
                else
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
            if (runningState.GetIdle() && !Busy)
            {
                runningState.SetIdle(false);
            }
            
        }

        private void Service(object Token)
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
                if (CurrentTab == Tab.NormalKey)
                {
                    Instances.NormalKeyViewModel.Stop();
                }
                else
                {
                    Instances.FishingViewModel.Stop();
                }
                runningState.SetIdle(true);
            }
        }

        public void Stop()
        {
            runningState.SetIdle(true);
        }

        public bool IsKeyValid(EKey key)
        {
            List<KeyEvent> keylist = ConfigFactory.GetValue<List<KeyEvent>>(ConfigKey.KeyList);
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

        private void RunningStateChangedHandler(object sender, bool e)
        {
            Idle = e;
            if (!e)
            {
                SoundPlayUtil.PlayStartSound();
                Instances.ConfigDrawerViewModel.Off();
                cts = new CancellationTokenSource();
                t = new Thread(new ParameterizedThreadStart(Service));
                t.SetApartmentState(ApartmentState.STA);
                t.Start(cts.Token);
            }
            else
            {
                SoundPlayUtil.PlayStopSound();
                if (cts != null && cts.Token.CanBeCanceled)
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = null;
                    t.Abort();
                }
            }
        }

        private void BusyStateChangedHandler(object sender, bool e)
        {
            Busy = e;
        }

        public void ShowTip(string param)
        {
            if(param == "PressToActivate")
            {
                Growl.Info(Constant.PressToActivateTip);
            }
        }

    }
}
