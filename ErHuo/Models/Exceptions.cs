using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{

    public class KeyException : Exception
    {
        public KeyException(string message) : base(message) { }
    }

    public class WindowBindingException : Exception
    {
        public WindowBindingException(string message) : base(message) { }
    }

    public class ServiceConfigException : Exception
    {
        public ServiceConfigException(string message) : base(message) { }
    }
}
