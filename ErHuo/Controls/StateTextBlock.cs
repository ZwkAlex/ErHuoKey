using ErHuo.Models;
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
    public class StateTextBlock : TextBlock
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(nameof(State), typeof(object), typeof(StateTextBlock), new PropertyMetadata(false, StatePropertyChanged));
        
        public bool State
        {
            get
            {
                return (bool)GetValue(StateProperty);
            }
            set
            {
                SetValue(StateProperty, value);
            }
        }

        private void StatePropertyChanged(bool state)
        {
            if (state)
            {
                Text = "已激活";
                Foreground = Brushes.Green;
            }
            else
            {
                Text = "未激活";
                Foreground = Brushes.Red;
            }
        }

        private static void StatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StateTextBlock)d).StatePropertyChanged((bool)e.NewValue);
        }
    }
}
