/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CprBroker.Schemas.Part;
using System.Xml;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Used as a link between miscellaneous person tables & person information
    /// </summary>
    public partial class PersonInfo
    {
        #region Properties & database expressions

        public PersonName PersonName { get; set; }
        public PersonTotal PersonTotal { get; set; }
        public Nationality Nationality { get; set; }
        public Street Street { get; set; }
        public PersonAddress Address { get; set; }

        /// <summary>
        /// LINQ expression that is able to create a IQueryable&lt;PersonInfo;gt; object based on a given date
        /// </summary>
        public static readonly Expression<Func<DPRDataContext, IQueryable<PersonInfo>>> PersonInfoExpression = (DPRDataContext dataContext) =>
            from personTotal in dataContext.PersonTotals
            join pNationality in dataContext.Nationalities on personTotal.PNR equals pNationality.PNR into personNationalities
            join pAddr in dataContext.PersonAddresses on personTotal.PNR equals pAddr.PNR into personAddresses
            join pName in dataContext.PersonNames on personTotal.PNR equals pName.PNR into personNames
            join strt in dataContext.Streets on new { personTotal.MunicipalityCode, personTotal.StreetCode } equals new { strt.MunicipalityCode, strt.StreetCode } into streets

            from personNationality in personNationalities.DefaultIfEmpty()
            from personAddress in personAddresses.OrderByDescending(pa => pa.AddressStartDate).DefaultIfEmpty()
            from personName in personNames.DefaultIfEmpty()
            from street in streets.DefaultIfEmpty()

            where
                // Active nationality only
            (personNationality == null || (personNationality.CorrectionMarker == null && personNationality.NationalityEndDate == null))
                // Active name only
            && (personName == null || (personName.CorrectionMarker == null && personName.NameTerminationDate == null))
                // Active address only
            && (personAddress == null || personAddress.CorrectionMarker == null)

            select new PersonInfo()
            {
                PersonTotal = personTotal,
                Nationality = personNationality,
                Address = personAddress,
                PersonName = personName,
                Street = street,
            };

        public static PersonInfo GetPersonInfo(DPRDataContext dataContext, decimal pnr)
        {
            var personTotal = dataContext.PersonTotals.Where(pt => pt.PNR == pnr).FirstOrDefault();
            if (personTotal != null)
            {
                return new PersonInfo()
                {
                    PersonTotal = personTotal,
                    Nationality = personTotal.Nationalities.Where(pn => pn.CorrectionMarker == null && pn.NationalityEndDate == null).OrderByDescending(pn => pn.NationalityStartDate).FirstOrDefault(),
                    Address = personTotal.PersonAddresses.Where(pa => pa.CorrectionMarker == null).OrderByDescending(pa => pa.AddressStartDate).FirstOrDefault(),
                    PersonName = personTotal.PersonNames.Where(pn => pn.CorrectionMarker == null).OrderByDescending(pn => pn.NameStartDate).FirstOrDefault(),
                    Street = personTotal.Street
                };
            }
            return null;
        }
        #endregion

        public DateTime[] GetCandidateRegistrationDates()
        {
            var dates = new List<DateTime?>();

            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.MaritalStatusDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.MunicipalityArrivalDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.MunicipalityLeavingDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.PaternityDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.PersonalSelectionDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.StatusDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.UnderGuardianshipDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonTotal.VotingDate));

            if (PersonName != null)
            {
                //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.AddressingNameDate));
                dates.Add(Utilities.DateFromDecimal(PersonName.AuthorityTextUpdateDate));
                dates.Add(Utilities.DateFromDecimal(PersonName.CprUpdateDate));
            }
            if (Address != null)
            {
                dates.Add(Utilities.DateFromDecimal(Address.CprUpdateDate));
            }
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.NameStartDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.NameTerminationDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.SearchNameDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.StatusDate));

            return (from d in dates where d.HasValue select d.Value).ToArray();
        }

        public DateTime RegistrationDate
        {
            get
            {
                var dates = GetCandidateRegistrationDates();
                if (dates.Length > 0)
                {
                    return dates.Max();
                }
                else
                {
                    return DateTime.Today;
                }
            }
        }

        public RegistreringType1 ToRegisteringType1(DateTime? effectTime, Func<string, Guid> cpr2uuidConverter, DPRDataContext dataContext)
        {
            Func<decimal, Guid> cpr2uuidFunc = (cpr) => cpr2uuidConverter(cpr.ToPnrDecimalString());

            var effectTimeDecimal = Utilities.DecimalFromDate(effectTime);

            RegistreringType1 ret = new RegistreringType1()
            {
                AttributListe = ToAttributListeType(),
                TilstandListe = ToTilstandListeType(),
                RelationListe = ToRelationListeType(cpr2uuidFunc, effectTimeDecimal, dataContext),

                AktoerRef = Constants.Actor,
                CommentText = Constants.CommentText,
                LivscyklusKode = LivscyklusKodeType.Rettet,
                Tidspunkt = TidspunktType.Create(this.RegistrationDate),
                Virkning = null
            };

            ret.CalculateVirkning();
            return ret;
        }

        public AttributListeType ToAttributListeType()
        {
            return new AttributListeType()
            {
                Egenskab = new EgenskabType[]
                {
                    ToEgenskabType()
                },
                RegisterOplysning = new RegisterOplysningType[]
                {
                    ToRegisterOplysningType()
                },

                // Health information not implemented
                SundhedOplysning = null,

                // No extensions at the moment
                LokalUdvidelse = null
            };
        }

        public EgenskabType ToEgenskabType()
        {
            var ret = new EgenskabType()
            {
                BirthDate = Utilities.DateFromDecimal(PersonTotal.DateOfBirth).Value,

                // Birth registration authority
                //TODO: Is this assignment correct?
                FoedselsregistreringMyndighedNavn = PersonTotal.BirthPlaceOfRegistration,

                // Place of birth
                FoedestedNavn = PersonTotal.BirthplaceText,

                PersonGenderCode = Utilities.PersonGenderCodeTypeFromChar(PersonTotal.Sex),

                NavnStruktur = PersonName != null ? NavnStrukturType.Create(PersonName.FirstName, PersonName.LastName) : null,

                AndreAdresser = Address != null ? Address.ToForeignAddressFromSupplementary() : null,
                //No contact channels implemented
                KontaktKanal = null,
                //Next of kin (nearest relative). Not implemented
                NaermestePaaroerende = null,
                //TODO: Fill this object
                Virkning = VirkningType.Create(Utilities.GetMaxDate(PersonTotal.DateOfBirth, PersonTotal.StatusDate), null)
            };
            if (PersonName != null)
            {
                ret.Virkning = VirkningType.Compose(ret.Virkning, VirkningType.Create(Utilities.DateFromDecimal(PersonName.NameStartDate), null));
            }
            // TODO: More effect date fields here
            ret.Virkning.FraTidspunkt = TidspunktType.Create(Utilities.GetMaxDate(PersonTotal.DateOfBirth));
            return ret;
        }

        public RegisterOplysningType ToRegisterOplysningType()
        {
            var ret = new RegisterOplysningType()
            {
                Item = null,
                Virkning = VirkningType.Create(null, null)
            };
            // Now create the appropriate object based on nationality
            if (
                Nationality == null
                || Nationality.CountryCode.ToDecimalString().Equals(Constants.CprNationalityKmdCode)
                || Nationality.CountryCode.ToDecimalString().Equals(Constants.StatelessKmdCode))
            {
                ret.Item = PersonTotal.ToUkendtBorgerType();
                ret.Virkning = PersonTotal.ToUkendtBorgerTypeVirkning();
            }
            else if (string.Equals(this.Nationality.CountryCode.ToDecimalString(), Constants.DenmarkKmdCode))
            {
                ret.Item = PersonTotal.ToCprBorgerType(Nationality, Address);
                ret.Virkning = PersonTotal.ToCprBorgerTypeVirkning(Nationality, Address);
            }
            else
            {
                ret.Item = PersonTotal.ToUdenlandskBorgerType(this.Nationality);
                ret.Virkning = PersonTotal.ToUdenlandskBorgerTypeVirkning();
            }

            return ret;
        }

        public TilstandListeType ToTilstandListeType()
        {
            return new TilstandListeType()
            {
                // TODO: Is it OK to get the full history here?
                CivilStatus = new CivilStatusType()
                {
                    CivilStatusKode = PersonTotal.ToCivilStatusCodeType(),
                    TilstandVirkning = TilstandVirkningType.Create(Utilities.DateFromDecimal(PersonTotal.MaritalStatusDate))
                },
                LivStatus = new LivStatusType()
                {
                    LivStatusKode = PersonTotal.ToLivStatusKodeType(),
                    TilstandVirkning = TilstandVirkningType.Create(Utilities.DateFromFirstDecimal(PersonTotal.StatusDate)),
                },
                // No extensions now
                LokalUdvidelse = null
            };
        }

        public RelationListeType ToRelationListeType(Func<decimal, Guid> cpr2uuidFunc, decimal? effectTimeDecimal, DPRDataContext dataContext)
        {
            var ret = new RelationListeType();
            // Now fill the relations
            var fatherPnr = Utilities.ToParentPnr(this.PersonTotal.FatherPersonalOrBirthdate);
            if (fatherPnr.HasValue)
            {
                ret.Fader = new PersonRelationType[]
                {
                    PersonRelationType.Create(
                        cpr2uuidFunc( fatherPnr.Value),
                        null,
                        null
                    )
                };
            }

            var motherPnr = Utilities.ToParentPnr(this.PersonTotal.MotherPersonalOrBirthDate);
            if (motherPnr.HasValue)
            {
                ret.Moder = new PersonRelationType[]
                {
                    PersonRelationType.Create
                        (cpr2uuidFunc( motherPnr.Value) ,
                        null,
                        null
                    )
                };
            }

            ret.Boern = PersonFlerRelationType.CreateList(
                Array.ConvertAll<PersonTotal, Guid>
                (
                    Child.PersonChildrenExpression.Compile()(effectTimeDecimal.Value, PersonTotal.PNR, dataContext).ToArray(),
                    (pt) => cpr2uuidFunc(pt.PNR)
                ));

            // TODO : Fill adult children
            ret.Foraeldremyndighedsboern = null;

            ret.Aegtefaelle =
                (from civ in dataContext.CivilStatus
                 where civ.PNR == PersonTotal.PNR
                    && civ.MaritalStatusDate <= effectTimeDecimal.Value
                    && civ.SpousePNR.HasValue && civ.SpousePNR.Value > 0
                    && !civ.CorrectionMarker.HasValue
                 orderby civ.MaritalStatusDate
                 select new PersonRelationType()
                 {
                     ReferenceID = UnikIdType.Create(cpr2uuidFunc(civ.SpousePNR.Value)),
                     CommentText = "",
                     Virkning = VirkningType.Create(Utilities.DateFromDecimal(civ.MaritalStatusDate), Utilities.DateFromDecimal((civ.MaritalEndDate)))
                 }).ToArray();

            // TODO: Add custody relations
            ret.Foraeldremyndighedsindehaver = null;

            ret.RegistreretPartner = null;
            //TODO: Fill Legal capacity Legal guardian for person
            ret.RetligHandleevneVaergeForPersonen = null;

            //TODO: Fill Legal capacity Guardianship Proprietor
            ret.Foraeldremyndighedsindehaver = null;

            return ret;
        }

    }
}
