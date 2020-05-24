using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ErHuo
{
    /// <summary>
    /// KeyControl.xaml 的交互逻辑
    /// </summary>
    public partial class KeyControl : UserControl
    {
        public KeyControl()
        {
            InitializeComponent();
            startkey.PreviewKeyDown += StartKeyCheck;
            stopkey.PreviewKeyDown += StopKeyCheck;
        }
        private void StartKeyCheck(object sender, KeyEventArgs e)
        {
            if ((int)e.Key == ConfigUtil.Config.Config_Key_Stop)
            {
                MessageBox.Show("不要将启动按键与停止按键设置相同，请使用 “同按键设置”");
                e.Handled = true;
            }
        }

        private void StopKeyCheck(object sender, KeyEventArgs e)
        {
            if ((int)e.Key == ConfigUtil.Config.Config_Key_Start)
            {
                MessageBox.Show("不要将启动按键与停止按键设置相同，请使用 “同按键设置”");
                e.Handled = true;
            }
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
