using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ErHuo
{
    public class KeyEvent : INotifyPropertyChanged
    {
        public string Key { get; set; }
        public int Code { get; set; }

        private bool _activate = false;
        public bool Activate
        {
            get => _activate; 
            set { _activate = value; OnPropertyChanged(); }
        } 

        public KeyEvent(string key, int code =-1, bool activate=false)
        {
            Key = key;
            if (code == -1)
                Code = (int)Enum.Parse(typeof(VK), key);
            else
                Code = code;
            Activate = activate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] String property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
