using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using TextBox = HandyControl.Controls.TextBox;
using System.Windows.Input;
using ErHuo.Utilities;
using ErHuo.Models;
using System.Windows.Media;
using HandyControl.Controls;

namespace ErHuo.Controls
{
    public class KeyTextBox: TextBox
    {
        private bool startEdit = false;
        public KeyTextBox()
        {
            CaretBrush = Brushes.Transparent;
            PreviewKeyDown += new KeyEventHandler(TextBox_PreviewKeyDown);
            KeyDown += new KeyEventHandler(TextBox_KeyDown);
            PreviewMouseWheel += new MouseWheelEventHandler(TextBox_PreviewMouseWheel);

            PreviewMouseDown += new MouseButtonEventHandler(TextBox_PreviewMouseDown);

            GotFocus += new RoutedEventHandler(TextBox_GotFocus);
            LostFocus += new RoutedEventHandler(TextBox_LostFocus);
        }


        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Instances.HotKeyViewModel.QueueBusy();
        }
        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Instances.HotKeyViewModel.DequeueBusy();
            startEdit = false;
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!startEdit)
            {
                startEdit = true;
                return;
            }
            VK vkey;
            if (e.ChangedButton == MouseButton.Left)
            {
                vkey = VK.LBUTTON;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                vkey = VK.RBUTTON;
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                vkey = VK.MBUTTON;
            }
            else if (e.ChangedButton == MouseButton.XButton1)
            {
                vkey = VK.XBUTTON1;
            }
            else if (e.ChangedButton == MouseButton.XButton2)
            {
                vkey = VK.XBUTTON2;
            }
            else
            {
                throw new NotImplementedException();
            }
            string vkeyName = vkey.ToString();
            Text = KeyTranslate(vkeyName);
        }


        private void TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (startEdit)
            {
                VK vkey;
                if (e.Delta / Math.Abs(e.Delta) == 1)
                {
                    vkey = VK.SCROLL_UP;
                }
                else
                {
                    vkey = VK.SCROLL_DOWN;
                }
                string vkeyName = vkey.ToString();
                Text = KeyTranslate(vkeyName);
            }
        }


        public void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                string vkeyName = Enum.GetName(typeof(VK), KeyInterop.VirtualKeyFromKey(Key.Space));
                Text = KeyTranslate(vkeyName);
                e.Handled = true;
            }
        }
        public void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            string vkeyName;
            if (e.SystemKey == Key.F10 || e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                vkeyName = Enum.GetName(typeof(VK), KeyInterop.VirtualKeyFromKey(e.SystemKey));
            }
            else
            {
                vkeyName = Enum.GetName(typeof(VK), KeyInterop.VirtualKeyFromKey(e.Key));
            }
            Text = KeyTranslate(vkeyName);
            e.Handled = true;
        }

        private string KeyTranslate(string vkeyName)
        {
           return Constant.KeyTranslate.ContainsKey(vkeyName) ? Constant.KeyTranslate[vkeyName] : vkeyName;
        }
    }
}
