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
        public EgenskabType ToXmlType()
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

        public static PersonProperties FromXmlType(EgenskabType oio)
        {
            return new PersonProperties()
            {
                BirthDate = oio.BirthDate,

                GenderId = DAL.Part.Gender.GetPartCode(oio.PersonGenderCode),
                BirthPlace = oio.FoedestedNavn,
                BirthRegistrationAuthority = oio.FoedselsregistreringMyndighedNavn,

                FirstName = oio.NavnStruktur.PersonNameStructure.PersonGivenName,
                MiddleName = oio.NavnStruktur.PersonNameStructure.PersonMiddleName,
                LastName = oio.NavnStruktur.PersonNameStructure.PersonSurnameName,

                NickName = oio.NavnStruktur.KaldenavnTekst,
                NameNoteText = oio.NavnStruktur.NoteTekst,
                AddressingName = oio.NavnStruktur.PersonNameForAddressingName,

                ContactChannel = ContactChannel.FromXmlType(oio.KontaktKanal),
                NextOfKinContactChannel = ContactChannel.FromXmlType(oio.NaermestePaaroerende),
                OtherAddress = Address.FromXmlType(oio.AndreAdresser),

                Effect = Effect.FromXmlType(oio.Virkning),
            };
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonProperties>(pp => pp.ContactChannel);
            loadOptions.LoadWith<PersonProperties>(pp => pp.OtherAddress);
            loadOptions.LoadWith<PersonProperties>(pp => pp.Effect);
        }
    }
}
