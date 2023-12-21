using ErHuo.Models;
using ErHuo.Service;
using ErHuo.Utilities;
using HandyControl.Controls;
using HandyControl.Tools;
using Newtonsoft.Json.Linq;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeyboardHook = ErHuo.Utilities.KeyboardHook;
using IContainer = StyletIoC.IContainer;
using System.Threading;
using lw;

namespace ErHuo.ViewModels
{
    public class HotKeyViewModel: PropertyChangedBase
    {
        private RunningState runningState;
        private int _keyCodeStart;
        private string _keyStart = KeyManager.KeyStart;
        public static CancellationTokenSource cts;
        public Tab CurrentTab {  get; set; }
        public string KeyStart
        {
            get
            {
                _keyCodeStart = Tool.VKStringtoInt(_keyStart);
                return _keyStart;
            }
            set
            {
                _keyCodeStart = Tool.VKStringtoInt(value);
                SetAndNotify(ref _keyStart, value);
            }
        }
        private int _keyCodeStop;
        private string _keyStop = KeyManager.KeyStop;
        public string KeyStop
        {
            get
            {
                _keyCodeStop = Tool.VKStringtoInt(_keyStop);
                return _keyStop;
            }
            set
            {
                _keyCodeStop = Tool.VKStringtoInt(value);
                _keyStop = value;
            }
        }
        private bool _modify;
        public bool ModifyKey
        {
            get=> _modify;
            set
            {
                if(_modify !=  value)
                {
                    SetAndNotify(ref _modify, value);
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
        private static IContainer _contrainer;
        public HotKeyViewModel(IContainer contrainer)
        {
            _contrainer = contrainer;
            runningState = RunningState.Instance;
            runningState.IdleChanged += RunningState_IdleChanged;
            ModifyKey = !CheckKey(KeyStart) || !CheckKey(KeyStop);
            Instances.KeyboardHook.KeyUpEvent -= KeyUpEventHandler;
            Instances.KeyboardHook.KeyUpEvent += KeyUpEventHandler;
            if (!ModifyKey)
            {
                Instances.KeyboardHook.Start();
            }
        }

        public void KeyConfigAction()
        {
            if (!ModifyKey) 
            {
                if (!CheckKey(KeyStart) || !CheckKey(KeyStop))
                {
                    ModifyKey = true;
                    Growl.Info("无效设置");
                    return;
                }
                KeyManager.SetKey(_keyStart, _keyStop);
                Instances.KeyboardHook.Start();
            }
            else
            {
                Instances.KeyboardHook.Stop();
            }
        }

        private void KeyUpEventHandler(object sender, KeyEventArgs e)
        {
            int keyCode = (int)e.KeyCode;
            if (!_modify)
            {
                if(keyCode == _keyCodeStart)
                {
                    if(_keyCodeStart != _keyCodeStop)
                    {
                        Start();
                    }
                    else
                    {
                        Switch();
                    }
                }
                else if(keyCode == _keyCodeStop)
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
            Idle = false;
            runningState.SetIdle(false);
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

        }
        
        private void StartService(object Token)
        {
            try
            {
                if (CurrentTab == Tab.NormalKey)
                {
                    Instances.NormalKeyViewModel.Start((CancellationToken)Token);
                }
            }
            catch (Exception e)
            {
                if (e is WindowBindingException || e is KeyException)
                {
                    Growl.Warning(e.Message);
                    Stop();
                }
                else
                {
                    throw e;
                }
            }
        }

        private void Stop()
        {
            if(!Idle || !runningState.GetIdle())
            {
                Idle = true;
                runningState.SetIdle(true);
                cts.Cancel();
                cts.Dispose();
                //if (CurrentTab == Tab.NormalKey)
                //{
                //    Instances.NormalKeyViewModel.Stop();
                //}
            }
        }


        public bool CheckKey(String key)
        {
            List<KeyEvent> keylist = ConfigFactory.GetListValue<KeyEvent>("KeyList");
            if (key == String.Empty || key == null)
                return false;
            foreach(KeyEvent k in keylist)
            {
                if(k.Key == KeyStart || k.Key == KeyStop)
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
    }
}
