using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CprBroker.Providers.CprServices.Responses
{
    public class BaseResponse<TRowItem>
        where TRowItem : RowItem
    {
        protected XmlDocument _ResponseDocument;
        protected XmlNamespaceManager _NamespaceManager;
        protected XmlElement[] _Rows;
        protected TRowItem[] _RowItems;

        private BaseResponse()
        { }

        public BaseResponse(string xml,
            Func<XmlElement, XmlNamespaceManager, TRowItem> creator)
        {
            _ResponseDocument = new XmlDocument();
            _ResponseDocument.LoadXml(xml);
            _NamespaceManager = new XmlNamespaceManager(_ResponseDocument.NameTable);
            _NamespaceManager.AddNamespace("c", CprServices.Constants.XmlNamespace);

            _Rows = _ResponseDocument.SelectNodes("//c:Table/c:Row", _NamespaceManager)
                .OfType<XmlElement>()
                .ToArray();

            _RowItems = _Rows
                .Select(r => creator(r, _NamespaceManager))
                .ToArray();
        }
    }

    public class BaseResponse : BaseResponse<RowItem>
    {
        public BaseResponse(string xml)
            : base(xml, (e, nsMgr) => new RowItem(e, nsMgr))
        { }
    }

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
    }
}
