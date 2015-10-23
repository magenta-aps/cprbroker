using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CprServices.Responses;

namespace CprBroker.Providers.ServicePlatform.Responses
{
    public class FamilyPlusResponse : BaseResponse<FamilyPlusResponse.RelationItem>
    {

        public FamilyPlusResponse(string xml)
            : base(xml, (e, nsMgr) => new RelationItem(e, nsMgr))
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
            var ret = new Schemas.Part.RelationListeType()
            {
                // Parental
                Fader = GetPersonRelations("Far", uuidGetter),
                Moder = GetPersonRelations("Mor", uuidGetter),
                Boern = GetPersonFlerRelations("Barn", uuidGetter),

                // Spouse(s)
                Aegtefaelle = GetPersonRelations("Ægtefælle", uuidGetter),
                RegistreretPartner = null,

                Foraeldremyndighedsindehaver = null, // TODO: Get from Family+ / CPR FORAELDR // To be set later

                // The rest are not available in test data for Familie+
                Bopaelssamling = null,
                ErstatningAf = null,
                ErstatningFor = null, // Not in Stam+
                Foraeldremyndighedsboern = null,
                RetligHandleevneVaergeForPersonen = null,
                RetligHandleevneVaergemaalsindehaver = null,

                // No extension
                LokalUdvidelse = null,
            };
            // Special logic for parental authority
            ret.Foraeldremyndighedsindehaver = ToParents(ret, uuidGetter);
            return ret;
        }

        public PersonRelationType[] ToParents(RelationListeType relationListe, Func<string, Guid> uuidGetter)
        {
            var parents = new List<PersonRelationType>();

            Action<string> adder = (postFix) =>
            {
                var item = this._RowItems.First();

                var parentType = item["CFMY_RELTYP" + postFix];
                if (!string.IsNullOrEmpty(parentType))
                {

                    Action relationUuidGetter = () =>
                    {
                        var relPnr = item["FORAELDRE_MYN_PNR" + postFix];
                        if (!string.IsNullOrEmpty(relPnr) && CprBroker.PartInterface.Strings.IsValidPersonNumber(relPnr))
                        {
                            parents.Add(PersonRelationType.Create(uuidGetter(relPnr), null, null));
                        }
                    };

                    var parentTypeInt = int.Parse(parentType);
                    switch (parentTypeInt)
                    {
                        case 3:
                            parents.AddRange(relationListe.Moder);
                            break;
                        case 4:
                            parents.AddRange(relationListe.Fader);
                            break;
                        case 5:
                            relationUuidGetter();
                            break;
                        case 6:
                            relationUuidGetter();
                            break;
                    }
                }
            };
            adder("A");
            adder("B");

            var ret = parents.Where(p => p != null).ToArray();
            if (ret.Length == 0)
                ret = null;

            return ret;
        }

        public CivilStatusType ToCivilStatusType()
        {
            var spouses = GetPersonRelations("Ægtefælle", pnr => Guid.NewGuid());
            return new CivilStatusType()
            {
                // Detection of registered partnership, or any other status is not possible
                CivilStatusKode = spouses.Length > 0 ? CivilStatusKodeType.Gift : CivilStatusKodeType.Ugift,
                // Date unavailable
                TilstandVirkning = TilstandVirkningType.Create(null),
            };
        }

        public DateTime? ToNameStartDate(bool allowIncompleteDates = false)
        {
            //System.Diagnostics.Debugger.Launch();
            var retStr = this.RowItems.First()["CNVN_STARTDATO"];
            DateTime? ret = null;
            if (!string.IsNullOrEmpty(retStr))
            {
                DateTime retDate;
                var formats = new List<string>(new string[] { "yyyyMMdd", "yyyyMMddHHmm", });

                if (allowIncompleteDates)
                    formats.AddRange(new string[] { "yyyyMM00HHmm", "yyyy0000HHmm" });

                if (
                    DateTime.TryParseExact(
                        retStr,
                        formats.ToArray(),
                        null,
                        System.Globalization.DateTimeStyles.None,
                        out retDate))
                {
                    ret = retDate;
                }
            }
            return ret;
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
