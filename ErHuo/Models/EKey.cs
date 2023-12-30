using ErHuo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErHuo.Models
{
    public class EKey
    {
        public string Key { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }

        public EKey() { }
        public EKey(string key, int code = -1, string name = null)
        {
            Key = key;
            if (code == -1)
                Code = (int)Enum.Parse(typeof(VK), key);
            else
                Code = code;
            if (name == null)
            {
                if (Constant.KeyTranslate.ContainsKey(key))
                {
                    Name = Constant.KeyTranslate[key];
                }
                else
                {
                    Name = key;
                }
            }
            else
            {
                Name = name;
            }
        }

        public static EKey GetEKeyFromName(string keyName)
        {
            string key;
            if (Constant.KeyNameTranslate.ContainsKey(keyName))
            {
                key = Constant.KeyNameTranslate[keyName];
            }
            else
            {
                key = keyName;
            }
            return new EKey(key);
        }

        public bool IsSame(EKey obj)
        {
            return Code == obj.Code;
        }
    }
}
