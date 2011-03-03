using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.Web;
using System.Xml;
using System.IO;

namespace CprBroker.Utilities
{
    public class DataProviderKeysSection : ConfigurationSection
    {
        public const string SectionName = "dataProviderKeys";

        [ConfigurationProperty("IV", IsRequired = true)]
        private string IVString
        {
            get
            {
                return (string)this["IV"];
            }
            set
            {
                this["IV"] = value;
            }
        }

        [ConfigurationProperty("Key", IsRequired = true)]
        private string KeyString
        {
            get
            {
                return (string)this["Key"];
            }
            set
            {
                this["Key"] = value;
            }
        }

        public static bool IsValid(DataProviderKeysSection section)
        {
            if (section == null || string.IsNullOrEmpty(section.IVString) || string.IsNullOrEmpty(section.KeyString))
            {
                return false;
            }
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateIV();
            if (rm.IV.Length != section.IV.Length)
            {
                return false;
            }
            rm.GenerateKey();
            if (rm.Key.Length != section.Key.Length)
            {
                return false;
            }
            return true;
        }

        public byte[] IV
        {
            get
            {
                return Strings.Deserialize<byte[]>(IVString);
            }
        }

        public byte[] Key
        {
            get
            {
                return Strings.Deserialize<byte[]>(KeyString);
            }
        }


        public static RijndaelManaged GetFromConfig()
        {
            RijndaelManaged rm = new RijndaelManaged();
            Configuration configFile = Config.GetConfigFile();
            ConfigurationSectionGroup group = configFile.SectionGroups[Constants.DataProvidersSectionGroupName];
            if (group == null)
            {
                group = new ConfigurationSectionGroup();
                configFile.SectionGroups.Add(Constants.DataProvidersSectionGroupName, group);
                configFile.Save();
            }

            DataProviderKeysSection section = group.Sections[SectionName] as DataProviderKeysSection;
            if (!IsValid(section))
            {
                if (section == null)
                {
                    section = new DataProviderKeysSection();
                    group.Sections.Add(DataProviderKeysSection.SectionName, section);
                }
                rm.GenerateIV();
                rm.GenerateKey();
                section.IVString = Strings.SerializeObject(rm.IV);
                section.KeyString = Strings.SerializeObject(rm.Key);

                if (!section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                }

                configFile.Save();
                ConfigurationManager.RefreshSection(SectionName);
            }
            rm.IV = section.IV;
            rm.Key = section.Key;
            return rm;
        }

        public static void RegisterInConfig(Configuration configFile)
        {
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateKey();
            rm.GenerateIV();

            ConfigurationSectionGroup group = configFile.SectionGroups[Constants.DataProvidersSectionGroupName];
            if (group == null)
            {
                group = new ConfigurationSectionGroup();
                configFile.SectionGroups.Add(Constants.DataProvidersSectionGroupName, group);
            }

            group.Sections.Clear();
            DataProviderKeysSection section = group.Sections[SectionName] as DataProviderKeysSection;
            if (section == null)
            {
                section = new DataProviderKeysSection();
            }
            section.IVString = Strings.SerializeObject(rm.IV);
            section.KeyString = Strings.SerializeObject(rm.Key);
            group.Sections.Add(SectionName, section);            

            configFile.Save();
        }

        /*  public static XmlNode CreateXmlSectionDefinitionNode()
          {
              XmlDocument doc = new XmlDocument();
              XmlElement node = doc.CreateElement("section");

              var nameAttr = doc.CreateAttribute("name");
              nameAttr.Value = SectionName;
              node.Attributes.Append(nameAttr);

              var typeAttr = doc.CreateAttribute("type");
              typeAttr.Value = typeof(DataProviderKeysSection).ToString();
              node.Attributes.Append(typeAttr);

              return node;
          }

          public static XmlNode CreateNewXmlSectionNode()
          {
              StringWriter sw = new StringWriter();
              XmlTextWriter xw = new XmlTextWriter(sw);
              XmlDocument doc = new XmlDocument();
              XmlElement node = doc.CreateElement(SectionName);

              RijndaelManaged rm = new RijndaelManaged();
              rm.GenerateIV();
              rm.GenerateKey();

              var ivAttr=doc.CreateAttribute("IV");
              ivAttr.Value= Strings.SerializeObject(rm.IV);
              node.Attributes.Append(ivAttr);

              var keyAttr = doc.CreateAttribute("Key");
              keyAttr.Value = Strings.SerializeObject(rm.Key);
              node.Attributes.Append(ivAttr);

              return node;
          }*/



    }
}
