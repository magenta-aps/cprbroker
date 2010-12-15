using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Providers.KMD.WS_AS78207;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Part;
using CPRBroker.DAL;

namespace CPRBroker.Providers.KMD
{
    public partial class KmdDataProvider : IPartReadDataProvider, IPartSearchDataProvider
    {
        public static readonly Guid ActorId = new Guid("{A4E9B3E0-275F-4b76-AADB-4398AA56B871}");

        #region IPartReadDataProvider Members

        public CPRBroker.Schemas.Part.PersonRegistration Read(PersonIdentifier uuid, DateTime? effectDate, out QualityLevel? ql)
        {
            // TODO: Find a solution for effect date in KMD Read
            PersonRegistration ret = null;
            var resp = new EnglishAS78207Response(CallAS78207(uuid.CprNumber));
            var addressResp = CallAN08002(uuid.CprNumber);
            var relationsResponse = CallAN08010(uuid.CprNumber);


            bool protectAddress = resp.AddressProtection.Equals("B") || resp.AddressProtection.Equals("L");

            ret = new PersonRegistration()
            {
                Attributes = new PersonAttributes()
                {
                    BirthDate = ToDateTime(resp.BirthDate).Value,
                    ContactChannel = new ContactChannel[0],
                    Gender = ToPartGender(uuid.CprNumber),
                    Name = new Effect<string>()
                    {
                        StartDate = ToDateTime(resp.NameDate),
                        EndDate = null,
                        Value = new PersonNameStructureType(resp.FirstName, resp.LastName).ToString()
                    },
                    // TODO: check if other addresses can be filled from another service (e.g. AN80002)
                    OtherAddresses = new CPRBroker.Schemas.Part.Address[0],
                    //TODO: Handle the case of Foreign data or unknown data
                    PersonData = new CprData()
                    {
                        AddressingName = resp.AddressingName,
                        BirthDateUncertainty = false,
                        CprNumber = resp.PNR,
                        Gender = ToPartGender(uuid.CprNumber),

                        IndividualTrackStatus = false,
                        NameAndAddressProtection = protectAddress,
                        NationalityCountryCode = DAL.Country.GetCountryAlpha2CodeByKmdCode(resp.NationalityCode),
                        NickName = null,
                        PersonName = new Effect<PersonNameStructureType>()
                        {
                            StartDate = ToDateTime(resp.NameDate),
                            EndDate = null,
                            Value = new PersonNameStructureType(resp.FirstName, resp.LastName)
                        },
                        PopulationAddress = resp.ToPartAddress(),
                    }
                },
                ActorId = ActorId,
                //TODO: Pick the correct date for registrations from KMD instead of StatusDate
                RegistrationDate = ToDateTime(resp.StatusDate).Value,
                //TODO: fill relations for KMD read
                Relations = new PersonRelations()
                {
                    Children = new Effect<PersonRelation>[0],
                    Parents = new PersonRelation[0],
                    ReplacedBy = null,
                    Spouses = new Effect<PersonRelation>[0],
                    SubstituteFor = null,
                },
                States = new PersonStates()
                {
                    CivilStatus = new Effect<CPRBroker.Schemas.Part.Enums.MaritalStatus>()
                    {
                        StartDate = ToDateTime(resp.MaritalStatusDate),
                        EndDate = null,
                        Value = Schemas.Util.Enums.ToPartMaritalStatus(resp.MaritallStatusCode[0]),
                    },
                    LifeStatus = new Effect<CPRBroker.Schemas.Part.Enums.LifeStatus>()
                    {
                        //TODO: Status date may not be the correct field (for example, the status may have changed from 01 to  07 at the date, but the life status is still alive)
                        StartDate = ToDateTime(resp.StatusDate),
                        EndDate = null,
                        Value = Schemas.Util.Enums.ToLifeStatus(GetCivilRegistrationStatus(resp.StatusKmd, resp.StatusCpr))
                    },
                }
            };

            if (relationsResponse.OutputArrayRecord != null)
            {
                var relationIdentifiers = (from rel in relationsResponse.OutputArrayRecord where !string.IsNullOrEmpty(rel.PNR.Replace("-", "")) select new PersonIdentifier() { CprNumber = rel.PNR.Replace("-", "") }).ToArray();
                DAL.Part.PersonMapping.AssignGuids(relationIdentifiers);
                ret.Relations.Children = GetPersonRelations(relationsResponse.OutputArrayRecord, relationIdentifiers, RelationTypes.Baby, RelationTypes.ChildOver18);
                ret.Relations.Parents = Array.ConvertAll<Effect<PersonRelation>, PersonRelation>(GetPersonRelations(relationsResponse.OutputArrayRecord, relationIdentifiers, RelationTypes.Parents), (rel) => rel.Value);
                ret.Relations.Spouses = GetPersonRelations(relationsResponse.OutputArrayRecord, relationIdentifiers, RelationTypes.Spouse, RelationTypes.Partner);
            }

            ql = QualityLevel.Cpr;
            return ret;
        }


        public CPRBroker.Schemas.Part.PersonRegistration[] List(PersonIdentifier[] uuids, DateTime? effectDate, out QualityLevel? ql)
        {
            // TODO: implement List operation on KMD after Read is completed
            throw new NotImplementedException();
        }

        #endregion

        #region IPartSearchDataProvider Members

        public PersonIdentifier[] Search(CPRBroker.Schemas.Part.PersonSearchCriteria searchCriteria, DateTime? effectDate, out QualityLevel? ql)
        {
            ql = QualityLevel.Cpr;
            PersonIdentifier[] ret = new PersonIdentifier[0];

            if (searchCriteria.BirthDate.HasValue)
            {
                var birthdateResp = CallAN08100(searchCriteria.BirthDate.Value, searchCriteria.Gender);
                var ids = Array.ConvertAll<WS_AN08100.ReplyPeople, PersonIdentifier>(birthdateResp.OutputArrayRecord, (p) => new PersonIdentifier() { CprNumber = p.PNR });
                if (!MergeSearchResult(ids, ref ret))
                {
                    return ret;
                }
            }

            if (!string.IsNullOrEmpty(searchCriteria.CprNumber))
            {
                var cprNumberResp = CallAS78207(searchCriteria.CprNumber);
                var ids = new PersonIdentifier[] { new PersonIdentifier() { CprNumber = searchCriteria.CprNumber } };
                if (!MergeSearchResult(ids, ref ret))
                {
                    return ret;
                }
            }

            if (!string.IsNullOrEmpty(searchCriteria.NationalityCountryCode))
            {
                // TODO: Search by nationality
            }

            if (!searchCriteria.Name.IsEmpty || searchCriteria.Gender.HasValue)
            {
                var nameResp = this.CallAN08300(searchCriteria.Name, searchCriteria.Gender);
                //TODO: fill PersonIdentifier birthdate
                var ids = Array.ConvertAll<WS_AN08300.ReplyPeople, PersonIdentifier>(nameResp, (p) => new PersonIdentifier() { CprNumber = p.PNR });
                if (!MergeSearchResult(ids, ref ret))
                {
                    return ret;
                }
            }
            CPRBroker.DAL.Part.PersonMapping.AssignGuids(ret);
            return ret;
        }

        #endregion

        #region Utility methods

        //TODO: Convert to anonymous method withn Search method
        private bool MergeSearchResult(PersonIdentifier[] ids, ref PersonIdentifier[] finalResult)
        {
            return true;
        }

        private Schemas.Part.Enums.Gender ToPartGender(string cprNumber)
        {
            int cprNum = int.Parse(cprNumber[cprNumber.Length - 1].ToString());
            if (cprNum % 2 == 0)
            {
                return Schemas.Part.Enums.Gender.Female;
            }
            else
            {
                return Schemas.Part.Enums.Gender.Male;
            }
        }
        private char FromPartGender(Schemas.Part.Enums.Gender? gender)
        {
            switch (gender)
            {
                case Schemas.Part.Enums.Gender.Male:
                    return 'M';
                    break;
                case Schemas.Part.Enums.Gender.Female:
                    return 'K';
                    break;
                default:
                    return '*';
                    break;
            }
        }

        private Effect<Schemas.Part.PersonRelation>[] GetPersonRelations(WS_AN08010.ReplyPerson[] persons, PersonIdentifier[] personIdentifiers, params string[] typeFilters)
        {
            if (persons == null)
            {
                persons = new WS_AN08010.ReplyPerson[0];
            }

            List<Effect<Schemas.Part.PersonRelation>> ret = new List<Effect<Schemas.Part.PersonRelation>>();

            for (int i = 0; i < persons.Length; i++)
            {
                var person = persons[i];
                if (Array.IndexOf<string>(typeFilters, person.Type) != -1 && person.IsUnknown == false)
                {
                    ret.Add(new Effect<Schemas.Part.PersonRelation>()
                    {
                        StartDate = null,
                        EndDate = null,
                        Value = new CPRBroker.Schemas.Part.PersonRelation()
                        {
                            TargetUUID = personIdentifiers[i].UUID.Value,
                        }
                    });
                }
            }
            return ret.ToArray();
        }

        internal static decimal GetCivilRegistrationStatus(string kmdStatus, string cprStatus)
        {
            int iKmd = int.Parse(kmdStatus);
            int iCpr = int.Parse(cprStatus);
            if (iKmd == 1)// >10
            {
                return iCpr * 10;
            }
            else
            {
                // TODO: differentiate betwee 01, 03, 05 & 07 because cprStatus is always 0 here
                return 1;
            }
        }
        #endregion

    }
}
