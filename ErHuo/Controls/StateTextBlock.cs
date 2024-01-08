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
        public static readonly DependencyProperty StateValidTextProperty = DependencyProperty.RegisterAttached(nameof(StateValidText), typeof(object), typeof(StateTextBlock), new PropertyMetadata("True", null));

        public static readonly DependencyProperty StateInvalidTextProperty = DependencyProperty.RegisterAttached(nameof(StateInvalidText), typeof(object), typeof(StateTextBlock), new PropertyMetadata("False", null));

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

        public string StateValidText
        {
            get
            {
                return (string)GetValue(StateValidTextProperty);
            }
            set
            {
                SetValue (StateValidTextProperty, value);
            }
        }

        public string StateInvalidText
        {
            get
            {
                return (string)GetValue(StateInvalidTextProperty);
            }
            set
            {
                SetValue (StateInvalidTextProperty, value);
            }
        }

        private void StatePropertyChanged(bool state)
        {
            if (state)
            {
                Text = StateValidText;
            }
            else
            {
                Text = StateInvalidText;
            }
        }

        private static void StatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StateTextBlock)d).StatePropertyChanged((bool)e.NewValue);
        }
    }
}
