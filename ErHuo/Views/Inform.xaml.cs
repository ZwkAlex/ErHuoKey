using System.Windows;

namespace ErHuo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Infrom : Window
    {
        public Infrom()
        {
            InitializeComponent();
        }

        public void CloseInformWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
