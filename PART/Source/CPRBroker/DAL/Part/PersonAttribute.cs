using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonAttribute
    {
        public Schemas.Part.AttributListeType ToXmlType()
        {
            // TODO: Add support for name date
            var ret = new AttributListeType()
            {
                Egenskaber = new List<EgenskaberType>
                (
                    new EgenskaberType[]
                    {
                        new EgenskaberType()
                        {
                            PersonBirthDateStructure = new PersonBirthDateStructureType()
                            {
                               BirthDate=this.BirthDate.HasValue?this.BirthDate.Value : DateTime.MinValue, 
                                BirthDateUncertaintyIndicator = false,
                            },
                            PersonGenderCode = DAL.Part.Gender.GetPartGender(this.GenderId),
                            PersonNameStructure = new CprBroker.Schemas.PersonNameStructureType( ){PersonGivenName=this.FirstName,PersonMiddleName=this.MiddleName,PersonSurnameName=this.LastName},
                            RegisterOplysninger = new RegisterOplysningerType(),
                            Virkning = VirkningType.Create(null,null),
                        },
                    }
                )
            };

            if (this.CprData != null)
            {
                ret.Egenskaber[0].RegisterOplysninger.Item = CprData.ToXmlType();
            }
            else if (this.ForeignCitizenData != null)
            {
                ret.Egenskaber[0].RegisterOplysninger.Item = ForeignCitizenData.ToXmlType();
            }
            else if (this.UnknownCitizenData != null)
            {
                ret.Egenskaber[0].RegisterOplysninger.Item = UnknownCitizenData.ToXmlType();
            }
            return ret;
        }

        public static PersonAttribute FromXmlType(Schemas.Part.AttributListeType partAttributes)
        {
            var oo = partAttributes.Egenskaber[0];
            // TODO: Add support for contact channels and other addresses
            // TODO: Add support for name date
            var ret = new PersonAttribute()
            {
                BirthDate = oo.PersonBirthDateStructure.BirthDate,
                GenderId = DAL.Part.Gender.GetPartCode(oo.PersonGenderCode),
                FirstName = oo.PersonNameStructure.PersonGivenName,
                MiddleName=oo.PersonNameStructure.PersonMiddleName,
                LastName=oo.PersonNameStructure.PersonSurnameName
            };
            /*
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
            }*/
            return ret;
        }
    }
}
