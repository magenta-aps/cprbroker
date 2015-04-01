using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CprBroker.Providers.ServicePlatform.Responses
{
    public class BaseResponse
    {
        protected XmlDocument _ResponseDocument;
        protected XmlNamespaceManager _NamespaceManager;

        private BaseResponse()
        { }

        public BaseResponse(string xml)
        {
            _ResponseDocument = new XmlDocument();
            _ResponseDocument.LoadXml(xml);
            _NamespaceManager = new XmlNamespaceManager(_ResponseDocument.NameTable);
            _NamespaceManager.AddNamespace("c", CprServices.Constants.XmlNamespace);
        }
    }
}
