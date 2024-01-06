using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{
    public class RegisterState
    {
        private static RegisterState _instance;

        private RegisterState()
        {
        }

        public static RegisterState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RegisterState();
                return _instance;
            }
        }

        // values
        // 由初始化设置为 true
        private bool _reg = false;

        public bool Reg
        {
            get => _reg;
            set
            {
                _reg = value;
                OnRegisterStateChanged(value);
            }
        }

        // getters
        public bool GetReg() => Reg;

        // action
        public void SetReg(bool reg) => Reg = reg;

        // subscribes
        public event EventHandler<bool> RegisterStateChanged;

        public virtual void OnRegisterStateChanged(bool newRegValue)
        {
            RegisterStateChanged?.Invoke(this, newRegValue);
        }

        /// <summary>
        /// 等待状态变为闲置
        /// </summary>
        /// <param name="time">查询间隔</param>
        /// <returns>Task</returns>
        public async Task UntilRegAsync(int time = 1000)
        {
            while (!GetReg())
            {
                await Task.Delay(time);
            }
        }
    }
}
