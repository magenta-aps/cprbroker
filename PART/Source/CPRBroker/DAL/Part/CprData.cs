using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL.Part
{
    public partial class CprData
    {
        public Schemas.Part.CprData ToXmlType()
        {
            return new Schemas.Part.CprData()
            {
                AddressingName = this.AddressingName,
                BirthDateUncertainty = this.IsBirthdateUncertain,
                CprNumber = this.CprNumber,
                Gender = Gender.GetPartGender(this.PersonAttribute.GenderId),
                IndividualTrackStatus = this.IndividualTrackStatus,
                NameAndAddressProtection = this.HasNameAndAddressProtection,
                NationalityCountryCode = this.NationalityCountryAlpha2Code,
                NickName = this.NickName,
                PersonName = new CPRBroker.Schemas.Part.Effect<CPRBroker.Schemas.PersonNameStructureType>()
                {
                    StartDate = NameStartDate,
                    EndDate = NameEndDate,
                    Value = new CPRBroker.Schemas.PersonNameStructureType 
                    {
                        PersonGivenName = this.FirstName,
                        PersonMiddleName = this.MiddleName,
                        PersonSurnameName = this.LastName
                    },
                },
                // TODO: fill address
                PopulationAddress = null,
            };
        }
    }
}
