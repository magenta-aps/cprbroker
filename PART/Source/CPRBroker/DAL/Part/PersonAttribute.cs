using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonAttribute
    {
        public Schemas.Part.AttributListeType ToXmlType()
        {
            var ret = new AttributListeType()
            {
                /*Egenskaber = new EgenskaberType[]
                {
                    new EgenskaberType()
                    {
                        PersonBirthDateStructure = new PersonBirthDateStructureType()
                        {
                            //TODO: Check that new DateTime is the correct value here
                           BirthDate=this.BirthDate.HasValue?this.BirthDate.Value : new DateTime(), 
                            BirthDateUncertaintyIndicator = this.BirthdateUncertainty,
                        },
                        PersonGenderCode = DAL.Part.Gender.GetPartGender(this.GenderId),
                        PersonNameStructure = new CprBroker.Schemas.Part.PersonNameStructureType( )
                        {
                            PersonGivenName=this.FirstName,
                            PersonMiddleName=this.MiddleName,
                            PersonSurnameName=this.LastName
                        },                        
                        Virkning = VirkningType.Create(null,null),
                    },
                },
                RegisterOplysninger = new RegisterOplysningerType[] { new RegisterOplysningerType() },
                LokalUdvidelse = null*/
            };

            if (this.CprData != null)
            {
                //ret.RegisterOplysninger[0].Item = CprData.ToXmlType();
            }
            else if (this.ForeignCitizenData != null)
            {
                //ret.RegisterOplysninger[0].Item = ForeignCitizenData.ToXmlType();
            }
            else if (this.UnknownCitizenData != null)
            {
                //ret.RegisterOplysninger[0].Item = UnknownCitizenData.ToXmlType();
            }
            return ret;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonAttribute>(pa => pa.CprData);
            loadOptions.LoadWith<PersonAttribute>(pa => pa.Effect);
            loadOptions.LoadWith<PersonAttribute>(pa => pa.ForeignCitizenData);
            loadOptions.LoadWith<PersonAttribute>(pa => pa.UnknownCitizenData);

            CprData.SetChildLoadOptions(loadOptions);
        }

        public static PersonAttribute FromXmlType(Schemas.Part.AttributListeType partAttributes)
        {
            //var oo = partAttributes.Egenskaber[0];
            var ret = new PersonAttribute()
            {
                /*BirthDate = oo.PersonBirthDateStructure.BirthDate,
                BirthdateUncertainty = oo.PersonBirthDateStructure.BirthDateUncertaintyIndicator,
                Effect = Effect.FromXmlType(oo.Virkning),
                GenderId = DAL.Part.Gender.GetPartCode(oo.PersonGenderCode),
                FirstName = oo.PersonNameStructure.PersonGivenName,
                MiddleName = oo.PersonNameStructure.PersonMiddleName,
                LastName = oo.PersonNameStructure.PersonSurnameName*/
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
