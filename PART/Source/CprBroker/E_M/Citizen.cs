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
        public RegistreringType1 ToRegistreringType1(Func<string, Guid> cpr2uuidFunc)
        {
            var ret = new RegistreringType1()
            {
                AktoerRef = UnikIdType.Create(E_MDataProvider.ActorId),
                AttributListe = ToAttributListeType(),
                CommentText = null,
                LivscyklusKode = LivscyklusKodeType.Rettet,
                RelationListe = ToRelationListeType(cpr2uuidFunc),
                Tidspunkt = ToTidspunktType(),
                TilstandListe = ToTilstandListeType(),
                Virkning = null
            };
            ret.CalculateVirkning();
            return ret;
        }

        private AttributListeType ToAttributListeType()
        {
            return new AttributListeType()
            {
                Egenskab = new EgenskabType[] { ToEgenskabType() },
                LokalUdvidelse = ToLokalUdvidelseType(),
                RegisterOplysning = new RegisterOplysningType[] { ToRegisterOplysningType() },
                SundhedOplysning = new SundhedOplysningType[] { ToSundhedOplysningType() }
            };
        }

        private EgenskabType ToEgenskabType()
        {
            return new EgenskabType()
            {
                AndreAdresser = ToAndreAdresse(),
                BirthDate = this.Birthdate.Value,
                FoedestedNavn = null,
                FoedselsregistreringMyndighedNavn = this.BirthRegistrationText,
                KontaktKanal = ToKontaktKanalType(),
                NaermestePaaroerende = ToNextOfKin(),
                NavnStruktur = ToNavnStrukturType(),
                PersonGenderCode = Converters.ToPersonGenderCodeType(Gender),
                Virkning = ToVirkningType()
            };
        }

        private NavnStrukturType ToNavnStrukturType()
        {
            //TODO: Fill person name
            return null;
        }

        private RegisterOplysningType ToRegisterOplysningType()
        {
            var ret = new RegisterOplysningType()
            {
                Item = null,
                // TODO: Fill effect dates
                Virkning = null,
            };
            if (!CountryCode.HasValue
                || new short[] { Constants.AbroadCountryCode, Constants.ReservedCountryCode, Constants.StatelessCountryCode, Constants.UnknownCountryCode }.Contains(CountryCode.Value))
            {
                ret.Item = ToUkendtBorgerType();
            }
            else if (CountryCode.Value == Constants.DenmarkCountryCode)
            {
                ret.Item = ToCprBorgerType();
            }
            else
            {
                ret.Item = ToUdenlandskBorgerType();
            }
            return ret;
        }

        private UkendtBorgerType ToUkendtBorgerType()
        {
            return new UkendtBorgerType()
            {
                PersonCivilRegistrationReplacementIdentifier = PNR.ToString()
            };
        }

        private CprBorgerType ToCprBorgerType()
        {
            return new CprBorgerType()
            {
                AdresseNoteTekst = null,
                FolkekirkeMedlemIndikator = false,
                FolkeregisterAdresse = this.CitizenPotReadyAddresses.First().ToAdresseType(),
                ForskerBeskyttelseIndikator = false,
                NavneAdresseBeskyttelseIndikator = false,
                PersonCivilRegistrationIdentifier = null,
                PersonNationalityCode = null,
                PersonNummerGyldighedStatusIndikator = false,
                TelefonNummerBeskyttelseIndikator = false
            };
        }


        private UdenlandskBorgerType ToUdenlandskBorgerType()
        {
            //TODO: Revise foreign citizen data
            return new UdenlandskBorgerType()
            {
                FoedselslandKode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, CountryCode.Value.ToString()),
                PersonCivilRegistrationReplacementIdentifier = Converters.ToCprNumber(PNR),
                PersonIdentifikator = null,
                PersonNationalityCode = new CountryIdentificationCodeType[] { CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, CountryCode.Value.ToString()) },
                SprogKode = null
            };
        }

    }
}
