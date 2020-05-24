using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo
{
    public class KeyEvent : INotifyPropertyChanged
    {
        private String key;
        private int code;
        private bool activate = false;
        public String Key
        {
            get { return key; }
            set
            {
                this.key = value;
                OnPropertyChanged();
            }
        }

        public int Code
        {
            get { return code; }
            set
            {
                this.code = value;
                OnPropertyChanged();
            }
        }

        public bool Activate
        {
            get { return activate; }
            set
            {
                this.activate = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]String property = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
