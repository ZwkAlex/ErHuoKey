using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ErHuo.Views
{
    /// <summary>
    /// customTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class customTextBox : TextBox 
    {
        public customTextBox()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(TextBox_PreviewKeyDown);
            this.KeyDown += new KeyEventHandler(TextBox_KeyDown);
            this.PreviewMouseDown += new MouseButtonEventHandler(TextBox_PreviewMouseDown);
            this.GotFocus += new RoutedEventHandler(TextBox_GotFocus);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(EnabledChanged);
        }
        private void EnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                this.BorderThickness = new Thickness(1); 
            }
            else
            {
                this.BorderThickness = new Thickness(0);
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            e.Handled = true;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Text = Enum.GetName(typeof(VK), KeyInterop.VirtualKeyFromKey(Key.Space));
                SelectAll();
                e.Handled = true;
            }
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.F10 || e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                this.Text = Enum.GetName(typeof(VK), KeyInterop.VirtualKeyFromKey(e.SystemKey));
            }
            else
            {
                this.Text = Enum.GetName(typeof(VK), KeyInterop.VirtualKeyFromKey(e.Key));
            }
            SelectAll();
            e.Handled = true;
        }
    }
}
