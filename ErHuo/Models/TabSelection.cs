using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{
    public class TabSelection
    {
        public static TabSelection _instance;

        private TabSelection() { }

        public static TabSelection Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TabSelection();
                return _instance;
            }
        }

        public event EventHandler<Tab> CurrentTabChanged;

        private Tab _tab = Tab.NormalKey;

        public Tab CurrentTab
        {
            get => _tab;
            set
            {
                _tab = value;
                OnCurrentTabChanged(value);
            }
        }

        public Tab GetCurrentTab() => CurrentTab;

        // action
        public void SetCurrentTab(Tab _tab) => CurrentTab = _tab;

        public virtual void OnCurrentTabChanged(Tab selectedTab)
        {
            CurrentTabChanged?.Invoke(this, selectedTab);
        }

    }
}
