using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonProperties
    {
        public static EgenskabType[] ToXmlType(PersonProperties db)
        {
            if (db != null)
            {
                return new EgenskabType[]
                {
                    new EgenskabType()
                    {
                        BirthDate = db.BirthDate,
                        PersonGenderCode = DAL.Part.Gender.GetPartGender(db.GenderId),
                        NavnStruktur = new NavnStrukturType()
                        {
                            KaldenavnTekst = db.NickName,
                            NoteTekst = db.NameNoteText,
                            PersonNameForAddressingName = db.AddressingName,
                            PersonNameStructure = PersonName.ToXmlType(db.PersonName),
                        },
                        FoedestedNavn = db.BirthPlace,
                        FoedselsregistreringMyndighedNavn = db.BirthRegistrationAuthority,
                        KontaktKanal = ContactChannel.ToXmlType(db.ContactChannel),
                        NaermestePaaroerende = ContactChannel.ToXmlType(db.NextOfKinContactChannel),
                        AndreAdresser = Address.ToXmlType(db.OtherAddress),
                        Virkning = Effect.ToVirkningType(db.Effect)
                    }
                };
            }
            return null;
        }

        public static PersonProperties FromXmlType(EgenskabType[] oio)
        {
            if (oio != null && oio.Length > 0 && oio[0] != null)
            {
                return new PersonProperties()
                {
                    BirthDate = oio[0].BirthDate,

                    GenderId = DAL.Part.Gender.GetPartCode(oio[0].PersonGenderCode),
                    BirthPlace = oio[0].FoedestedNavn,
                    BirthRegistrationAuthority = oio[0].FoedselsregistreringMyndighedNavn,

                    PersonName = PersonName.FromXmlType(oio[0].NavnStruktur.PersonNameStructure),

                    NickName = oio[0].NavnStruktur.KaldenavnTekst,
                    NameNoteText = oio[0].NavnStruktur.NoteTekst,
                    AddressingName = oio[0].NavnStruktur.PersonNameForAddressingName,

                    ContactChannel = ContactChannel.FromXmlType(oio[0].KontaktKanal),
                    NextOfKinContactChannel = ContactChannel.FromXmlType(oio[0].NaermestePaaroerende),
                    OtherAddress = Address.FromXmlType(oio[0].AndreAdresser),

                    Effect = Effect.FromVirkningType(oio[0].Virkning),
                };
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonProperties>(pp => pp.ContactChannel);
            loadOptions.LoadWith<PersonProperties>(pp => pp.NextOfKinContactChannel);
            loadOptions.LoadWith<PersonProperties>(pp => pp.OtherAddress);
            loadOptions.LoadWith<PersonProperties>(pp => pp.Effect);
            loadOptions.LoadWith<PersonProperties>(pp => pp.Gender);
            loadOptions.LoadWith<PersonProperties>(pp => pp.PersonName);

            ContactChannel.SetChildLoadOptions(loadOptions);
            Address.SetChildLoadOptions(loadOptions);
        }
    }
}
