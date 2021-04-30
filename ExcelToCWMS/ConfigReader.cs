using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToCWMS
{
    public class ConfigReader
    {
        private string configFile;
        private Dictionary<string, string> dict;
        public ConfigReader(string configFile)
        {
            var lines = File.ReadAllLines(configFile);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in lines)
            {
                var tokens = item.Split(':');
                if (tokens.Length == 2)
                {
                    dict.Add(tokens[0], tokens[1]);
                }
            }
            this.dict = dict;
        }
        public string CRead(string item)
        {
            if (dict.TryGetValue(item, out string v))
            {
                return v;
            }
            else
            {
                throw new Exception("could not read " + item + " from " + configFile);
            }


        }
    }
}