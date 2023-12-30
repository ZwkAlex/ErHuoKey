using ErHuo.Utilities;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ErHuo.ViewModels
{
    public class TaskbarViewModel : PropertyChangedBase
    {
        private bool _iconVisible = false;
        public bool IconVisible
        {
            get
            {
                return _iconVisible;
            }
            set
            {
                SetAndNotify(ref _iconVisible, value);
                NotifyOfPropertyChange(nameof(IconVisiblity));
            }
        }

        public string _tipText = Constant.AppTip;
        public string TipText
        {
            get
            {
                return _tipText;
            }
            set
            {
                SetAndNotify(ref _tipText, Constant.AppTip + "-" + value);
            }
        }

        public Visibility IconVisiblity
        {
            get
            {
                return _iconVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void DoubleClickHandler()
        {
            ShowWindowCommand();
        }

        public void ExitCommand()
        {
            Environment.Exit(0);
        }

        public void ShowWindowCommand()
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        public void Hide()
        {
            IconVisible = false;
        }

        public void Show()
        {
            IconVisible = true;
        }
    }
}
