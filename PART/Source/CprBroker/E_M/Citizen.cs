using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        public RegistreringType1 ToRegistreringType1(DateTime effectDate, Func<string, Guid> cpr2uuidFunc)
        {
            var ret = new RegistreringType1()
            {
                AktoerRef = UnikIdType.Create(E_MDataProvider.ActorId),
                AttributListe = ToAttributListeType(this, effectDate),
                CommentText = null,
                LivscyklusKode = LivscyklusKodeType.Rettet,
                RelationListe = ToRelationListeType(this, cpr2uuidFunc),
                Tidspunkt = ToTidspunktType(this),
                TilstandListe = ToTilstandListeType(this),
                Virkning = null
            };
            ret.CalculateVirkning();
            return ret;
        }

        public AttributListeType ToAttributListeType(Citizen citizen, DateTime effectDate)
        {
            return new AttributListeType()
            {
                Egenskab = new EgenskabType[] { ToEgenskabType(citizen) },
                LokalUdvidelse = ToLokalUdvidelseType(citizen),
                RegisterOplysning = new RegisterOplysningType[] { this.ToRegisterOplysningType(effectDate) },
                SundhedOplysning = new SundhedOplysningType[] { this.ToSundhedOplysningType() }
            };
        }

        private static EgenskabType ToEgenskabType(Citizen citizen)
        {
            var ret = new EgenskabType()
            {
                AndreAdresser = ToAndreAdresse(citizen),
                BirthDate = ToBirthdate(citizen),
                FoedestedNavn = citizen.BirthPlaceText,
                FoedselsregistreringMyndighedNavn = Authority.ToAuthorityName(citizen.BirthRegistrationAuthority),
                KontaktKanal = ToKontaktKanalType(citizen),
                NaermestePaaroerende = ToNextOfKin(citizen),
                NavnStruktur = ToNavnStrukturType(citizen),
                PersonGenderCode = Converters.ToPersonGenderCodeType(citizen.Gender),
                Virkning = ToVirkningType(citizen)
            };
            return ret;
        }

        private static NavnStrukturType ToNavnStrukturType(Citizen citizen)
        {
            //TODO: Fill person name
            return null;
        }

        public RegisterOplysningType ToRegisterOplysningType(DateTime effectDate)
        {
            var ret = new RegisterOplysningType()
            {
                Item = null,
                // TODO: Fill effect dates
                Virkning = null,
            };
            if (Constants.UnknownCountryCodes.Contains(this.CountryCode))
            {
                ret.Item = ToUkendtBorgerType(this);
            }
            else if (this.CountryCode == Constants.DenmarkCountryCode)
            {
                ret.Item = ToCprBorgerType(this, effectDate);
            }
            else
            {
                ret.Item = ToUdenlandskBorgerType(this);
            }
            return ret;
        }

        public static CprBorgerType ToCprBorgerType(Citizen citizen, DateTime effectDate)
        {
            return new CprBorgerType()
            {
                AdresseNoteTekst = null,
                FolkekirkeMedlemIndikator = citizen.ToChurchMembershipIndicator(),
                FolkeregisterAdresse = citizen.ToAdresseType(),
                ForskerBeskyttelseIndikator = citizen.ToDirectoryProtectionIndicator(effectDate),
                NavneAdresseBeskyttelseIndikator = citizen.ToAddressProtectionIndicator(effectDate),
                PersonCivilRegistrationIdentifier = Converters.ToCprNumber(citizen.PNR),
                PersonNationalityCode = citizen.ToCountryIdentificationCodeType(),
                // Person number is valid as long as it is in the database
                PersonNummerGyldighedStatusIndikator = true,
                // Telphone numbers are not supported
                TelefonNummerBeskyttelseIndikator = false
            };
        }

        public static UdenlandskBorgerType ToUdenlandskBorgerType(Citizen citizen)
        {
            if (citizen != null)
            {
                return new UdenlandskBorgerType()
                {
                    // TODO: See if we can find a birth nationality (different from current nationality)
                    FoedselslandKode = null,
                    // TODO: Shall PersonCivilRegistrationReplacementIdentifier and PersonIdentifikator be swapped ?
                    PersonCivilRegistrationReplacementIdentifier = Converters.ToCprNumber(citizen.PNR),
                    PersonIdentifikator = null,
                    PersonNationalityCode = new CountryIdentificationCodeType[] { citizen.ToCountryIdentificationCodeType() },
                    SprogKode = null
                };
            }
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

        public static UkendtBorgerType ToUkendtBorgerType(Citizen citizen)
        {
            if (citizen != null)
            {
                return new UkendtBorgerType()
                {
                    PersonCivilRegistrationReplacementIdentifier = Converters.ToCprNumber(citizen.PNR)
                };
            }
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

    }
}
