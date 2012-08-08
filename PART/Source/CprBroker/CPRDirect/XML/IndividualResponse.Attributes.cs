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
            return this.PersonInformation.ToBirthdate(true).Value;
        }

        public string ToFoedestedNavn()
        {
            var oldestName = HistoricalNameType.GetOldestName(this.HistoricalName) as INameSource;
            if (oldestName == null)
            {
                oldestName = this.CurrentNameInformation;
            }
            var nameStartDate = Converters.ToDateTime(oldestName.NameStartDate, oldestName.NameStartDateUncertainty);
            var birthDate = this.ToBirthDate();

            if (nameStartDate.HasValue
                && (nameStartDate.Value - birthDate).TotalDays <= 14)
            {
                return oldestName.ToNavnStrukturType().PersonNameStructure.ToString();
            }
            else
            {
                return null;
            }
        }

        public string ToFoedselsregistreringMyndighedNavn()
        {
            return this.BirthRegistrationInformation.ToFoedselsregistreringMyndighedNavn();
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
            var effects = new List<VirkningType>();

            // TODO: Fill other address date
            // other address
            // Not implemented

            // birthdate
            // birthname
            // birth authority
            // gender
            effects.Add(VirkningType.Create(this.PersonInformation.ToBirthdate(), null));

            // contact channel
            // nearest relative
            // Not implemented

            // name
            effects.Add(VirkningType.Create(this.CurrentNameInformation.ToNameStartDate(), null));

            return VirkningType.Compose(effects.ToArray());
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

        public string ToPersonCivilRegistrationIdentifier()
        {
            return this.PersonInformation.ToPnr();
        }

        public bool ToNavneAdresseBeskyttelseIndikator(DateTime effectDate)
        {
            return ProtectionType.HasProtection(this.Protection, effectDate, ProtectionType.ProtectionCategoryCodes.NameAndAddress);
        }

        public bool ToForskerBeskyttelseIndikator(DateTime effectDate)
        {
            return ProtectionType.HasProtection(this.Protection, effectDate, ProtectionType.ProtectionCategoryCodes.Research);
        }

        public IAddressSource GetFolkeregisterAdresseSource()
        {
            if (this.CurrentAddressInformation != null && !this.ClearWrittenAddress.IsEmpty) // Both conditions are technically the same
            {
                return new CurrentAddressWrapper(this.CurrentAddressInformation, this.ClearWrittenAddress);
            }
            else if (this.CurrentDepartureData != null && !this.CurrentDepartureData.IsEmpty)
            {
                return CurrentDepartureData;
            }
            else
            {
                return new DummyAddressSource();
            }
        }

        public AdresseType ToFolkeregisterAdresse()
        {
            return this.GetFolkeregisterAdresseSource().ToAdresseType();
        }

        public VirkningType[] ToFolkeregisterAdresseVirknning()
        {
            return this.GetFolkeregisterAdresseSource().ToVirkningTypeArray();
        }

        public string ToAdresseNoteTekst()
        {
            return this.GetFolkeregisterAdresseSource().ToAddressNoteTekste();
        }

        private bool ToFolkekirkeMedlemIndikator()
        {
            return this.ChurchInformation.ToFolkekirkeMedlemIndikator();
        }

        public VirkningType ToCprBorgerTypeVirkning(DateTime effectDate)
        {
            var effects = new List<VirkningType>();

            var dates = new List<DateTime?>(
                new DateTime?[] { 
                    this.PersonInformation.ToStatusDate(),
                    this.ChurchInformation.ToChurchRelationshipDate(),
                    this.CurrentCitizenship.ToCitizenshipStartDate(),
            });

            effects.AddRange(dates.Select(d => VirkningType.Create(d, null)));

            effects.AddRange(
                ProtectionType.ToVirkningTypeArray(this.Protection, effectDate, ProtectionType.ProtectionCategoryCodes.NameAndAddress, ProtectionType.ProtectionCategoryCodes.Research)
                );

            effects.Add(this.PersonInformation.ToVirkningType());

            effects.AddRange(this.ToFolkeregisterAdresseVirknning());

            return VirkningType.Compose(effects.ToArray());
        }
    }

}
