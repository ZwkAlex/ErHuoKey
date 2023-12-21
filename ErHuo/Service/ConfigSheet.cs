using ErHuo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ErHuo.Utilities.HwndUtil;

namespace ErHuo.Service
{
    public class ConfigSheet
    {
        public WindowInfo WindowInfo { get; set; }
        public string Display { get; set; } = "normal";
        public string Mouse { get; set; } = "windows";
        public string Keyboard { get; set; } = "windows";
        public int Mode { get; set; } = 0;
    }
}
