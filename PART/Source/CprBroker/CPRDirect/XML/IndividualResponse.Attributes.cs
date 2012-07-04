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
                    PersonGenderCode = ToPersonGenderCodeType(),
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

        public string ToFoedselsregistreringMyndighedNavn()
        {
            // TODO: Map this.BirthRegistrationInformation.Code to authority name
            return null;
        }

        public NavnStrukturType ToNavnStrukturType()
        {
            return this.CurrentNameInformation.ToNavnStrukturType();
        }

        public PersonGenderCodeType ToPersonGenderCodeType()
        {
            return this.PersonInformation.ToPersonGenderCodeType();
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
