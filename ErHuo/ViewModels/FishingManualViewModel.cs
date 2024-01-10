using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace ErHuo.ViewModels
{
    public class FishingManualViewModel : Screen
    {
        public int _index = 0;

        public int Index
        {
            get { return _index; }
            set
            {
                SetAndNotify(ref _index, value);
            }
        }
        public void Previous()
        {
            Index = Math.Max(_index - 1, 0);
        }

        public void Next()
        {
            Index = Math.Min(_index + 1, 3);
        }

        public void ManualTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (TabItem t in ((TabControl)sender).Items)
            {
                if (e.AddedItems.Contains(t))
                {
                    t.Visibility = Visibility.Visible;
                }
                else
                {
                    t.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
