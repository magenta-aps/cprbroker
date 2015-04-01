using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.ServicePlatform.Responses
{
    public class FamilyPlusResponse
    {
        private XmlDocument _ResponseDocument;
        private XmlNamespaceManager _NamespaceManager;
        private RelationItem[] RelationNodes;

        private FamilyPlusResponse()
        { }

        public FamilyPlusResponse(string xml)
        {
            _ResponseDocument = new XmlDocument();
            _ResponseDocument.LoadXml(xml);
            _NamespaceManager = new XmlNamespaceManager(_ResponseDocument.NameTable);
            _NamespaceManager.AddNamespace("c", CprServices.Constants.XmlNamespace);

            RelationNodes = _ResponseDocument.SelectNodes("//c:Table/c:Row", _NamespaceManager)
                .OfType<XmlElement>()
                .Select(e => new RelationItem(e, _NamespaceManager))
                .ToArray();
        }

        private RelationItem[] GetRelationNodes(string key)
        {
            return this.RelationNodes
                .Where(r =>
                    (string.IsNullOrEmpty(key) || r.RelationTypeString == key)
                    && CprBroker.PartInterface.Strings.IsValidPersonNumber(r.PnrOrBirthdate)
                    )
                    .ToArray();
        }

        public PersonFlerRelationType[] GetPersonFlerRelations(string key, Func<string, Guid> func)
        {
            return GetRelationNodes(key).Select(n => PersonFlerRelationType.Create(func(n.PnrOrBirthdate), null, null)).ToArray();
        }

        public PersonRelationType[] GetPersonRelations(string key, Func<string, Guid> func)
        {
            return GetRelationNodes(key).Select(n => PersonRelationType.Create(func(n.PnrOrBirthdate), null, null)).ToArray();
        }

        public string[] ToRelationPNRs()
        {
            return GetRelationNodes(null)
                .Select(n => n.PnrOrBirthdate)
                .ToArray();
        }

        public RelationListeType ToRelationListeType(Func<string, Guid> uuidGetter)
        {
            return new Schemas.Part.RelationListeType()
            {
                // Parental
                Fader = GetPersonRelations("Far", uuidGetter),
                Moder = GetPersonRelations("Mor", uuidGetter),
                Boern = GetPersonFlerRelations("Barn", uuidGetter),

                // Spouse(s)
                Aegtefaelle = GetPersonRelations("Ægtefælle", uuidGetter),
                RegistreretPartner = null,

                // The rest are not available in test data for Familie+
                Bopaelssamling = null,
                ErstatningAf = null,
                ErstatningFor = null,
                Foraeldremyndighedsboern = null,
                Foraeldremyndighedsindehaver = null,
                RetligHandleevneVaergeForPersonen = null,
                RetligHandleevneVaergemaalsindehaver = null,

                // No extension
                LokalUdvidelse = null,
            };
        }

        public class RelationItem
        {
            private XmlElement _Node;
            private XmlNamespaceManager _NamespaceManager;

            public RelationItem(XmlElement elm, XmlNamespaceManager nsMgr)
            {
                _Node = elm;
                _NamespaceManager = nsMgr;
            }

            public string GetNodeValue(string key)
            {
                string xPath = string.Format("c:Field[@r='{0}']/@v", key);
                var nd = _Node.SelectSingleNode(xPath, _NamespaceManager);

                return nd != null ? nd.Value : null;
            }

            public string PnrOrBirthdate
            {
                get { return GetNodeValue("PNR_FOEDDATO"); }
            }

            public string RelationTypeString
            {
                get { return GetNodeValue("FAMMRK"); }
            }
        }
    }
}
