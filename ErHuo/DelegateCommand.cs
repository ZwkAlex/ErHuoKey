using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ErHuo
{
    class DelegateCommand:ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action ExecuteActionWithoutParameter { get; set; }
        public Action<string> ExecuteActionStringParameter { get; set; }
        public Action<object> ExecuteAction { get; set; }
        public Func<object, bool> CanExecuteFunc { get; set; }

        public bool CanExecute(object parameter)
        {
            if(this.CanExecuteFunc == null)
            {
                return true;
            }
            return this.CanExecuteFunc(parameter);
        }

       public void Execute(object parameter)
        {
            if (ExecuteAction != null) { 
                this.ExecuteAction(parameter);
            }else if (ExecuteActionStringParameter!= null)
            {
                this.ExecuteActionStringParameter((string)parameter);
            }
            else if (ExecuteActionStringParameter == null)
            {
                this.ExecuteActionWithoutParameter();
            }
        }
    }
}
