using ErHuo.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ErHuo
{
    public class KeyEvent : EKey, INotifyPropertyChanged
    {

        private bool _activate = false;
        public bool Activate
        {
            get => _activate;
            set { _activate = value; OnPropertyChanged(); }
        }

        public KeyEvent() : base()
        {

        }
        public KeyEvent(EKey ekey) : base(ekey.Key, ekey.Code, ekey.Name)
        {

        }

        public KeyEvent(string key, int code = -1, bool activate = false) : base(key, code)
        {
            Activate = activate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] String property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
