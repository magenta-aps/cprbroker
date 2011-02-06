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
                Egenskab = new EgenskabType[] { ToEgenskabType() },

                RegisterOplysning = new RegisterOplysningType[] { new RegisterOplysningType() },
                LokalUdvidelse = null
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

        public EgenskabType ToEgenskabType()
        {
            return new EgenskabType()
            {
                BirthDate = this.BirthDate,
                PersonGenderCode = DAL.Part.Gender.GetPartGender(this.GenderId),
                NavnStruktur = new NavnStrukturType()
                {
                    KaldenavnTekst = NickName,
                    NoteTekst = NameNoteText,
                    PersonNameForAddressingName = AddressingName,
                    PersonNameStructure = new NavnStruktur(FirstName, MiddleName, LastName),
                },
                FoedestedNavn = BirthPlace,
                FoedselsregistreringMyndighedNavn = BirthRegistrationAuthority,
                KontaktKanal = ContactChannel != null ? ContactChannel.ToXmlType() : null,
                NaermestePaaroerende = NextOfKinContactChannel != null ? NextOfKinContactChannel.ToXmlType() : null,
                AndreAdresser = OtherAddress != null ? OtherAddress.ToXmlType() : null,
                Virkning = Effect != null ? Effect.ToXmlType() : null
            };
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
