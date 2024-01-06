using ErHuo.Models;
using ErHuo.Services;
using ErHuo.Utilities;
using ErHuo.Views;
using HandyControl.Controls;
using Stylet;
using StyletIoC;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Window = HandyControl.Controls.Window;
using TabControl = System.Windows.Controls.TabControl;
using IContainer = StyletIoC.IContainer;
using System.Threading.Tasks;
using System.ComponentModel;
using HandyControl.Themes;
using System.Windows.Media;
using ErHuo.Plugins;

namespace ErHuo.ViewModels
{
    public class MainViewModel : Conductor<Screen>.Collection.OneActive
    {
        private Tab currentTab;
        public HotKeyViewModel HotKeyViewModel { get; set; }
        public NoClientAreaViewModel NoClientAreaViewModel { get; set; }
        public ConfigDrawerViewModel ConfigDrawerViewModel { get; set; }
        public TaskbarViewModel TaskbarViewModel { get; set; }

        private bool _drawerSwitch = false;
        public bool DrawerSwitch
        {
            get { return _drawerSwitch; }
            set { SetAndNotify(ref _drawerSwitch, value); }
        }
        public MainViewModel(IContainer container)
        {
            HotKeyViewModel = Instances.HotKeyViewModel;
            NoClientAreaViewModel = Instances.NoClientAreaViewModel;
            ConfigDrawerViewModel = Instances.ConfigDrawerViewModel;
            TaskbarViewModel = Instances.TaskbarViewModel;
            IRegister.Instance.TryRegister();
        }
        protected override void OnViewLoaded()
        {
            InitViewModels();
        }

        private void InitViewModels()
        {
            Items.Add(Instances.NormalKeyViewModel);
            Items.Add(Instances.FishingViewModel);

            ActiveItem = Instances.NormalKeyViewModel;
        }

        public void Test()
        {
            DrawerSwitch = true;
        }

        public void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActiveItem != null)
            {
                if (ActiveItem is NormalKeyViewModel)
                {
                    currentTab = Tab.NormalKey;
                    JX3WindowFinder.Stop();
                }
                else if (ActiveItem is FishingViewModel)
                {
                    currentTab = Tab.Fishing;
                    ((FrameworkElement)sender).Focus();
                    JX3WindowFinder.Start();
                }
            }
            else
            {
                currentTab = Tab.NormalKey;
            }
            HotKeyViewModel.CurrentTab = currentTab;
        }

        public void WindowMouseDown(FrameworkElement focusHolder)
        {
            focusHolder.Focus();
        }

        public void WindowVisiblityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                Instances.TaskbarViewModel.Show();
            }
            else
            {
                Instances.TaskbarViewModel.Hide();
            }
        }


        public void WindowStateChanged(object sender, EventArgs e)
        {
            if (((Window)sender).WindowState == WindowState.Minimized && !RunningState.Instance.GetIdle())
            {
                if (ConfigFactory.GetValue(ConfigKey.MinimizeToTray, false))
                {
                    Application.Current.MainWindow.Hide();
                }
            }
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Task.Run(() =>
            {
                Instances.GlobalHook.Stop();
                Instances.HotKeyViewModel.Stop();
                Environment.Exit(0);
            });
        }
    }
}
