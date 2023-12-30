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

            Instances.KeyboardHook.KeyUpEvent -= KeyUpEventHandler;
            Instances.KeyboardHook.KeyUpEvent += KeyUpEventHandler;

            ModifyKey = !CheckKeyInList(_keyStart) || !CheckKeyInList(_keyStop);
            if (!ModifyKey)
            {
                Instances.KeyboardHook.Start();
            }
        }

        public void KeyConfigAction()
        {
            if (!ModifyKey)
            {
                if (!CheckKeyInList(_keyStart) || !CheckKeyInList(_keyStop))
                {
                    ModifyKey = true;
                    Growl.Info(new GrowlInfo() { WaitTime = 2, Message = "无效设置", ShowDateTime = false });
                    KeyStartName = KeyManager.KeyStart.Name;
                    KeyStopName = KeyManager.KeyStop.Name;
                    return;
                }
                KeyManager.SetKey(_keyStart, _keyStop);
                DequeueBusy();
            }
            else
            {
                QueueBusy();
            }
        }

        private void KeyUpEventHandler(object sender, KeyEventArgs e)
        {
            int keyCode = (int)e.KeyCode;
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

        private void Start()
        {
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

        private void Stop()
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


        public bool CheckKeyInList(EKey key)
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
            return true;
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
                Instances.KeyboardHook.Stop();
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
                Instances.KeyboardHook.Start();
            }
        }

    }
}
