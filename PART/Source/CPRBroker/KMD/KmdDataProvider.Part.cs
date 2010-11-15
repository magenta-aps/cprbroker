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
        #region IPartReadDataProvider Members

        public CPRBroker.Schemas.Part.PersonRegistration Read(PersonIdentifier uuid, DateTime? effectDate, out QualityLevel? ql)
        {
            // TODO: Find a solution for effect date in KMD Read
            PersonRegistration ret = null;
            var resp = new EnglishAS78207Response(CallAS78207(uuid.CprNumber));
            var addressResp = CallAN08002(uuid.CprNumber);
            bool protectAddress = resp.AddressProtection.Equals("B") || resp.AddressProtection.Equals("L");

            ret = new PersonRegistration()
            {
                Attributes = new PersonAttributes()
                {
                    BirthDate = ToDateTime(resp.BirthDate),
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
                RegistrationDate = null,
                //TODO: fill relations for KMD read
                Relations = new PersonRelations()
                {
                    Children = new Effect<PersonRelation>[0],
                    Parents = new PersonRelation[0],
                    ReplacedBy = null,
                    Spouses = new Effect<PersonRelation>[0],
                    SubstituteFor = null,
                },
                //TODO: Fill states for KMD read
                States = new PersonStates()
                {
                    CivilStatus = new Effect<CPRBroker.Schemas.Part.Enums.MaritalStatus>()
                    {
                        StartDate = null,
                        EndDate = null,
                        //Value = null
                    },
                    LifeStatus = new Effect<CPRBroker.Schemas.Part.Enums.LifeStatus>()
                    {
                        StartDate = null,
                        EndDate = null,
                        //Value = null
                    }
                }
            };

            ql = QualityLevel.Cpr;
            return null;
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
                var ids = Array.ConvertAll<WS_AN08100.ReplyPeople, PersonIdentifier>(birthdateResp.OutputArrayRecord, (p) => new PersonIdentifier() { CprNumber = p.PNR, Birthdate = searchCriteria.BirthDate.Value });
                if (!MergeSearchResult(ids, ref ret))
                {
                    return ret;
                }
            }

            if (!string.IsNullOrEmpty(searchCriteria.CprNumber))
            {
                var cprNumberResp = CallAS78207(searchCriteria.CprNumber);
                var ids = new PersonIdentifier[] { new PersonIdentifier() { CprNumber = searchCriteria.CprNumber, Birthdate = ToDateTime(cprNumberResp.OutputRecord.DFOEDS) } };
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
                // TODO: Call AN08300
                var nameResp = this.CallAN08300(searchCriteria.Name, searchCriteria.Gender);
                //TODO: fill PersonIdentifier birthdate
                var ids = Array.ConvertAll<WS_AN08300.ReplyPeople, PersonIdentifier>(nameResp, (p) => new PersonIdentifier() { CprNumber = p.PNR });
                if (!MergeSearchResult(ids, ref ret))
                {
                    return ret;
                }
            }
            //TODO: Is it better to assign ids for each individual result? pros: ID assigment for more persons; cons: uuid explosion if many brokers are there
            CPRBroker.DAL.Part.PersonMapping.AssignGuids(ret);
            return ret;
        }

        private bool MergeSearchResult(PersonIdentifier[] ids, ref PersonIdentifier[] finalResult)
        {
            return true;
        }

        #endregion

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

    }
}
