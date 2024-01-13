using ErHuo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{
    public class BusyState
    {
        private static BusyState _instance;

        private BusyState()
        {
        }

        public static BusyState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BusyState();
                return _instance;
            }
        }
        private int _busyLock = 0;
        // values
        // 由初始化设置为 true
        private bool _busy = false;

        public bool Busy
        {
            get => _busy;
            set
            {
                _busy = value;
                OnBusyStateChanged(value);
            }
        }

        // getters
        public bool GetBusy() => Busy;

        // action
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

        // subscribes
        public event EventHandler<bool> BusyStateChanged;

        public virtual void OnBusyStateChanged(bool newRegValue)
        {
            BusyStateChanged?.Invoke(this, newRegValue);
        }

    }
}
