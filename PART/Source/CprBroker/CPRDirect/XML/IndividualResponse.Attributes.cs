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
            return new RegisterOplysningType[]{
                new RegisterOplysningType()
                {
                    Item = ToCprBorgerType(),
                    Virkning = ToCprBorgerTypeVirkning()
                }
            };
        }

        public CprBorgerType ToCprBorgerType()
        {
            return new CprBorgerType()
            {
                AdresseNoteTekst = ToAdresseNoteTekst(),
                FolkekirkeMedlemIndikator = ToFolkekirkeMedlemIndikator(),
                FolkeregisterAdresse = ToFolkeregisterAdresse(),
                ForskerBeskyttelseIndikator = ToForskerBeskyttelseIndikator(),
                NavneAdresseBeskyttelseIndikator = ToNavneAdresseBeskyttelseIndikator(),
                PersonCivilRegistrationIdentifier = ToPersonCivilRegistrationIdentifier(),
                PersonNationalityCode = ToPersonNationalityCode(),
                PersonNummerGyldighedStatusIndikator = ToPersonNummerGyldighedStatusIndikator(),
                TelefonNummerBeskyttelseIndikator = ToTelefonNummerBeskyttelseIndikator(),
            };
        }

        private bool ToTelefonNummerBeskyttelseIndikator()
        {
            throw new NotImplementedException();
        }

        private bool ToPersonNummerGyldighedStatusIndikator()
        {
            throw new NotImplementedException();
        }

        private CountryIdentificationCodeType ToPersonNationalityCode()
        {
            throw new NotImplementedException();
        }

        private string ToPersonCivilRegistrationIdentifier()
        {
            throw new NotImplementedException();
        }

        private bool ToNavneAdresseBeskyttelseIndikator()
        {
            throw new NotImplementedException();
        }

        private bool ToForskerBeskyttelseIndikator()
        {
            throw new NotImplementedException();
        }

        private AdresseType ToFolkeregisterAdresse()
        {
            throw new NotImplementedException();
        }

        private bool ToFolkekirkeMedlemIndikator()
        {
            return this.ChurchInformation.ToFolkekirkeMedlemIndikator();
        }

        public string ToAdresseNoteTekst()
        {
            return null;
        }

        public VirkningType ToCprBorgerTypeVirkning()
        {
            var dates = new DateTime?[] { 
                this.ChurchInformation.ToChurchRelationshipDate()
            };
            throw new NotImplementedException();
        }


    }
}
