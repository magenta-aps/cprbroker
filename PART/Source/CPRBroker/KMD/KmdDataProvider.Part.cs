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
                        NationalityCountryCode =DAL.Country.GetCountryAlpha2CodeByKmdCode( resp.NationalityCode),
                        NickName = null,
                        PersonName = new Effect<PersonNameStructureType>() 
                        {
                            StartDate = ToDateTime(resp.NameDate),
                            EndDate = null,
                            Value = new PersonNameStructureType(resp.FirstName, resp.LastName)
                        },
                        PopulationAddress = resp.ToPartAddress(),
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
            throw new NotImplementedException();
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

    }
}
