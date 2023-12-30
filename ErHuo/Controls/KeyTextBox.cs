using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using TextBox = System.Windows.Controls.TextBox;
using System.Windows.Input;
using ErHuo.Utilities;
using ErHuo.Models;

namespace ErHuo.Controls
{
    public class KeyTextBox: TextBox
    {
        public KeyTextBox()
        {
            PreviewKeyDown += new KeyEventHandler(TextBox_PreviewKeyDown);
            KeyDown += new KeyEventHandler(TextBox_KeyDown);

            GotMouseCapture += new MouseEventHandler(TextBox_GotMouseCapture);
            IsMouseCapturedChanged += new DependencyPropertyChangedEventHandler(TextBox_IsMouseCaptureWithinChanged);

            GotFocus += new RoutedEventHandler(TextBox_GotFocus);
            LostFocus += new RoutedEventHandler(TextBox_LostFocus);
            IsEnabledChanged += new DependencyPropertyChangedEventHandler(EnabledChanged);
        }

        public void EnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                BorderThickness = new Thickness(1);
            }
            else
            {
                BorderThickness = new Thickness(0);
            }
        }
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Instances.HotKeyViewModel.QueueBusy();
            CaptureMouse();
        }
        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Instances.HotKeyViewModel.DequeueBusy();
        }

        private void TextBox_GotMouseCapture(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }

        private void TextBox_IsMouseCaptureWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SelectAll();
        }

        public void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                string vkey = Enum.GetName(typeof(VK), KeyInterop.VirtualKeyFromKey(Key.Space));
                Text = Constant.KeyTranslate.ContainsKey(vkey) ? Constant.KeyTranslate[vkey] : vkey;
                SelectAll();
                e.Handled = true;
            }
        }
        public void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            string vkey;
            if (e.SystemKey == Key.F10 || e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                vkey = Enum.GetName(typeof(VK), KeyInterop.VirtualKeyFromKey(e.SystemKey));
            }
            else
            {
                vkey = Enum.GetName(typeof(VK), KeyInterop.VirtualKeyFromKey(e.Key));
            }
            Text = Constant.KeyTranslate.ContainsKey(vkey) ? Constant.KeyTranslate[vkey] : vkey;
            SelectAll();
            e.Handled = true;
        }
    }
}
