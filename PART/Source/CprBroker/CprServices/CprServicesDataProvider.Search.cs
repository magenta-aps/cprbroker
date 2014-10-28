using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    public partial class CprServicesDataProvider
    {
        #region ADRSOG1
        public bool CanCallADRSOG1(SoegAttributListeType attributes)
        {
            if (attributes.SoegRegisterOplysning != null
                && attributes.SoegRegisterOplysning[0].Item is CprBorgerType)
            {
                var cpr = attributes.SoegRegisterOplysning[0].Item as CprBorgerType;
                if (cpr.FolkeregisterAdresse.Item is DanskAdresseType)
                {
                    var dan = cpr.FolkeregisterAdresse.Item as DanskAdresseType;
                    return !string.IsNullOrEmpty(dan.AddressComplete.AddressAccess.MunicipalityCode.Trim())
                        && !string.IsNullOrEmpty(dan.AddressComplete.AddressAccess.StreetCode.Trim());
                }
                else if (cpr.FolkeregisterAdresse.Item is GroenlandAdresseType)
                {
                    var gr = cpr.FolkeregisterAdresse.Item as GroenlandAdresseType;
                    return !string.IsNullOrEmpty(gr.AddressCompleteGreenland.MunicipalityCode.Trim())
                        && !string.IsNullOrEmpty(gr.AddressCompleteGreenland.StreetCode.Trim());
                }
            }
            return false;
        }

        public string[] CallADRSOG1(string token, SoegAttributListeType attributes)
        {
            var inp = Properties.Resources.ADRSOG1;

            var doc = new XmlDocument();
            doc.LoadXml(inp);

            var resp = "";
            var kvit = Send(doc.OuterXml, ref token, out resp);
            return null;
        }

        public void AddADRSOG1SearchCriteria(XmlDocument doc, SoegAttributListeType attributes)
        {
            // As a minimum, this method should add KOMK and VEJK
            var keyNode = InitKeyNode(doc);

            // Add max records node - fixed by definition, but apparently not required            
            //AddKeyField(keyNode, "MAXA", "20");

            
        }

        public void AddNameSearchCriteria(XmlNode keyNode, NavnStrukturType egen)
        {
            // Names
            
        }
        #endregion

        #region Utility functions
        private static XmlNode InitKeyNode(XmlDocument doc)
        {
            var keyNode = doc.SelectSingleNode("//Key");
            keyNode.RemoveAll();
            return keyNode;
        }

        public void AddKeyField(XmlNode keyNode, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var fieldNode = keyNode.SelectSingleNode("//Field[@r='" + name + "']");
                if (fieldNode == null)
                {
                    fieldNode = keyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "", "Field", "");
                    keyNode.AppendChild(fieldNode);

                    var rAttr = keyNode.OwnerDocument.CreateAttribute("r");
                    rAttr.Value = name;
                    fieldNode.Attributes.Append(rAttr);

                    var vAttr = keyNode.OwnerDocument.CreateAttribute("v");
                    vAttr.Value = value;
                    fieldNode.Attributes.Append(vAttr);

                }
                else
                {
                    fieldNode.Attributes["v"].Value = value;
                }
            }
        }
        #endregion

        #region ADRESSE3
        public void CallADRESSE3(string token, string pnr)
        {
            var inp = Properties.Resources.ADRESSE3;
            var doc = new XmlDocument();
            doc.LoadXml(inp);
            var keyNode = InitKeyNode(doc);
            AddKeyField(keyNode, "PNR", pnr);
            var ret = "";
            var kvit = Send(doc.OuterXml, ref token, out ret);
            if (kvit.OK)
            {
                object ok = "";
            }
            object o = "";
        }
        #endregion

        #region NVNSOG1
        public bool CanCallNVNSOG2(SoegAttributListeType attributes)
        {
            if (attributes.SoegEgenskab.Length > 0)
            {
                var nvn = attributes.SoegEgenskab[0].NavnStruktur;
                return !string.IsNullOrEmpty(nvn.PersonNameForAddressingName.Trim())
                    //|| !string.IsNullOrEmpty(nvn.KaldenavnTekst.Trim())
                    || (nvn.PersonNameStructure != null && nvn.PersonNameStructure.IsEmpty);
            }
            return false;
        }

        public void CallNVNSOG2(string token, SoegAttributListeType attribtues)
        {
            var inp = Properties.Resources.NVNSOG2;
            var doc = new XmlDocument();
            doc.LoadXml(inp);
            AddNVNSOG2SearchCriteria(doc, attribtues);
            var resp = "";
            var kvit = Send(doc.OuterXml, ref token, out resp);
        }

        public void AddNVNSOG2SearchCriteria(XmlDocument doc, SoegAttributListeType attributes)
        {
            var nvn = attributes.SoegEgenskab[0].NavnStruktur;
            var keyNode = InitKeyNode(doc);
            AddNameSearchCriteria(keyNode, nvn);
            AddKeyField(keyNode, "KON", "M");
            AddKeyField(keyNode, "MAXA", "20");
        }

        #endregion
    }
}
