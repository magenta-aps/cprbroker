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
        public static readonly string DefaultRowXPath = "//c:Table/c:Row";

        protected XmlDocument _ResponseDocument;
        protected XmlNamespaceManager _NamespaceManager;
        protected XmlElement[] _RowXmlElements;
        protected TRowItem[] _RowItems;

        public TRowItem[] RowItems { get { return _RowItems; } }

        private BaseResponse()
        { }

        public BaseResponse(string xml,
            Func<XmlElement, XmlNamespaceManager, TRowItem> creator)
            : this(xml, DefaultRowXPath, creator)
        {
        }

        public BaseResponse(string xml,
            string rowXPath,
            Func<XmlElement, XmlNamespaceManager, TRowItem> creator)
        {
            _ResponseDocument = new XmlDocument();
            _ResponseDocument.LoadXml(xml);
            _NamespaceManager = new XmlNamespaceManager(_ResponseDocument.NameTable);
            _NamespaceManager.AddNamespace("c", CprServices.Constants.XmlNamespace);

            _RowXmlElements = _ResponseDocument.SelectNodes(rowXPath, _NamespaceManager)
                .OfType<XmlElement>()
                .ToArray();

            _RowItems = _RowXmlElements
                .Select(r => creator(r, _NamespaceManager))
                .ToArray();
        }

    }

    public class GenericResponse : BaseResponse<RowItem>
    {
        public GenericResponse(string xml)
            : base(xml, (elm, nsMgr) => new RowItem(elm, nsMgr))
        { }
    }
}
