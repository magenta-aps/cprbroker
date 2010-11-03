using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas.Part;

namespace CPRBroker.DAL.Part
{
    public partial class PersonAttribute
    {
        public Schemas.Part.PersonAttributes ToXmlType()
        {
            var ret = new PersonAttributes()
                {
                    BirthDate = this.BirthDate,
                    ContactAddresses = new Effect<CPRBroker.Schemas.Part.Address>[0],
                    ContactChannel = new Effect<ContactChannel>[0],
                    Gender = DAL.Part.Gender.GetPartGender(this.GenderId),
                    Name = new Effect<string>()
                    {
                        StartDate = null,
                        EndDate = null,
                        Value = null,
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
    }
}
