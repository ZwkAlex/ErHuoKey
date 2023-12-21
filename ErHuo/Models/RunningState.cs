using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{
    public class RunningState
    {
        private static RunningState _instance;

        private RunningState()
        {
        }

        public static RunningState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RunningState();
                return _instance;
            }
        }

        // values
        // 由初始化设置为 true
        private bool _idle = true;

        public bool Idle
        {
            get => _idle;
            set
            {
                if (_idle == value)
                {
                    return;
                }

                _idle = value;
                OnIdleChanged(value);
            }
        }

        // getters
        public bool GetIdle() => Idle;

        // action
        public void SetIdle(bool idle) => Idle = idle;

        // subscribes
        public event EventHandler<bool> IdleChanged;

        public virtual void OnIdleChanged(bool newIdleValue)
        {
            IdleChanged?.Invoke(this, newIdleValue);
        }

        /// <summary>
        /// 等待状态变为闲置
        /// </summary>
        /// <param name="time">查询间隔</param>
        /// <returns>Task</returns>
        public async Task UntilIdleAsync(int time = 1000)
        {
            while (!GetIdle())
            {
                await Task.Delay(time);
            }
        }
    }
}
