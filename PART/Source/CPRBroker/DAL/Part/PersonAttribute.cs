using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonAttribute
    {
        public Schemas.Part.PersonAttributes ToXmlType()
        {
            // TODO: Add support for contact channels and other addresses
            // TODO: Add support for name date
            var ret = new PersonAttributes()
                {
                    BirthDate = this.BirthDate,
                    OtherAddresses = new CprBroker.Schemas.Part.Address[0],
                    ContactChannel = new ContactChannel[0],
                    Gender = DAL.Part.Gender.GetPartGender(this.GenderId),
                    Name = new Effect<string>()
                    {
                        StartDate = null,
                        EndDate = null,
                        Value = Name,
                    },
                    PersonData = null
                };

            if (this.CprData != null)
            {
                ret.PersonData = CprData.ToXmlType();
            }
            else if (this.ForeignCitizenData != null)
            {
                ret.PersonData = ForeignCitizenData.ToXmlType();
            }
            else if (this.UnknownCitizenData != null)
            {
                ret.PersonData = UnknownCitizenData.ToXmlType();
            }
            return ret;
        }

        public static PersonAttribute FromXmlType(Schemas.Part.PersonAttributes partAttributes)
        {
            // TODO: Add support for contact channels and other addresses
            // TODO: Add support for name date
            var ret = new PersonAttribute()
            {
                BirthDate = partAttributes.BirthDate,
                GenderId = DAL.Part.Gender.GetPartCode(partAttributes.Gender),
                Name = partAttributes.Name.Value
            };
            if (partAttributes.PersonData is Schemas.Part.CprData)
            {
                ret.CprData = DAL.Part.CprData.FromXmlType(partAttributes.PersonData as Schemas.Part.CprData);
            }
            else if (partAttributes.PersonData is Schemas.Part.ForeignCitizenData)
            {
                ret.ForeignCitizenData = DAL.Part.ForeignCitizenData.FromXmlType(partAttributes.PersonData as Schemas.Part.ForeignCitizenData);
            }
            else if (partAttributes.PersonData is Schemas.Part.UnknownCitizenData)
            {
                ret.UnknownCitizenData = DAL.Part.UnknownCitizenData.FromXmlType(partAttributes.PersonData as Schemas.Part.UnknownCitizenData);
            }
            return ret;
        }
    }
}
