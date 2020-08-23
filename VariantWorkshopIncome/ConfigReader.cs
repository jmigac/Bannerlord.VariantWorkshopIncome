using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VariantWorkshopIncome
{
    class ConfigReader
    {
        public static ConfigReader _INSTANCE;
        public ConfigReader()
        {

        }
        public static ConfigReader GetInstance()
        {
            if (_INSTANCE == null){
                _INSTANCE = new ConfigReader();
            }
            return _INSTANCE;
        }
        public string Get(string key)
        {
            Configuration config = GetConfiguration();
            return config != null ? GetConfigData(config, key) : (string)null;
        }
        private Configuration GetConfiguration()
        {
            string path = this.GetType().Assembly.Location; 
            try
            {
                return ConfigurationManager.OpenExeConfiguration(path);
            }
            catch (Exception ex)
            {

            }
            return (Configuration)null;
        }
        private string GetConfigData(Configuration config,string key)
        {
            KeyValueConfigurationElement settingValue = config.AppSettings.Settings[key];
            if(settingValue != null)
            {
                string returnMe = settingValue.Value;
                if (!string.IsNullOrEmpty(returnMe))
                    return returnMe;
            }
            return string.Empty;
        }
    }
}
