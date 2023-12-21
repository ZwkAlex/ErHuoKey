using Newtonsoft.Json;
using ObservableCollections;
using System;
using System.Collections.Generic;
using System.IO;

namespace ErHuo.Utilities
{
    public static class ConfigFactory
    {
        private static readonly string config_file = Path.Combine(Environment.CurrentDirectory, "config.json");
        public static ObservableDictionary<string, dynamic> config { get; set; } = new ObservableDictionary<string, dynamic>();

        public static ObservableDictionary<string, dynamic> LoadConfigFile()
        {
            ObservableDictionary<string, dynamic> parsed = null;
            if (File.Exists(config_file))
            {
                try
                {
                    parsed = JsonConvert.DeserializeObject<ObservableDictionary<string, dynamic>>(File.ReadAllText(config_file));
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            if (parsed is null)
            {
                parsed = new ObservableDictionary<string, dynamic>();
            }

            config = parsed;
            return parsed;
        }

        public static T GetValue<T>(string key, T defaultValue)
        {
            var hasValue = config.TryGetValue(key, out var obj);
            if (hasValue)
            {
                if (obj.GetType().Name == "Int64")
                    obj = Convert.ToInt32(obj);
                return obj;
            }
            SetValue(key, defaultValue);
            return defaultValue;
        }

        public static List<T> GetListValue<T>(string key)
        {
            var hasValue = config.TryGetValue(key, out var obj);
            if (hasValue)
            {
                if (obj.GetType().Name == "JArray")
                    return obj.ToObject<List<T>>();
                else
                    return obj;
            }
            List<T> emptyValue = new List<T>();
            SetValue(key, emptyValue);
            return emptyValue;
            
        }

        public static bool SetValue<T>(string key, T value)
        {
            if (config.ContainsKey(key))
            {
                var old = config[key];
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
