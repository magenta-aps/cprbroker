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

        // TODO: Remove this method
        [Obsolete]
        internal PersonRegistration ToPersonRegistration(DateTime? effectTime, DPRDataContext dataContext)
        {
            var civilRegistrationStatus = Schemas.Util.Enums.ToCivilRegistrationStatus(PersonTotal.Status);
            var effectTimeDecimal = Utilities.DecimalFromDate(effectTime);
            PersonNameStructureType tempPersonName = new PersonNameStructureType(PersonName.FirstName, PersonName.LastName);
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
                        PersonName = new Effect<PersonNameStructureType>()
                        {
                            StartDate = Utilities.DateFromDecimal(PersonName.NameStartDate),
                            EndDate = Utilities.DateFromDecimal(PersonName.NameTerminationDate),
                            Value = tempPersonName
                        },
                        AddressingName = PersonName.AddressingName,
                        BirthDateUncertainty = null,

                        CprNumber = Utilities.PersonCivilRegistrationIdentifierFromDecimal(PersonName.PNR),

                        Gender = Utilities.GenderFromChar(PersonTotal.Sex),
                        //TODO: correct this field
                        IndividualTrackStatus = true,

                        NameAndAddressProtection = HasProtection,
                        NationalityCountryCode = DAL.Country.GetCountryAlpha2CodeByDanishName(PersonTotal.Nationality),
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
                States = new PersonStates()
                {
                    LifeStatus = new Effect<CprBroker.Schemas.Part.Enums.LifeStatus>()
                    {
                        StartDate = Utilities.DateFromDecimal(PersonTotal.StatusDate),
                        // Handled later using the first PersonTotal that has a later StatusDate
                        EndDate = null,
                        Value = Schemas.Util.Enums.ToLifeStatus(PersonTotal.Status)
                    }
                }
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

            //TODO: Fill the relations in DPR
            /**************/
            /*
            ret.Relations.Parents = Array.ConvertAll<Guid, PersonRelation>
            (
                DAL.Part.PersonMapping.AssignGuids
                (
                    (
                        from pers in Child.PersonParentsExpression.Compile()(PersonTotal.PNR, dataContext)
                        select new PersonIdentifier()
                        {
                            CprNumber = pers.PNR.ToString("D2")
                        }
                    ).ToArray()
                ),
                (id) => new PersonRelation() { TargetUUID = id }
            );
            ret.Relations.Parents = DAL.Part.PersonMapping.AssignGuids<decimal, PersonRelation>(
                 (
                        from pers in Child.PersonParentsExpression.Compile()(PersonTotal.PNR, dataContext)
                        select pers.PNR
                    ).ToArray(),
                    (pnr) => null,
                    (pnr) => new PersonIdentifier() { CprNumber = pnr.ToString("D2") },
                    (dd,id)=>dd.TargetUUID = id);


            ret.Relations.Children = DAL.Part.PersonMapping.AssignGuids<PersonTotal, Effect<PersonRelation>>
            (
                Child.PersonChildrenExpression.Compile()(effectTimeDecimal.Value, PersonTotal.PNR, dataContext).ToArray(),
                (child) => new Effect<PersonRelation>()
                {
                    StartDate = Utilities.DateFromDecimal(child.DateOfBirth),
                    EndDate = null,
                    Value = new PersonRelation()
                },
                (child) => new PersonIdentifier()
                {
                    CprNumber = child.PNR.ToString("D2"),
                },
                (rel, id) => rel.Value.TargetUUID = id
            );

            ret.Relations.Spouse = DAL.Part.PersonMapping.AssignGuids<CivilStatus, Effect<PersonRelation>>
            (
                civilStates,
                (civilStatus) => new Effect<PersonRelation>()
                {
                    StartDate = Utilities.DateFromDecimal(civilStatus.MaritalStatusDate),
                    EndDate = Utilities.DateFromDecimal(civilStatus.MaritalEndDate),
                    Value = new PersonRelation()
                    {
                    }
                },
                (civilStatus) => new PersonIdentifier()
                {
                    CprNumber = civilStatus.SpousePNR.Value.ToString("D2"),
                },
                (p, id) =>
                {
                    p.Value.TargetUUID = id;
                }
            );
            */
            return ret;
        }

        private DateTime[] GetCandidateRegistrationDates()
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
            //dates.AddRange(from c in personInfo.CivilStates select Utilities.DateFromDecimal(c.MaritalStatusDate));
            //dates.AddRange(from c in personInfo.CivilStates select Utilities.DateFromDecimal(c.MaritalEndDate));
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


        /// <summary>
        /// Creates a cross product of the person database objects based on the possible registration dates
        /// </summary>
        /// <param name="personInfos"></param>
        /// <returns></returns>
        public static System.Collections.Generic.ICollection<PersonInfo> Populate(ICollection<PersonInfo> personInfos, DateTime? effectFromDate, DateTime? effectToDate)
        {
            var ret = new List<PersonInfo>();

            foreach (var personInfo in personInfos)
            {
                var personInfoAsQueryable = new PersonInfo[] { personInfo }.AsQueryable();
                var dates = new List<DateTime>(personInfo.GetCandidateRegistrationDates());

                dates = dates
                    .Where(
                        (d) =>
                                (!effectFromDate.HasValue || d >= effectFromDate.Value)
                            && (!effectToDate.HasValue || d <= effectToDate.Value)
                        )
                    .Distinct()
                    .ToList();

                ret.AddRange
                (
                    from pi in personInfoAsQueryable
                    from d in dates.AsQueryable()
                    orderby d descending
                    select new PersonInfo()
                    {
                        //TODO : Filter these records by dates
                        CivilStates = pi.CivilStates,
                        ContactAddress = pi.ContactAddress,
                        HasProtection = pi.HasProtection,
                        PersonName = pi.PersonName,
                        PersonTotal = pi.PersonTotal,
                        Street = pi.Street,
                    }
                );
            }
            return ret;
        }

        internal RegistreringType1 ToRegisteringType1(DateTime? effectTime, Func<string, Guid> cpr2uuidConverter, DPRDataContext dataContext)
        {
            Func<decimal, Guid> cpr2uuidFunc = (cpr) => cpr2uuidConverter(cpr.ToString());

            var civilRegistrationStatus = Schemas.Util.Enums.ToCivilRegistrationStatus(PersonTotal.Status);
            var effectTimeDecimal = Utilities.DecimalFromDate(effectTime);
            PersonNameStructureType tempPersonName = new PersonNameStructureType(PersonName.FirstName, PersonName.LastName);


            RegistreringType1 ret = new RegistreringType1()
            {
                AttributListe = new AttributListeType()
                {                    
                    // Filled later in the code below
                    Egenskaber=null,
                    // Filled later in the code below
                    RegisterOplysninger = null,
                    // Health information not implemented
                    SundhedsOplysninger = null,
                    // No extensions at the moment
                    LokalUdvidelse = null
                },

                AktoerTekst = Constants.ActorId.ToString(),
                CommentText = Constants.CommentText,
                //Fill as Rettet (Updated)
                LivscyklusKode = Constants.UpdatedLifecycleStatusCode,
                // FIll with empty object and put the details later
                RelationListe = new RelationListeType()
                {
                    
                },
                TidspunktDatoTid = TidspunktType.Create(this.RegistrationDate),
                TilstandListe = new TilstandListeType()
                {                    
                    CivilStatus = null,
                    LivStatus=null,
                    //TODO: Fill with orgfaelles:Gyldighed as soon as knowing what that is???
                    //Gyldighed = null,

                    // No extensions now
                    LokalUdvidelse = null
                },
                //TODO: Pass parameters to this method
                Virkning = VirkningType.Create(null, null)
            };

            ret.AttributListe.Egenskaber = new EgenskaberType[] 
            {
                new EgenskaberType()
                {
                    PersonBirthDateStructure = new CprBroker.Schemas.Part.PersonBirthDateStructureType()
                    {
                        BirthDate = Utilities.DateFromDecimal(PersonTotal.DateOfBirth).Value,                                
                        BirthDateUncertaintyIndicator = PersonTotal.DateOfBirth < 0
                    },
                    // Birth registration authority
                    //TODO: Is this assignment correct?
                    fodselsregistreringmyndighed=PersonTotal.BirthPlaceOfRegistration,
                    // Place of birth
                    foedested=PersonTotal.BirthplaceText,
                    PersonGenderCode = Utilities.PersonGenderCodeTypeFromChar( PersonTotal.Sex),
                    PersonNameStructure = tempPersonName,
                    //TODO: Fill address when address schema is ready
                    AndreAdresser= null,
                    //No contact channels implemented
                    Kontaktkanal=null,
                    //Next of kin (nearest relative). Not implemented
                    NaermestePaaroerende=null,
                    //TODO: Fill this object ******************************************************************************************
                    Virkning = VirkningType.Create(null,null)
                }
            };

            // Now fill person data
            ret.AttributListe.RegisterOplysninger = new RegisterOplysningerType[]
            {
                new RegisterOplysningerType()
                {
                    Item = null,
                }
            };
            // Now create the appropriate object based on nationality
            if (string.Equals(PersonTotal.Nationality, Constants.DenmarkNationality, StringComparison.OrdinalIgnoreCase))
            {
                ret.AttributListe.RegisterOplysninger[0].Item = new CprBorgerType()
                {
                    // No address note
                    AdresseNote = null,
                    // Church membership
                    // TODO : Where to fill from?
                    FolkekirkeMedlemsskab = PersonTotal.ChristianMark.HasValue,
                    //TODO: Fill address when class is ready
                    FolkeregisterAdresse = null,
                    // TODO: What is this (same name as above)
                    FolkeRegisterAdresse = "",

                    // Research protection
                    ForskerBeskyttelseIndikator = PersonTotal.DirectoryProtectionMarker == '1',
                    // TODO : What to put here? Could it be PersonTotal.AddressProtectionMarker?
                    PersonInformationProtectionIndicator = false,

                    PersonCivilRegistrationIdentifier = Utilities.PersonCivilRegistrationIdentifierFromDecimal(PersonTotal.PNR),

                    PersonNationalityCode = DAL.Country.GetCountryAlpha2CodeByDanishName(PersonTotal.Nationality),
                    //PNR validity status
                    PersonNummerGyldighedStatus = true,
                    //Use false since we do not have telephone numbers here
                    // TODO: Check if this is correct
                    TelefonNummerBeskyttelseIndikator = false,
                };
            }
            else if (
                (!string.Equals(PersonTotal.Nationality, Constants.CprNationality, StringComparison.OrdinalIgnoreCase))
                && (!string.Equals(PersonTotal.Nationality, Constants.Stateless, StringComparison.OrdinalIgnoreCase))
                )
            {
                ret.AttributListe.RegisterOplysninger[0].Item = new UdenlandskBorgerType()
                {
                    // Birth country.Not in DPR
                    FoedselsLand = null,
                    // TODO: What is that?
                    PersonID = "",
                    // Languages. Not implemented here
                    Sprog = new string[] { },
                    // Citizenships
                    Statsborgerskaber = new string[] { DAL.Country.GetCountryAlpha2CodeByDanishName(PersonTotal.Nationality) },
                    PersonCivilRegistrationReplacementIdentifier = Utilities.PersonCivilRegistrationIdentifierFromDecimal(PersonTotal.PNR),
                };
            }
            else
            {
                ret.AttributListe.RegisterOplysninger[0].Item = new UkendtBorgerType()
                {
                    PersonCivilRegistrationReplacementIdentifier = Utilities.PersonCivilRegistrationIdentifierFromDecimal(PersonTotal.PNR),
                };
            }

            // TODO: Is it OK to get the full history here?
            ret.TilstandListe.CivilStatus= new CivilStatusType[]
                    {
                        new CivilStatusType()
                        {
                            Status = PersonTotal.PartCivilStatus,
                            TilstandVirkning = TilstandVirkningType.Create(Utilities.DateFromDecimal(PersonTotal.MaritalStatusDate))
                        }
                    },
                    LivStatus = new LivStatusType[]
                    {
                        new LivStatusType()
                        {
                            Status = PersonTotal.PartLifeStatus,
                            TilstandVirkning = TilstandVirkningType.Create(Utilities.DateFromFirstDecimal(PersonTotal.StatusDate)),
                        }
                    },

            // Now fill the relations
            var fatherPnr = PersonTotal.GetParent(this.PersonTotal.FatherMarker, this.PersonTotal.FatherPersonalOrBirthdate);
            if (fatherPnr.HasValue)
            {
                ret.RelationListe.Fader = new PersonRelationType[]
                {
                    PersonRelationType.Create(
                        cpr2uuidFunc( fatherPnr.Value),
                        ret.AttributListe.Egenskaber[0].PersonBirthDateStructure.BirthDate,
                        null
                    )
                };
            }


            var motherPnr = PersonTotal.GetParent(this.PersonTotal.MotherMarker, this.PersonTotal.MotherPersonalOrBirthDate);
            if (motherPnr.HasValue)
            {
                ret.RelationListe.Moder = new PersonRelationType[]
                {
                    PersonRelationType.Create
                        (cpr2uuidFunc( motherPnr.Value) ,
                        ret.AttributListe.Egenskaber[0].PersonBirthDateStructure.BirthDate,
                        null
                    )
                };
            }

            ret.RelationListe.Boern = PersonFlerRelationType.CreateList(
                Array.ConvertAll<PersonTotal, Guid>
                (
                    Child.PersonChildrenExpression.Compile()(effectTimeDecimal.Value, PersonTotal.PNR, dataContext).ToArray(),
                    (pt) => cpr2uuidFunc(pt.PNR)
                ));

            // TODO : Fill adult children
            ret.RelationListe.ForaeldremyndgihdedsBoern = null;

            ret.RelationListe.Aegtefaelle =
                (from civ in dataContext.CivilStatus
                 where civ.PNR == PersonTotal.PNR
                    && civ.MaritalStatusDate <= effectTimeDecimal.Value
                    && civ.SpousePNR.HasValue && civ.SpousePNR.Value > 0
                    && !civ.CorrectionMarker.HasValue
                 orderby civ.MaritalStatusDate
                 select new PersonRelationType()
                 {
                     ReferenceIDTekst = cpr2uuidFunc(civ.SpousePNR.Value).ToString(),
                     CommentText = "",
                     Virkning = VirkningType.Create(Utilities.DateFromDecimal(civ.MaritalStatusDate), Utilities.DateFromDecimal((civ.MaritalEndDate)))
                 }).ToArray();

            // TODO: Add custody relations
            ret.RelationListe.ForaeldremyndgihdedsIndehaver = null;

            ret.RelationListe.RegistreretPartner = null;
            //TODO: Fill Legal capacity Legal guardian for person
            ret.RelationListe.RetligHandleevneVaergeForPersonen = null;

            //TODO: Fill Legal capacity Guardianship Proprietor
            ret.RelationListe.RetligHandleevneVaergemaalsIndehaver = null;

            return ret;
        }
    }
}
