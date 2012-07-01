using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public AttributListeType ToAttributListeType()
        {
            return new AttributListeType()
            {
                Egenskab = ToEgenskabType(),
                LokalUdvidelse = ToLokalUdvidelseType(),
                RegisterOplysning = ToRegisterOplysningType(),
                SundhedOplysning = ToSundhedOplysningType()
            };
        }

        public EgenskabType[] ToEgenskabType()
        {
            return new EgenskabType[] {
                new EgenskabType()
                {
                    AndreAdresser = ToAndreAdresser(),
                    BirthDate = ToBirthDate(),
                    FoedestedNavn = ToFoedestedNavn(),
                    FoedselsregistreringMyndighedNavn = ToFoedselsregistreringMyndighedNavn(),
                    KontaktKanal = ToKontaktKanalType(),
                    NaermestePaaroerende = ToNaermestePaaroerende(),
                    NavnStruktur = ToNavnStrukturType(),
                    PersonGenderCode = PersonGenderCodeType(),
                    Virkning = ToEgenskabVirkning()
                }
            };
        }

        private AdresseType ToAndreAdresser()
        {
            throw new NotImplementedException();
        }

        public DateTime ToBirthDate()
        {
            var val = this.PersonInformation.ToBirthdate();
            if (val.HasValue)
                return val.Value;
            else
                return Utilities.Strings.PersonNumberToDate(this.PersonInformation.ToPnr()).Value;
        }

        public string ToFoedestedNavn()
        {
            // Birth name not implemented
            // TODO: See if can be found
            return null;
        }

        public string ToFoedselsregistreringMyndighedNavn()
        {
            return this.BirthRegistrationInformation.AdditionalBirthRegistrationText;
        }

        public NavnStrukturType ToNavnStrukturType()
        {
            // TODO: See how to use corresponding 3 name flags
            return NavnStrukturType.Create(this.CurrentNameInformation.FirstName_s, this.CurrentNameInformation.MiddleName, this.CurrentNameInformation.LastName);
        }

        private PersonGenderCodeType PersonGenderCodeType()
        {
            throw new NotImplementedException();
        }

        private VirkningType ToEgenskabVirkning()
        {
            throw new NotImplementedException();
        }

        public RegisterOplysningType[] ToRegisterOplysningType()
        {
            throw new NotImplementedException();
        }


    }
}
