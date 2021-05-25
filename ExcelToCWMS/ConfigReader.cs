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
        private string m_configFile;
        private Dictionary<string, string> m_dict;
        public ConfigReader(string configFile)
        {
            m_configFile = configFile;
            var lines = File.ReadAllLines(configFile);
            m_dict = new Dictionary<string, string>();
            foreach (var item in lines)
            {
                if (item.StartsWith("#"))
                    continue;
                var tokens = item.Split(':');
                if (tokens.Length == 2)
                {
                    m_dict.Add(tokens[0], tokens[1]);
                }
            }
        }
        public string CRead(string item)
        {
            if (m_dict.TryGetValue(item, out string v))
            {
                return v;
            }
            else
            {
                throw new Exception("could not read " + item + " from " + m_configFile);
            }


        }
    }
}