using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public AttributListeType ToAttributListeType(DateTime effectDate)
        {
            return new AttributListeType()
            {
                Egenskab = ToEgenskabType(),
                LokalUdvidelse = ToLokalUdvidelseType(),
                RegisterOplysning = ToRegisterOplysningType(effectDate),
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

        public RegisterOplysningType[] ToRegisterOplysningType(DateTime effectDate)
        {
            return new RegisterOplysningType[]{
                new RegisterOplysningType()
                {
                    Item = ToCprBorgerType(effectDate),
                    Virkning = ToCprBorgerTypeVirkning(effectDate)
                }
            };
        }

        public CprBorgerType ToCprBorgerType(DateTime effectDate)
        {
            return new CprBorgerType()
            {
                AdresseNoteTekst = ToAdresseNoteTekst(),
                FolkekirkeMedlemIndikator = ToFolkekirkeMedlemIndikator(),
                FolkeregisterAdresse = ToFolkeregisterAdresse(),
                ForskerBeskyttelseIndikator = ToForskerBeskyttelseIndikator(effectDate),
                NavneAdresseBeskyttelseIndikator = ToNavneAdresseBeskyttelseIndikator(effectDate),
                PersonCivilRegistrationIdentifier = ToPersonCivilRegistrationIdentifier(),
                PersonNationalityCode = ToPersonNationalityCode(),
                PersonNummerGyldighedStatusIndikator = ToPersonNummerGyldighedStatusIndikator(),
                TelefonNummerBeskyttelseIndikator = ToTelefonNummerBeskyttelseIndikator(),
            };
        }

        public bool ToPersonNummerGyldighedStatusIndikator()
        {
            return this.PersonInformation.ToPersonNummerGyldighedStatusIndikator();
        }

        public CountryIdentificationCodeType ToPersonNationalityCode()
        {
            return this.CurrentCitizenship.ToPersonNationalityCode();
        }

        private string ToPersonCivilRegistrationIdentifier()
        {
            throw new NotImplementedException();
        }

        public bool ToNavneAdresseBeskyttelseIndikator(DateTime effectDate)
        {
            return ProtectionType.HasProtection(this.Protection, effectDate, ProtectionType.ProtectionCategoryCodes.NameAndAddress);
        }

        public bool ToForskerBeskyttelseIndikator(DateTime effectDate)
        {
            return ProtectionType.HasProtection(this.Protection, effectDate, ProtectionType.ProtectionCategoryCodes.Research);
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

        public VirkningType ToCprBorgerTypeVirkning(DateTime effectDate)
        {
            var dates = new DateTime?[] { 
                this.PersonInformation.ToStatusDate(),
                this.ChurchInformation.ToChurchRelationshipDate(),
                this.CurrentCitizenship.ToCitizenshipStartDate(),
            };

            var effects = new List<VirkningType>();
            effects.AddRange(ProtectionType.ToVirkningTypeArray(this.Protection, effectDate, ProtectionType.ProtectionCategoryCodes.NameAndAddress, ProtectionType.ProtectionCategoryCodes.Research));

            throw new NotImplementedException();
        }


    }
}
