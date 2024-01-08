using ErHuo.Models;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ErHuo.Controls
{
    public class JX3DetectTextBlock: TextBlock
    {

        public static readonly DependencyProperty JX3WindowProperty = DependencyProperty.RegisterAttached(nameof(JX3Window), typeof(object), typeof(JX3DetectTextBlock), new PropertyMetadata(new WindowInfo(), JX3WindowPropertyChanged));

        public WindowInfo JX3Window
        {
            get
            {
                return (WindowInfo)GetValue(JX3WindowProperty);
            }
            set
            {
                SetValue(JX3WindowProperty, value);
            }
        }

        private void JX3WindowPropertyChanged(WindowInfo jx3)
        {
            if (jx3.IsValid())
            {
                Text = "已启动-" + jx3.szWindowName;
                if (Text.Length >= 15)
                {
                    Text = Text.Substring(0, 15) + "...";
                }
                Foreground = Brushes.Green;
            }
            else
            {
                Text = "未启动";
                Foreground = Brushes.Red;
            }
        }

        private static void JX3WindowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((JX3DetectTextBlock)d).JX3WindowPropertyChanged((WindowInfo)e.NewValue);
        }
    }
}
