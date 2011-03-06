using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;

namespace CprBroker.Utilities
{
    public static class Config
    {
        public static Configuration GetConfigFile()
        {
            if (HttpContext.Current != null)
            {
                return System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            }
            else
            {
                return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
        }
    }
}
