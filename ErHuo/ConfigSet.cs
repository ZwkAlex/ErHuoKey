using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace ErHuo
{
    public class ConfigSet : INotifyPropertyChanged
    {
        private ObservableCollection<KeyEvent> config_key_list;
        private int config_key_start;
        private int config_key_stop;
        private int config_key_pause;
        private bool config_is_use_same_key;
        private double config_volume;
        private bool config_first_time;
        private int config_frequency;
        private bool config_switch;
        private int config_driver;
        private int config_key_mode;

        private int config_key_fish_release;
        private int config_key_fish_collect;
        private bool config_fish_revive;
        private Point config_fish_point1;
        private Point config_fish_point2;
        private Point config_fish_point3;

        private Point config_performer_point1;
        private Point config_performer_point2;

        public ConfigSet()
        {
            List<string> pre_config_key_list = new List<string>();
            List<bool> pre_config_key_activate_list = new List<bool>();
            try
            {
                config_frequency = int.Parse(ConfigurationManager.AppSettings["Frequency"]);
                config_first_time = bool.Parse(ConfigurationManager.AppSettings["FristTime"]);
                if(!ConfigurationManager.AppSettings["KeyValue"].Equals(""))
                {
                    pre_config_key_list = ConfigurationManager.AppSettings["KeyValue"].Split(',').ToList();
                    pre_config_key_activate_list = StringList2BoolList(ConfigurationManager.AppSettings["IsActivate"].Split(',').ToList());
                }         
                config_volume = double.Parse(ConfigurationManager.AppSettings["Volume"]);
                config_key_start = int.Parse(ConfigurationManager.AppSettings["StartKey"]);
                config_key_stop = int.Parse(ConfigurationManager.AppSettings["StopKey"]);
                config_key_pause = int.Parse(ConfigurationManager.AppSettings["PauseKey"]);
                config_is_use_same_key = bool.Parse(ConfigurationManager.AppSettings["UseSameKey"]);
                config_switch = bool.Parse(ConfigurationManager.AppSettings["Switch"]);
                config_driver = int.Parse(ConfigurationManager.AppSettings["Driver"]);
                config_key_mode = int.Parse(ConfigurationManager.AppSettings["KeyMode"]);
                config_key_fish_release = int.Parse(ConfigurationManager.AppSettings["FishReleaseKey"]);
                config_key_fish_collect = int.Parse(ConfigurationManager.AppSettings["FishCollectKey"]);
                config_fish_revive = bool.Parse(ConfigurationManager.AppSettings["FishRevive"]);
                config_fish_point1 = StringToPoint(ConfigurationManager.AppSettings["FishPoint1"]);
                config_fish_point2 = StringToPoint(ConfigurationManager.AppSettings["FishPoint2"]);
                config_fish_point3 = StringToPoint(ConfigurationManager.AppSettings["FishPoint3"]);
                config_performer_point1 = StringToPoint(ConfigurationManager.AppSettings["PerformerPoint1"]);
                config_performer_point2 = StringToPoint(ConfigurationManager.AppSettings["PerformerPoint2"]);
            }
            catch
            {
                config_first_time = true;
                config_volume = 0.2;
                config_key_start = 120;
                config_key_stop = 123;
                config_key_pause = 121;
                config_is_use_same_key = false;
                config_frequency = 100;
                config_switch = true;
                config_driver = 1;
                config_key_mode = 0;
                config_key_fish_release = 49;
                config_key_fish_collect = 50;
                config_fish_revive = false;
                config_fish_point1 = new Point(0, 0);
                config_fish_point2 = new Point(0, 0);
                config_fish_point3 = new Point(0, 0);
                config_performer_point1 = new Point(0, 0);
                config_performer_point2 = new Point(0, 0);
            }
            config_key_list = new ObservableCollection<KeyEvent>();
            for (var item = 0; item < pre_config_key_list.Count; item++)
            {
                KeyEvent new_key = new KeyEvent
                {
                    Activate = pre_config_key_activate_list[item],
                    Code = int.Parse(pre_config_key_list[item]),
                    Key = Enum.GetName(typeof(VK), int.Parse(pre_config_key_list[item]))
                };
                new_key.PropertyChanged += KeyEventPropertyChanged;
                config_key_list.Add(new_key);
            }
            config_key_list.CollectionChanged += ContentCollectionChanged;
        }

        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (KeyEvent item in e.OldItems)
                {    
                    //Removed items
                    item.PropertyChanged -= KeyEventPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (KeyEvent item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += KeyEventPropertyChanged;
                }
            }
            ConfigUtil.SaveConfig();
        }
        public void KeyEventPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ConfigUtil.SaveConfig();
        }


        public ObservableCollection<KeyEvent> Config_Key_List
        {
            get { return config_key_list; }
            set
            {
                this.config_key_list = value;
                OnPropertyChanged();
            }
        }

        public int Config_Key_Start
        {
            get { return config_key_start; }
            set
            {
                if( config_key_start != value) { 
                    if (CheckKeyinList(value))
                    {
                        MessageBox.Show("按键列表中已有此键 请先删除左方列表对应按键");
                    }
                    else { 
                        this.config_key_start = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public int Config_Key_Stop
        {
            get { return config_key_stop; }
            set
            {
                if (config_key_stop != value)
                {
                    if (CheckKeyinList(value))
                    {
                        MessageBox.Show("按键列表中已有此键 请先删除左方列表对应按键");
                    }
                    else
                    {
                        this.config_key_stop = value;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public int Config_Key_Pause
        {
            get { return config_key_pause; }
            set
            {
                if (config_key_pause != value)
                {
                    if (CheckKeyinList(value))
                    {
                        MessageBox.Show("按键列表中已有此键 请先删除左方列表对应按键");
                    }
                    else
                    {
                        this.config_key_pause = value;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public bool Config_is_Use_Same_Key
        {
            get { return config_is_use_same_key; }
            set
            {
                
                this.config_is_use_same_key = value;
                OnPropertyChanged();
            }
        }

        public double Config_Volume
        {
            get { return config_volume; }
            set
            {
                this.config_volume = value;
                OnPropertyChanged();
            }
        }

        public bool Config_FristTime
        {
            get { return config_first_time; }
            set
            {
                this.config_first_time = value;
                OnPropertyChanged();
            }
        }

        public int Config_Frequency
        {
            get { return config_frequency; }
            set
            {
                this.config_frequency = value;
                OnPropertyChanged();
            }
        }

        public bool Config_Switch
        {
            get { return config_switch; }
            set
            {
                if (!value) 
                {
                    MessageBox.Show("此开关为防误开关 \n需要为开启状态才可以使按键通过快捷键开启关闭 \n本按键不含联网功能 \n需要快捷键开启请勾选");
                }
                this.config_switch = value;
                OnPropertyChanged();
            }
        }

        public int Config_Driver
        {
            get { return config_driver; }
            set
            {
                this.config_driver = value;
                OnPropertyChanged();
            }
        }

        public int Config_Key_Mode
        {
            get { return config_key_mode; }
            set
            {
                this.config_key_mode = value;
                OnPropertyChanged();
            }
        }

        public int Config_Key_Fish_Release
        {
            get { return config_key_fish_release; }
            set
            {
                if (config_key_fish_release != value)
                {
                    if (CheckKeyinUse(value))
                    {
                        MessageBox.Show("按键有冲突");
                    }
                    else
                    {
                        this.config_key_fish_release = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public int Config_Key_Fish_Collect
        {
            get { return config_key_fish_collect; }
            set
            {
                if (config_key_fish_collect != value)
                {
                    if (CheckKeyinUse(value))
                    {
                        MessageBox.Show("按键有冲突");
                    }
                    else
                    {
                        this.config_key_fish_collect = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public bool Config_Fish_Revive
        {
            get { return config_fish_revive; }
            set
            {
                this.config_fish_revive = value;
                OnPropertyChanged();
            }
        }

         public Point Config_Fish_Point1
        {
            get {  return config_fish_point1; }
            set
            {
                this.config_fish_point1 = value;
                OnPropertyChanged();
            }
        }
        public Point Config_Fish_Point2
        {
            get { return config_fish_point2; }
            set
            {
                this.config_fish_point2 = value;
                OnPropertyChanged();
            }
        }
        public Point Config_Fish_Point3
        {
            get { return config_fish_point3; }
            set
            {
                this.config_fish_point3 = value;
                OnPropertyChanged();
            }
        }

        public Point Config_Perfromer_Point1
        {
            get { return config_performer_point1; }
            set
            {
                this.config_performer_point1 = value;
                OnPropertyChanged();
            }
        }
        public Point Config_Perfromer_Point2
        {
            get { return config_performer_point2; }
            set
            {
                this.config_performer_point2 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]String property = null)
        {
            ConfigUtil.SaveConfig();
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        private List<bool> StringList2BoolList(List<string> string_list)
        {
            List<bool> temp = new List<bool>();
            foreach (var str in string_list)
            {
                if (!str.Equals(""))
                {
                    temp.Add(bool.Parse(str));
                }
               
            }
            return temp;
        }

        private bool CheckKeyinList(int key)
        {
            foreach (KeyEvent k in config_key_list)
            {
                if(k.Code == key)
                {
                    return true;
                }
            }
            if(key == config_key_start|| key  ==config_key_stop || key == config_key_pause)
            {
                return true;
            }
            return false;
        }

        private bool CheckKeyinUse(int key)
        {
            if (key == config_key_start || key == config_key_stop || key == config_key_pause)
            {
                return true;
            }
            return false;
        }

        private static Point StringToPoint(string s)
        {
            if(s != null){
                string[] temp = s.Split(',');
                return new Point(int.Parse(temp[0]), int.Parse(temp[1]));
            }
            else
            {
                return new Point(0, 0);
            }
            
        }

    }
    public static class ConfigUtil
    {
        private static ConfigSet config = new ConfigSet();
        public static ConfigSet Config { get { return config; }}

        public static void SaveConfig()
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            List<int> key_list = new List<int>();
            List<bool> key_activate_list = new List<bool>();
            foreach (var key in config.Config_Key_List)
            {
                key_list.Add(key.Code);
                key_activate_list.Add(key.Activate);
            } 
            SettingSave(cfa, "KeyValue", string.Join(",", key_list));
            SettingSave(cfa, "IsActivate", string.Join(",", key_activate_list));
            SettingSave(cfa, "Volume",config.Config_Volume.ToString());
            SettingSave(cfa, "StartKey", config.Config_Key_Start.ToString());
            SettingSave(cfa, "StopKey", config.Config_Key_Stop.ToString());
            SettingSave(cfa, "PauseKey", config.Config_Key_Pause.ToString());
            SettingSave(cfa, "UseSameKey", config.Config_is_Use_Same_Key.ToString());
            SettingSave(cfa, "FristTime", config.Config_FristTime.ToString());
            SettingSave(cfa, "Frequency", config.Config_Frequency.ToString());
            SettingSave(cfa, "Switch", config.Config_Switch.ToString());
            SettingSave(cfa, "Driver", config.Config_Driver.ToString());
            SettingSave(cfa, "KeyMode", config.Config_Key_Mode.ToString());
            SettingSave(cfa, "FishReleaseKey", config.Config_Key_Fish_Release.ToString());
            SettingSave(cfa, "FishCollectKey", config.Config_Key_Fish_Collect.ToString());
            SettingSave(cfa, "FishRevive", config.Config_Fish_Revive.ToString());
            SettingSave(cfa, "FishPoint1", config.Config_Fish_Point1.X+ "," + config.Config_Fish_Point1.Y);
            SettingSave(cfa, "FishPoint2", config.Config_Fish_Point2.X + "," + config.Config_Fish_Point2.Y);
            SettingSave(cfa, "FishPoint3", config.Config_Fish_Point3.X + "," + config.Config_Fish_Point3.Y);
            SettingSave(cfa, "PerformerPoint1", config.Config_Perfromer_Point1.X + "," + config.Config_Perfromer_Point1.Y);
            SettingSave(cfa, "PerformerPoint2", config.Config_Perfromer_Point2.X + "," + config.Config_Perfromer_Point2.Y);
            cfa.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
        private static void SettingSave(Configuration cfa, string name, string value)
        {
            if(cfa.AppSettings.Settings[name] != null)
            {
                cfa.AppSettings.Settings[name].Value = value;
            }
            else
            {
                cfa.AppSettings.Settings.Add(name, value);
            }
        }


    }
}
