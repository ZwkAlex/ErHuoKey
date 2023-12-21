using ErHuo.Service;
using ErHuo.Utilities;
using Stylet;
using StyletIoC;
using System;
using System.Windows.Controls;



namespace ErHuo.ViewModels
{
    public class MainViewModel : Conductor<Screen>.Collection.OneActive
    {
        private Tab currentTab;
        public HotKeyViewModel HotKeyViewModel { get; set; } 

        public MainViewModel(IContainer container)
        {
            HotKeyViewModel = container.Get<HotKeyViewModel>();
        }
        protected override void OnViewLoaded()
        {
            InitViewModels();
        }

        private void InitViewModels()
        {
            Items.Add(Instances.NormalKeyViewModel);
            Items.Add(Instances.OtherViewModel);

            ActiveItem = Instances.NormalKeyViewModel;
        }
        
        public void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Type currentActivate = ActiveItem?.GetType();
            if(currentActivate != null)
            {
                if (currentActivate == typeof(NormalKeyViewModel))
                {
                    currentTab = Tab.NormalKey;
                }
                else if (currentActivate == typeof(OtherViewModel))
                {
                    currentTab = Tab.OtherKey;
                }
            }
            else
            {
                currentTab = Tab.NormalKey;
            }
            HotKeyViewModel.CurrentTab = currentTab;
        }
    }
}
