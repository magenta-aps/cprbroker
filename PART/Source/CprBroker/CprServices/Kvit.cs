using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CprBroker.Providers.CprServices
{
    public class Kvit
    {
        [XmlAttribute("r")]
        public string Row { get; set; }

        [XmlAttribute("t")]
        public string ReturnText { get; set; }

        [XmlAttribute("v")]
        public string ReturnCode { get; set; }

        public static Kvit FromResponseXml(string responseText)
        {
            var doc = new XmlDocument();
            doc.LoadXml(responseText);
            return FromXmlDocument(doc);
        }

        public static Kvit FromXmlDocument(XmlDocument doc)
        {
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cpr", Constants.XmlNamespace);
            var kvitNode = doc.SelectSingleNode("//cpr:Kvit", ns);
            if (kvitNode != null)
            {
                var ret = new Kvit();
                ret.Row = kvitNode.Attributes["r"].Value;
                ret.ReturnCode = kvitNode.Attributes["v"].Value;
                ret.ReturnText = kvitNode.Attributes["t"].Value;
                return ret;
            }
            return null;
        }

        public bool OK
        {
            get { return ReturnCode == "900"; }
        }

    }
}
