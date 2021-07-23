using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ErHuo
{
    public class KeyEvent : INotifyPropertyChanged
    {
        private string key;
        private int code;
        private bool activate = false;
        public string Key
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
