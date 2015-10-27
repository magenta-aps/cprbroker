using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CprBroker.Providers.CprServices.Responses
{
    public class RowItem
    {
        protected XmlElement _Node;
        protected XmlNamespaceManager _NamespaceManager;

        public RowItem(XmlElement elm, XmlNamespaceManager nsMgr)
        {
            _Node = elm;
            _NamespaceManager = nsMgr;
        }

        public string GetNodeValue(string key, string attrName = "v")
        {
            string xPath = string.Format("c:Field[@r='{0}']/@{1}", key, attrName);
            var nd = _Node.SelectSingleNode(xPath, _NamespaceManager);

            return nd != null ? nd.Value : null;
        }

        public string this[string key]
        {
            get { return this.GetNodeValue(key); }
        }

        public string this[string key, string attrName]
        {
            get { return this.GetNodeValue(key, attrName); }
        }

        public string GetFieldValue(XmlElement elm, string name)
        {
            return GetFieldAttributeValue(elm, name, "v");
        }

        public string GetFieldText(XmlElement elm, string name)
        {
            return GetFieldAttributeValue(elm, name, "t");
        }

        public string GetFieldAttributeValue(XmlElement elm, string name, string attributeName)
        {
            return this[name, attributeName];
        }

        public void Merge(RowItem item)
        { 
            foreach(XmlElement newFieldElement in item._Node.ChildNodes.OfType<XmlElement>())
            {
                var fieldName = newFieldElement.Attributes["r"].Value;

                string xPath = string.Format("c:Field[@r='{0}']", fieldName);
                var myNode = _Node.SelectSingleNode(xPath, _NamespaceManager);
                if(myNode == null)
                {
                    myNode = _Node.OwnerDocument.CreateElement(newFieldElement.Name, newFieldElement.NamespaceURI);
                    _Node.AppendChild(myNode);
                }
                foreach(XmlAttribute attr in newFieldElement.Attributes)
                {
                    var myAttr = myNode.Attributes[attr.Name];
                    if(myAttr == null)
                    {
                        myAttr = _Node.OwnerDocument.CreateAttribute(attr.Prefix, attr.LocalName, attr.NamespaceURI);
                        myNode.Attributes.Append(myAttr);
                    }
                    myAttr.Value = attr.Value;
                }
                
            }
        }
    }
}
