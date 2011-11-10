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
                AttributListe = this.ToAttributListeType(effectDate),
                CommentText = null,
                LivscyklusKode = LivscyklusKodeType.Rettet,
                RelationListe = this.ToRelationListeType(cpr2uuidFunc),
                Tidspunkt = this.ToTidspunktType(),
                TilstandListe = this.ToTilstandListeType(),
                Virkning = null
            };
            ret.CalculateVirkning();
            return ret;
        }

        public virtual AttributListeType ToAttributListeType(DateTime effectDate)
        {
            return new AttributListeType()
            {
                Egenskab = new EgenskabType[] { this.ToEgenskabType() },
                LokalUdvidelse = this.ToLokalUdvidelseType(),
                RegisterOplysning = new RegisterOplysningType[] { this.ToRegisterOplysningType(effectDate) },
                SundhedOplysning = new SundhedOplysningType[] { this.ToSundhedOplysningType() }
            };
        }

        public virtual EgenskabType ToEgenskabType()
        {
            var ret = new EgenskabType()
            {
                AndreAdresser = this.ToAndreAdresse(),
                BirthDate = this.ToBirthdate(),
                FoedestedNavn = this.BirthPlaceText,
                FoedselsregistreringMyndighedNavn = ToBirthRegistrationAuthority(),
                KontaktKanal = this.ToKontaktKanalType(),
                NaermestePaaroerende = this.ToNextOfKin(),
                NavnStruktur = this.ToNavnStrukturType(),
                PersonGenderCode = Converters.ToPersonGenderCodeType(this.Gender),
                Virkning = this.ToEgenskabVirkningType()
            };
            return ret;
        }

        public virtual NavnStrukturType ToNavnStrukturType()
        {
            var ret = NavnStrukturType.Create(this.AddressingName);
            ret.PersonNameForAddressingName = this.AddressingName;
            return ret;
        }

        public virtual RegisterOplysningType ToRegisterOplysningType(DateTime effectDate)
        {
            var ret = new RegisterOplysningType()
            {
                Item = null,
                // TODO: Fill effect dates
                Virkning = null,
            };
            if (Constants.UnknownCountryCodes.Contains(this.CountryCode))
            {
                ret.Item = this.ToUkendtBorgerType();
            }
            else if (this.CountryCode == Constants.DenmarkCountryCode)
            {
                ret.Item = this.ToCprBorgerType(effectDate);
            }
            else
            {
                ret.Item = this.ToUdenlandskBorgerType();
            }
            return ret;
        }

        public CprBorgerType ToCprBorgerType(DateTime effectDate)
        {
            return new CprBorgerType()
            {
                AdresseNoteTekst = null,
                FolkekirkeMedlemIndikator = this.ToChurchMembershipIndicator(),
                FolkeregisterAdresse = this.ToAdresseType(),
                ForskerBeskyttelseIndikator = this.ToDirectoryProtectionIndicator(effectDate),
                NavneAdresseBeskyttelseIndikator = this.ToAddressProtectionIndicator(effectDate),
                PersonCivilRegistrationIdentifier = Converters.ToCprNumber(this.PNR),
                PersonNationalityCode = this.ToCountryIdentificationCodeType(),
                PersonNummerGyldighedStatusIndikator = this.ToCivilRegistrationValidityStatusIndicator(),
                // Telphone numbers are not supported
                TelefonNummerBeskyttelseIndikator = false
            };
        }

        public UdenlandskBorgerType ToUdenlandskBorgerType()
        {
            return new UdenlandskBorgerType()
            {
                // TODO: See if we can find a birth nationality (different from current nationality)
                FoedselslandKode = null,
                // TODO: Shall PersonCivilRegistrationReplacementIdentifier and PersonIdentifikator be swapped ?
                PersonCivilRegistrationReplacementIdentifier = Converters.ToCprNumber(this.PNR),
                PersonIdentifikator = null,
                PersonNationalityCode = new CountryIdentificationCodeType[] { this.ToCountryIdentificationCodeType() },
                SprogKode = null
            };
        }

        public UkendtBorgerType ToUkendtBorgerType()
        {
            return new UkendtBorgerType()
            {
                PersonCivilRegistrationReplacementIdentifier = Converters.ToCprNumber(this.PNR)
            };
        }

    }
}
