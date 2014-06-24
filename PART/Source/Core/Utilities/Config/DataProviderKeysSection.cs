/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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

        public static RijndaelManaged GetFromConfig(Configuration configFile)
        {
            RijndaelManaged rm = new RijndaelManaged();
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

            try { group.Sections.Remove(SectionName); }
            catch { }

            DataProviderKeysSection section = new DataProviderKeysSection();

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
