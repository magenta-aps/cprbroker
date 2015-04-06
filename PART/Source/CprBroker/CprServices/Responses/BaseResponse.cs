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
}
