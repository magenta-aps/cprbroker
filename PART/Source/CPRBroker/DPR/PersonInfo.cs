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
    internal partial class PersonInfo
    {
        #region Properties & database expressions

        public PersonName PersonName { get; set; }
        public PersonTotal PersonTotal { get; set; }
        public Street Street { get; set; }
        public ContactAddress ContactAddress { get; set; }
        //TODO: remove this field
        public bool HasProtection { get; set; }
        public IQueryable<CivilStatus> CivilStates { get; set; }

        /// <summary>
        /// LINQ expression that is able to create a IQueryable&lt;PersonInfo;gt; object based on a given date
        /// </summary>
        internal static readonly Expression<Func<DPRDataContext, IQueryable<PersonInfo>>> PersonInfoExpression = (DPRDataContext dataContext) =>
            from personTotal in dataContext.PersonTotals
            //TODO: Convert to left outer join, beware that this will make it possible for PersonName to be null
            join personName in dataContext.PersonNames on personTotal.PNR equals personName.PNR
            join street in dataContext.Streets on new { personTotal.MunicipalityCode, personTotal.StreetCode } equals new { street.MunicipalityCode, street.StreetCode } into strt
            join contactAddress in dataContext.ContactAddresses on personName.PNR equals contactAddress.PNR into contactAddr
            // TODO correct this condition
            where
            personName.NameTerminationDate == null
            select new PersonInfo()
            {
                PersonName = personName,
                PersonTotal = personTotal,
                Street = strt.FirstOrDefault(),
                ContactAddress = contactAddr.SingleOrDefault(),
                // TODO: include protection type with PNR because the index is on PNR & ProtectionType                
                HasProtection = (
                   from protection in dataContext.Protections
                   select protection.PNR
                ).Contains(personName.PNR),
                //TODO: Beware that there might be time range intersections in the last day of an older period, like a marriage period after a divorce period
                CivilStates = (from civ in dataContext.CivilStatus where !civ.CorrectionMarker.HasValue && (civ.PNR == personTotal.PNR || civ.SpousePNR == personTotal.PNR) select civ),
            };

        internal static readonly Expression<Func<decimal, decimal, DPRDataContext, PersonTotal>> NextPersonTotalExpression = (pnr, statusDate, dataContext) =>
            (from personTotal in dataContext.PersonTotals
             where pnr == personTotal.PNR && personTotal.StatusDate > statusDate
             orderby personTotal.StatusDate
             select personTotal
            ).FirstOrDefault();

        #endregion

        // TODO: Remove this method
        [Obsolete]
        private PersonRegistration ToPersonRegistration(DateTime? effectTime, DPRDataContext dataContext)
        {
            var civilRegistrationStatus = Schemas.Util.Enums.ToCivilRegistrationStatus(PersonTotal.Status);
            var effectTimeDecimal = Utilities.DecimalFromDate(effectTime);
            NavnStruktur tempPersonName = new NavnStruktur(PersonName.FirstName, PersonName.LastName);
            var civilStates = (from civilStatus in dataContext.CivilStatus
                               where civilStatus.PNR == PersonTotal.PNR && civilStatus.MaritalStatusDate <= effectTimeDecimal.Value
                               orderby civilStatus.MaritalStatusDate
                               select civilStatus).ToArray();

            PersonRegistration ret = new PersonRegistration()
            {
                Attributes = new PersonAttributes()
                {
                    BirthDate = Utilities.DateFromDecimal(PersonTotal.DateOfBirth).Value,
                    OtherAddresses = new Address[0],
                    ContactChannel = new ContactChannel[0],
                    Gender = Utilities.GenderFromChar(PersonTotal.Sex),
                    Name = new Effect<string>()
                    {
                        StartDate = Utilities.DateFromDecimal(PersonName.NameStartDate),
                        EndDate = Utilities.DateFromDecimal(PersonName.NameTerminationDate),
                        Value = tempPersonName.ToString(),
                    },
                    PersonData = new CprData()
                    {
                        PersonName = new Effect<NavnStruktur>()
                        {
                            StartDate = Utilities.DateFromDecimal(PersonName.NameStartDate),
                            EndDate = Utilities.DateFromDecimal(PersonName.NameTerminationDate),
                            Value = tempPersonName
                        },
                        AddressingName = PersonName.AddressingName,
                        BirthDateUncertainty = null,

                        CprNumber = PersonName.PNR.ToDecimalString(),

                        Gender = Utilities.GenderFromChar(PersonTotal.Sex),
                        //TODO: correct this field
                        IndividualTrackStatus = true,

                        NameAndAddressProtection = HasProtection,
                        NationalityCountryCode = CprBroker.DAL.Country.GetCountryAlpha2CodeByDanishName(PersonTotal.Nationality),
                        //TODO: find if applicable
                        NickName = null,
                        // TODO: Ensure that ContactAddress is the right object to pass
                        PopulationAddress = PersonTotal.ToPartAddress(civilRegistrationStatus, Street, ContactAddress),
                    },
                },
                //TODO: Fix calculation of registration date in DPR
                RegistrationDate = Utilities.DateFromDecimal(this.PersonName.NameStartDate).Value,
                // TODO: Add relations
                Relations = new PersonRelations()
                {
                    Children = new Effect<PersonRelation>[0],
                    Parents = new PersonRelation[0],
                    ReplacedBy = null,
                    Spouses = new Effect<PersonRelation>[0],
                    SubstituteFor = new Effect<PersonRelation>[0],
                },
            };

            // Now fill the null EndDate(s)
            if (effectTime.HasValue && effectTime.Value.Date < DateTime.Today)
            {
                if (PersonTotal.StatusDate.HasValue)
                {
                    var nextPersonTotal = NextPersonTotalExpression.Compile()(PersonTotal.PNR, PersonTotal.StatusDate.Value, dataContext);
                    if (nextPersonTotal != null)
                    {
                        ret.States.LifeStatus.EndDate = Utilities.DateFromDecimal(nextPersonTotal.StatusDate);
                    }
                }
                if (PersonTotal.MaritalStatusDate.HasValue)
                {
                    var maritalStatus =
                        (
                            from ms in civilStates
                            where ms.MaritalStatusDate == PersonTotal.MaritalStatusDate
                            select ms
                        ).FirstOrDefault();
                    if (maritalStatus != null)
                    {
                        ret.States.CivilStatus.EndDate = Utilities.DateFromDecimal(maritalStatus.MaritalEndDate);
                    }
                }
            }


            return ret;
        }

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

            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.AddressingNameDate));
            dates.Add(Utilities.DateFromDecimal(PersonName.AuthorityTextUpdateDate));
            dates.Add(Utilities.DateFromDecimal(PersonName.CprUpdateDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.NameStartDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.NameTerminationDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.SearchNameDate));
            //dates.Add(Utilities.DateFromDecimal(personInfo.PersonName.StatusDate));

            if (ContactAddress != null)
            {
                //dates.Add(Utilities.DateFromDecimal(personInfo.ContactAddress.ContactAddressDate));
                dates.Add(Utilities.DateFromDecimal(ContactAddress.UpdateDate));
            }
            //dates.AddRange(fromDate c in personInfo.CivilStates select Utilities.DateFromDecimal(c.MaritalStatusDate));
            //dates.AddRange(fromDate c in personInfo.CivilStates select Utilities.DateFromDecimal(c.MaritalEndDate));
            dates.AddRange(from c in CivilStates select Utilities.DateFromDecimal(c.AuthorityTextUpdateDate));
            dates.AddRange(from c in CivilStates select Utilities.DateFromDecimal(c.UpdateDateOfCpr));

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
            Func<decimal, Guid> cpr2uuidFunc = (cpr) => cpr2uuidConverter(cpr.ToString());

            var civilRegistrationStatus = Schemas.Util.Enums.ToCivilRegistrationStatus(PersonTotal.Status);
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

                NavnStruktur = NavnStrukturType.Create(PersonName.FirstName, PersonName.LastName),
                //TODO: Fill address when address schema is ready
                AndreAdresser = null,
                //No contact channels implemented
                KontaktKanal = null,
                //Next of kin (nearest relative). Not implemented
                NaermestePaaroerende = null,
                //TODO: Fill this object
                Virkning = VirkningType.Create(Utilities.GetMaxDate(PersonTotal.DateOfBirth, PersonName.NameStartDate, PersonTotal.StatusDate), null)
            };
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
            if (string.Equals(PersonTotal.Nationality, Constants.DenmarkNationality, StringComparison.OrdinalIgnoreCase))
            {
                ret.Item = new CprBorgerType()
                {
                    // No address note
                    AdresseNoteTekst = null,
                    // Church membership
                    // TODO : Where to fill fromDate?
                    FolkekirkeMedlemIndikator = PersonTotal.ChristianMark.HasValue,
                    //TODO: Fill address when class is ready
                    FolkeregisterAdresse = ToAdresseType(),

                    // Research protection
                    ForskerBeskyttelseIndikator = PersonTotal.DirectoryProtectionMarker == '1',
                    // TODO : What to put here? Could it be PersonTotal.AddressProtectionMarker?
                    NavneAdresseBeskyttelseIndikator = false,

                    PersonCivilRegistrationIdentifier = PersonTotal.PNR.ToDecimalString(),

                    PersonNationalityCode = new CountryIdentificationCodeType(),// DAL.Country.GetCountryAlpha2CodeByDanishName(PersonTotal.Nationality),
                    //PNR validity status
                    // TODO: Make sure that true is the correct value
                    PersonNummerGyldighedStatusIndikator = true,
                    //Use false since we do not have telephone numbers here
                    // TODO: Check if this is correct
                    TelefonNummerBeskyttelseIndikator = false,
                };
                ret.Virkning = VirkningType.Create(Utilities.GetMaxDate(PersonTotal.AddressDate, PersonName.AddressingNameDate, PersonTotal.StatusDate), null);
            }
            else if (
                (!string.Equals(PersonTotal.Nationality, Constants.CprNationality, StringComparison.OrdinalIgnoreCase))
                && (!string.Equals(PersonTotal.Nationality, Constants.Stateless, StringComparison.OrdinalIgnoreCase))
                )
            {
                ret.Item = new UdenlandskBorgerType()
                {
                    // Birth country.Not in DPR
                    FoedselslandKode = null,
                    // TODO: What is that?
                    PersonIdentifikator = "",
                    // Languages. Not implemented here
                    SprogKode = new CountryIdentificationCodeType[] { },
                    // Citizenships
                    PersonNationalityCode = new CountryIdentificationCodeType[] { 
                        CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.iso3166alpha2, CprBroker.DAL.Country.GetCountryAlpha2CodeByDanishName(PersonTotal.Nationality)) },
                    PersonCivilRegistrationReplacementIdentifier = PersonTotal.PNR.ToDecimalString(),
                };
                ret.Virkning = VirkningType.Create(Utilities.GetMaxDate(PersonTotal.StatusDate), null);
            }
            else
            {
                ret.Item = new UkendtBorgerType()
                {
                    PersonCivilRegistrationReplacementIdentifier = PersonTotal.PNR.ToDecimalString(),
                };
                ret.Virkning = VirkningType.Create(Utilities.GetMaxDate(PersonTotal.StatusDate), null);
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
                    CivilStatusKode = PersonTotal.PartCivilStatus,
                    TilstandVirkning = TilstandVirkningType.Create(Utilities.DateFromDecimal(PersonTotal.MaritalStatusDate))
                },
                LivStatus = new LivStatusType()
                {
                    LivStatusKode = PersonTotal.PartLifeStatus,
                    TilstandVirkning = TilstandVirkningType.Create(Utilities.DateFromFirstDecimal(PersonTotal.StatusDate)),
                },

                //TODO: Fill with orgfaelles:Gyldighed as soon as knowing what that is???
                //Gyldighed = null,

                // No extensions now
                LokalUdvidelse = null
            };
        }

        public RelationListeType ToRelationListeType(Func<decimal, Guid> cpr2uuidFunc, decimal? effectTimeDecimal, DPRDataContext dataContext)
        {
            var ret = new RelationListeType();
            // Now fill the relations
            var fatherPnr = PersonTotal.GetParent(this.PersonTotal.FatherMarker, this.PersonTotal.FatherPersonalOrBirthdate);
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

            var motherPnr = PersonTotal.GetParent(this.PersonTotal.MotherMarker, this.PersonTotal.MotherPersonalOrBirthDate);
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

        public AdresseType ToAdresseType()
        {
            return new AdresseType()
            {
                Item = new DanskAdresseType()
                {
                    AddressComplete = new CprBroker.Schemas.Part.AddressCompleteType()
                    {
                        AddressAccess = new CprBroker.Schemas.Part.AddressAccessType()
                        {
                            MunicipalityCode = PersonTotal.MunicipalityCode.ToDecimalString(),
                            StreetBuildingIdentifier = PersonTotal.HouseNumber,
                            StreetCode = PersonTotal.StreetCode.ToDecimalString()
                        },
                        AddressPostal = new CprBroker.Schemas.Part.AddressPostalType()
                        {
                            CountryIdentificationCode = null,
                            DistrictName = null,
                            DistrictSubdivisionIdentifier = null,
                            FloorIdentifier = PersonTotal.Floor,
                            MailDeliverySublocationIdentifier = null,
                            PostCodeIdentifier = PersonTotal.PostCode.ToDecimalString(),
                            PostOfficeBoxIdentifier = null,
                            StreetBuildingIdentifier = PersonTotal.HouseNumber,
                            StreetName = Street.StreetAddressingName,
                            StreetNameForAddressingName = null,
                            SuiteIdentifier = PersonTotal.Door,
                        }
                    },
                    // No address point
                    AddressPoint = null,
                    NoteTekst = null,
                    PolitiDistriktTekst = null,
                    PostDistriktTekst = PersonTotal.PostDistrictName,
                    SkoleDistriktTekst = null,
                    SocialDistriktTekst = null,
                    SogneDistriktTekst = null,
                    SpecielVejkodeIndikator = false,
                    SpecielVejkodeIndikatorSpecified = false,
                    UkendtAdresseIndikator = false,
                    ValgkredsDistriktTekst = null
                }
            };
        }
    }
}
