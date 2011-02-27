using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Web;


namespace CprBroker.Engine.Util
{
    public static class Security
    {
        public static string CurrentUser
        {
            get
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null)
                    return System.Web.HttpContext.Current.User.Identity.Name;
                else
                    return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
        }

        public static void EncryptConfigSection(string sectionName)
        {
            Configuration config;
            if (HttpContext.Current != null)
            {
                config = WebConfigurationManager.OpenWebConfiguration("/");
            }
            else
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }

            ConfigurationSection section = config.GetSection(sectionName);
            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                config.Save();
            }
        }

        public static void EncryptConnectionStrings()
        {
            EncryptConfigSection("connectionStrings");
        }


    }
}
