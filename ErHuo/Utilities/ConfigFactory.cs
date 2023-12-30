using ErHuo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObservableCollections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ErHuo.Utilities
{
    public static class ConfigFactory
    {
        private static readonly string config_file = Constant.ConfigFilePath;
        private static ObservableDictionary<string, dynamic> config { get; set; } = new ObservableDictionary<string, dynamic>();

        public static Dictionary<string, dynamic> LoadConfigFile()
        {
            Dictionary<string, dynamic> parsed = null;
            if (File.Exists(config_file))
            {
                try
                {
                    parsed = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText(config_file));

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            if (parsed is null)
            {
                parsed = new Dictionary<string, dynamic>();
            }
            foreach (KeyValuePair<string, dynamic> kv in parsed)
            {
                var dictKey = kv.Key;
                var dictValue = kv.Value;
                if (dictKey == nameof(CursorPoint))
                    config.Add(dictKey, dictValue.ToObject<Dictionary<string, CursorPoint>>());
                else if (dictKey == nameof(EKey))
                    config.Add(dictKey, dictValue.ToObject<Dictionary<string, EKey>>());
                else if (dictKey == ConfigKey.KeyList)
                    config.Add(dictKey, dictValue.ToObject<List<KeyEvent>>());
                else if (dictKey == nameof(Plugin))
                    config.Add(dictKey, (Plugin)dictValue);
                else if (dictValue is long)
                    config.Add(dictKey, Convert.ToInt32(dictValue));
                else
                    config.Add(dictKey, dictValue);
            }
            return parsed;
        }

        public static T GetValue<T>(string key, T defaultValue)
        {
            var typeKey = typeof(T).Name;
            if (config.ContainsKey(key))
            {
                return config[key];
            }
            else if (config.ContainsKey(typeKey) && config[typeKey] is Dictionary<string, T> && config[typeKey].ContainsKey(key))
            {
                return config[typeof(T).Name][key];
            }
            SetValue(key, defaultValue);
            return defaultValue;
        }

        public static List<T> GetListValue<T>(string key)
        {
            var hasValue = config.TryGetValue(key, out var obj);
            if (hasValue)
            {
                return obj;
            }
            List<T> emptyValue = new List<T>();
            SetValue(key, emptyValue);
            return emptyValue;

        }

        public static bool SetValue<T>(string key, T value)
        {
            if (value is CursorPoint || value is EKey)
            {
                var typeKey = typeof(T).Name;
                if (!config.ContainsKey(typeKey))
                    config.Add(typeKey, new Dictionary<string, T>());
                if (config[typeKey].ContainsKey(key))
                    config[typeKey][key] = value;
                else
                    config[typeKey].Add(key, value);
            }
            else if (config.ContainsKey(key))
            {
                config[key] = value;
            }
            else
            {
                config.Add(key, value);
            }
            return Save();
        }


        public static bool Save()
        {
            try
            {
                File.WriteAllText(config_file, JsonConvert.SerializeObject(config));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
    }
}
