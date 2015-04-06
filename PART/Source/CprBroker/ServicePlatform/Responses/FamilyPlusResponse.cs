using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.ServicePlatform.Responses
{
    public class FamilyPlusResponse : BaseResponse<FamilyPlusResponse.RelationItem>
    {

        public FamilyPlusResponse(string xml)
            : base(xml, (e,nsMgr) => new RelationItem(e, nsMgr))
        {
            
        }

        private RelationItem[] GetRelationNodes(string key)
        {
            return this._RowItems
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

        public class RelationItem : RowItem
        {
            public RelationItem(XmlElement elm, XmlNamespaceManager nsMgr)
                : base(elm, nsMgr)
            {
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
