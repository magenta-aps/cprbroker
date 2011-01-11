using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace CprBroker.DAL.Part
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
                //Gender = Gender.GetPartGender(this.PersonAttribute.GenderId),
                IndividualTrackStatus = this.IndividualTrackStatus,
                NameAndAddressProtection = this.HasNameAndAddressProtection,
                NationalityCountryCode = this.NationalityCountryAlpha2Code,
                NickName = this.NickName,
                PersonName = new CprBroker.Schemas.Part.Effect<CprBroker.Schemas.PersonNameStructureType>()
                {
                    StartDate = NameStartDate,
                    EndDate = NameEndDate,
                    Value = new CprBroker.Schemas.PersonNameStructureType
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

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<CprData>(cpr => cpr.Address);
        }

        public static CprData FromXmlType(Schemas.Part.CprData partCprData)
        {
            // TODO: Implement CprData.FromXmlType()
            return new CprData()
            {
                AddressingName = partCprData.AddressingName,
                IsBirthdateUncertain = partCprData.BirthDateUncertainty,
                CprNumber = partCprData.CprNumber,
                //GenderId = Gender.GetPartCode(partCprData.Gender),
                IndividualTrackStatus = partCprData.IndividualTrackStatus,
                HasNameAndAddressProtection = partCprData.NameAndAddressProtection,
                NationalityCountryAlpha2Code = partCprData.NationalityCountryCode,
                NickName = partCprData.NickName,
                NameStartDate = partCprData.PersonName.StartDate,
                NameEndDate = partCprData.PersonName.EndDate,

                FirstName = partCprData.PersonName.Value.PersonGivenName,
                MiddleName = partCprData.PersonName.Value.PersonMiddleName,
                LastName = partCprData.PersonName.Value.PersonSurnameName
                ,
                // TODO: fill address
                Address = null,
            };
        }
    }
}
