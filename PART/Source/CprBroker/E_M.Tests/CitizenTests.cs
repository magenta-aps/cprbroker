using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    public class CitizenTests
    {
        public decimal[] RandomCprNumbers = Utilities.RandomCprNumbers(5);

        class CitizenStub : Citizen
        {
            public CitizenStub()
            {
                PNR = Utilities.RandomCprNumber();
                Gender = 'M';
            }

            public bool _ToChurchMembershipIndicator;
            public override bool ToChurchMembershipIndicator()
            {
                return _ToChurchMembershipIndicator;
            }

            public AdresseType _ToAdresseType = new AdresseType();
            public override AdresseType ToAdresseType()
            {
                return _ToAdresseType;
            }

            public bool _ToDirectoryProtectionIndicator;
            public override bool ToDirectoryProtectionIndicator(DateTime effectDate)
            {
                return _ToDirectoryProtectionIndicator;
            }

            public bool _ToAddressProtectionIndicator;
            public override bool ToAddressProtectionIndicator(DateTime effectDate)
            {
                return _ToAddressProtectionIndicator;
            }

            public CountryIdentificationCodeType _ToCountryIdentificationCodeType = new CountryIdentificationCodeType() { Value = "5648465" };
            public override CountryIdentificationCodeType ToCountryIdentificationCodeType()
            {
                return _ToCountryIdentificationCodeType;
            }

            public AdresseType _ToAndreAdresse = new AdresseType();
            public override AdresseType ToAndreAdresse()
            {
                return _ToAndreAdresse;
            }

            public DateTime _ToBirthdate = new DateTime(2011, 11, 9);
            public override DateTime ToBirthdate()
            {
                return _ToBirthdate;
            }

            public string _ToBirthRegistrationAuthority;
            public override string ToBirthRegistrationAuthority()
            {
                return _ToBirthRegistrationAuthority;
            }

            public KontaktKanalType _ToKontaktKanalType = new KontaktKanalType();
            public override KontaktKanalType ToKontaktKanalType()
            {
                return _ToKontaktKanalType;
            }

            public bool _ToCivilRegistrationValidityStatusIndicator;
            public override bool ToCivilRegistrationValidityStatusIndicator()
            {
                return _ToCivilRegistrationValidityStatusIndicator;
            }

            public KontaktKanalType _ToNextOfKin = new KontaktKanalType();
            public override KontaktKanalType ToNextOfKin()
            {
                return _ToNextOfKin;
            }

            public NavnStrukturType _ToNavnStrukturType = new NavnStrukturType();
            public override NavnStrukturType ToNavnStrukturType()
            {
                return _ToNavnStrukturType;
            }

            public VirkningType _ToEgenskabVirkningType = new VirkningType();
            public override VirkningType ToEgenskabTypeVirkning()
            {
                return this._ToEgenskabVirkningType;
            }
        }

        [TestFixture]
        public class ToRegistreringType1
        {
            public class ToRegistreringType1Citizen : Citizen
            {
                public AttributListeType _ToAttributListeType = new AttributListeType();
                public override AttributListeType ToAttributListeType(DateTime effectDate)
                {
                    return _ToAttributListeType;
                }

                public TilstandListeType _ToTilstandListeType = new TilstandListeType();
                public override TilstandListeType ToTilstandListeType()
                {
                    return _ToTilstandListeType;
                }

                public RelationListeType _ToRelationListeType = new RelationListeType();
                public override RelationListeType ToRelationListeType(Func<string, Guid> cpr2uuidFunc)
                {
                    return _ToRelationListeType;
                }

                public TidspunktType _ToTidspunktType = new TidspunktType();
                public override TidspunktType ToTidspunktType()
                {
                    return _ToTidspunktType;
                }
            }

            [Test]
            public void ToRegistreringType1_Valid_NotNull()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.NotNull(result);
            }

            [Test]
            public void ToRegistreringType1_Valid_CorrectActorRefItem()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.AreEqual(E_MDataProvider.ActorId.ToString(), result.AktoerRef.Item);
            }

            [Test]
            public void ToRegistreringType1_Valid_CorrectActorRefItemEkementName()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.AreEqual(ItemChoiceType.UUID, result.AktoerRef.ItemElementName);
            }

            [Test]
            public void ToRegistreringType1_Valid_CorrectAttributListe()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.AreEqual(citizen._ToAttributListeType, result.AttributListe);
            }

            [Test]
            public void ToRegistreringType1_Valid_NullCommentText()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.IsNullOrEmpty(result.CommentText);
            }

            [Test]
            public void ToRegistreringType1_Valid_CorrectLivscyklusKode()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.AreEqual(LivscyklusKodeType.Rettet, result.LivscyklusKode);
            }

            [Test]
            public void ToRegistreringType1_Valid_CorrectRelationListe()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.AreEqual(citizen._ToRelationListeType, result.RelationListe);
            }

            [Test]
            public void ToRegistreringType1_Valid_CorrectTidspunkt()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.AreEqual(citizen._ToTidspunktType, result.Tidspunkt);
            }

            [Test]
            public void ToRegistreringType1_Valid_CorrectTilstandListe()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.AreEqual(citizen._ToTilstandListeType, result.TilstandListe);
            }

            [Test]
            public void ToRegistreringType1_Valid_VirkningNotNull()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.NotNull(result.Virkning);
            }

            [Test]
            public void ToRegistreringType1_Valid_VirkningNotZero()
            {
                var citizen = new ToRegistreringType1Citizen();
                var result = citizen.ToRegistreringType1(DateTime.Now, null);
                Assert.GreaterOrEqual(result.Virkning.Length, 1);
            }
        }

        [TestFixture]
        public class ToAttributListeType
        {
            class ToAttributListeTypeCitizen : Citizen
            {
                public EgenskabType _ToEgenskabType = new EgenskabType();
                public override EgenskabType ToEgenskabType()
                {
                    return _ToEgenskabType;
                }

                public LokalUdvidelseType _ToLokalUdvidelseType = new LokalUdvidelseType();
                public override LokalUdvidelseType ToLokalUdvidelseType()
                {
                    return _ToLokalUdvidelseType;
                }

                public RegisterOplysningType _ToRegisterOplysningType = new RegisterOplysningType();
                public override RegisterOplysningType ToRegisterOplysningType(DateTime effectDate)
                {
                    return _ToRegisterOplysningType;
                }

                public SundhedOplysningType _ToSundhedOplysningType = new SundhedOplysningType();
                public override SundhedOplysningType ToSundhedOplysningType()
                {
                    return _ToSundhedOplysningType;
                }
            }

            [Test]
            public void ToAttributListeType_Valid_NotNull()
            {
                var citizen = new ToAttributListeTypeCitizen();
                var result = citizen.ToAttributListeType(DateTime.Now);
                Assert.NotNull(result);
            }

            [Test]
            public void ToAttributListeType_Valid_SingleEgenskab()
            {
                var citizen = new ToAttributListeTypeCitizen();
                var result = citizen.ToAttributListeType(DateTime.Now);
                Assert.AreEqual(1, result.Egenskab.Length);
            }

            [Test]
            public void ToAttributListeType_Valid_CorrectEgenskab()
            {
                var citizen = new ToAttributListeTypeCitizen();
                var result = citizen.ToAttributListeType(DateTime.Now);
                Assert.AreEqual(citizen._ToEgenskabType, result.Egenskab[0]);
            }

            [Test]
            public void ToAttributListeType_Valid_CorrectLokalUdvidelse()
            {
                var citizen = new ToAttributListeTypeCitizen();
                var result = citizen.ToAttributListeType(DateTime.Now);
                Assert.AreEqual(citizen._ToLokalUdvidelseType, result.LokalUdvidelse);
            }

            [Test]
            public void ToAttributListeType_Valid_SingleRegisterOplysning()
            {
                var citizen = new ToAttributListeTypeCitizen();
                var result = citizen.ToAttributListeType(DateTime.Now);
                Assert.AreEqual(1, result.RegisterOplysning.Length);
            }

            [Test]
            public void ToAttributListeType_Valid_CorrectRegisterOplysning()
            {
                var citizen = new ToAttributListeTypeCitizen();
                var result = citizen.ToAttributListeType(DateTime.Now);
                Assert.AreEqual(citizen._ToRegisterOplysningType, result.RegisterOplysning[0]);
            }

            [Test]
            public void ToAttributListeType_Valid_SingleSundhedOplysning()
            {
                var citizen = new ToAttributListeTypeCitizen();
                var result = citizen.ToAttributListeType(DateTime.Now);
                Assert.AreEqual(1, result.SundhedOplysning.Length);
            }

            [Test]
            public void ToAttributListeType_Valid_CorrectSundhedOplysning()
            {
                var citizen = new ToAttributListeTypeCitizen();
                var result = citizen.ToAttributListeType(DateTime.Now);
                Assert.AreEqual(citizen._ToSundhedOplysningType, result.SundhedOplysning[0]);
            }
        }

        [TestFixture]
        public class ToEgenskabType
        {
            [Test]
            public void ToEgenskabType_Valid_NotNull()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.NotNull(result);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectAndreAdresser()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToAndreAdresse, result.AndreAdresser);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectBirthdate()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToBirthdate, result.BirthDate);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectBirthPlaceText()
            {
                var citizen = new CitizenStub() { BirthPlaceText = "jkhgkhkahkj" };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen.BirthPlaceText, result.FoedestedNavn);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectBirthRegistrationAuthority(
                [Values("GK", "LTK")]string authorityName)
            {
                var citizen = new CitizenStub() { _ToBirthRegistrationAuthority = authorityName };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(authorityName, result.FoedselsregistreringMyndighedNavn);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectKontaktKanal()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToKontaktKanalType, result.KontaktKanal);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectNaermestePaaroerende()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToNextOfKin, result.NaermestePaaroerende);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectNavnStruktur()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToNavnStrukturType, result.NavnStruktur);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectGender(
                [Values('K', 'M')]char gender)
            {
                var citizen = new CitizenStub() { Gender = gender };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(Converters.ToPersonGenderCodeType(gender), result.PersonGenderCode);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectVirkning()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToEgenskabVirkningType, result.Virkning);
            }

        }

        [TestFixture]
        public class ToNavnStrukturType
        {

            [Test]
            public void ToNavnStrukturType_Null_NotNull(
                [Values(null, "")]string addressingName)
            {
                var citizen = new Citizen() { AddressingName = addressingName };
                var result = citizen.ToNavnStrukturType();
                Assert.NotNull(result);
            }

            string[] AddressingNames = new string[] { "John Mendel-Jensen Smith", "Smith, John Mendel-Jensen", "John Mendel-Jensen         Smith" };

            [Test]
            public void ToNavnStrukturType_Valid_AddressingNameNotNull(
                [ValueSource("AddressingNames")]string addressingName)
            {
                var citizen = new Citizen() { AddressingName = addressingName };
                var result = citizen.ToNavnStrukturType();
                Assert.IsNotNullOrEmpty(result.PersonNameForAddressingName);
            }

            [Test]
            public void ToNavnStrukturType_Valid_PersonNameNotNull(
                [ValueSource("AddressingNames")]string addressingName)
            {
                var citizen = new Citizen() { AddressingName = addressingName };
                var result = citizen.ToNavnStrukturType();
                Assert.NotNull(result.PersonNameStructure);
            }

            [Test]
            public void ToNavnStrukturType_Valid_CorrectLastName(
                [ValueSource("AddressingNames")]string addressingName)
            {
                var citizen = new Citizen() { AddressingName = addressingName };
                var result = citizen.ToNavnStrukturType();
                Assert.AreEqual("John Mendel-Jensen Smith", result.PersonNameStructure.ToString());
            }
        }

        [TestFixture]
        public class ToRegisterOplysningType
        {
            [Test]
            public void ToRegisterOplysningType_WhateverCountry_ReturnsNotNull(
                [Values(5100, 6763, 12)]short countryCode)
            {
                var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode, CitizenStatusCode = 1, Road = new Road() { RoadCode = 222 } };
                var result = citizen.ToRegisterOplysningType(DateTime.Today);
                Assert.NotNull(result.Item);
            }

            [Test]
            public void ToRegisterOplysningType_WhateverCountry_VirkningNotNull(
                [Values(5100, 6763, 12)]short countryCode)
            {
                var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CitizenStatusCode = 1, CountryCode = countryCode, Road = new Road() { RoadCode = 22 } };
                var result = citizen.ToRegisterOplysningType(DateTime.Today);
                Assert.NotNull(result.Virkning);
            }

            [Test]
            public void ToRegisterOplysningType_UnknownCountry_ReturnsUkendtBorgerType(
                [ValueSource(typeof(Constants), "UnknownCountryCodes")]short countryCode)
            {
                var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode };
                var result = citizen.ToRegisterOplysningType(DateTime.Today);
                Assert.IsInstanceOf<UkendtBorgerType>(result.Item);
            }

            [Test]
            public void ToRegisterOplysningType_UnknownCountry_ReturnsCprBorgerType(
                [Values(5100)]short countryCode)
            {
                var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode, CitizenStatusCode = 1, Road = new Road() { RoadCode = 222 } };
                var result = citizen.ToRegisterOplysningType(DateTime.Today);
                Assert.IsInstanceOf<CprBorgerType>(result.Item);
            }

            [Test]
            public void ToRegisterOplysningType_UnknownCountry_ReturnsUdenlandskBorgerType(
                [Values(1, 7683, 628, 6583)]short countryCode)
            {
                var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode };
                var result = citizen.ToRegisterOplysningType(DateTime.Today);
                Assert.IsInstanceOf<UdenlandskBorgerType>(result.Item);
            }
        }

        [TestFixture]
        public class ToCprBorgerType
        {
            [Test]
            public void ToCprBorgerType_Valid_NotNull()
            {
                var citizen = new CitizenStub();
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.NotNull(result);
            }

            [Test]
            public void ToCprBorgerType_Valid_NullAddressNoteText()
            {
                var citizen = new CitizenStub();
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.IsNullOrEmpty(result.AdresseNoteTekst);
            }

            [Test]
            public void ToCprBorgerType_Valid_CorrectFolkekirkeMedlemIndikator(
                [Values(true, false)]bool isMember)
            {
                var citizen = new CitizenStub() { _ToChurchMembershipIndicator = isMember };
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.AreEqual(isMember, result.FolkekirkeMedlemIndikator);
            }

            [Test]
            public void ToCprBorgerType_Valid_CorrectAdresseType()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.AreEqual(citizen._ToAdresseType, result.FolkeregisterAdresse);
            }

            [Test]
            public void ToCprBorgerType_Valid_CorrectForskerBeskyttelseIndikator(
                [Values(true, false)] bool isProtected)
            {
                var citizen = new CitizenStub() { _ToDirectoryProtectionIndicator = isProtected };
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.AreEqual(citizen._ToDirectoryProtectionIndicator, result.ForskerBeskyttelseIndikator);
            }

            [Test]
            public void ToCprBorgerType_Valid_CorrectNavneAdresseBeskyttelseIndikator(
                [Values(true, false)] bool isProtected)
            {
                var citizen = new CitizenStub() { _ToAddressProtectionIndicator = isProtected };
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.AreEqual(citizen._ToAddressProtectionIndicator, result.NavneAdresseBeskyttelseIndikator);
            }

            [Test]
            public void ToCprBorgerType_Valid_CorrectPersonCivilRegistrationIdentifier(
                [ValueSource(typeof(CitizenTests), "RandomCprNumbers")] decimal cprNumber)
            {
                var citizen = new CitizenStub() { PNR = cprNumber };
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.AreEqual(Converters.ToCprNumber(citizen.PNR), result.PersonCivilRegistrationIdentifier);
            }

            [Test]
            public void ToCprBorgerType_Valid_CorrectPersonNationalityCode()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.AreEqual(citizen._ToCountryIdentificationCodeType, result.PersonNationalityCode);
            }

            [Test]
            public void ToCprBorgerType_Valid_CorrectPersonNummerGyldighedStatusIndikator(
                [Values(true, false)]bool pnrValidity)
            {
                var citizen = new CitizenStub() { _ToCivilRegistrationValidityStatusIndicator = pnrValidity };
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.AreEqual(citizen._ToCivilRegistrationValidityStatusIndicator, result.PersonNummerGyldighedStatusIndikator);
            }

            [Test]
            public void ToCprBorgerType_Valid_FalsePersonNummerGyldighedStatusIndikator()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToCprBorgerType(DateTime.Now);
                Assert.AreEqual(false, result.TelefonNummerBeskyttelseIndikator);
            }
        }

        [TestFixture]
        public class ToUdenlandskBorgerType
        {
            [Test]
            [ExpectedException()]
            public void ToUdenlandskBorgerType_Empty_ThrowsException()
            {
                var citizen = new Citizen();
                citizen.ToUdenlandskBorgerType();
            }

            [Test]
            [Combinatorial]
            public void ToUdenlandskBorgerType_Valid_CorrectCountryCode(
                [ValueSource(typeof(CitizenTests), "RandomCprNumbers")] decimal cprNumber,
                [Values(1500, 762, 876)] short countryCode)
            {
                var citizen = new Citizen() { PNR = cprNumber, CountryCode = countryCode };
                var result = citizen.ToUdenlandskBorgerType();
                Assert.AreEqual(countryCode.ToString(), result.PersonNationalityCode[0].Value);
            }

            [Test]
            [Combinatorial]
            public void ToUdenlandskBorgerType_Valid_CorrectPNR(
                [ValueSource(typeof(CitizenTests), "RandomCprNumbers")] decimal cprNumber,
                [Values(1500, 762, 876)] short countryCode)
            {
                var citizen = new Citizen() { PNR = cprNumber, CountryCode = countryCode };
                var result = citizen.ToUdenlandskBorgerType();
                Assert.AreEqual(Converters.ToCprNumber(cprNumber), result.PersonCivilRegistrationReplacementIdentifier);
            }

            [Test]
            [Combinatorial]
            public void ToUdenlandskBorgerType_Valid_NullFoedselslandKode(
                [ValueSource(typeof(CitizenTests), "RandomCprNumbers")] decimal cprNumber,
                [Values(1500, 762, 876)] short countryCode)
            {
                var citizen = new Citizen() { PNR = cprNumber, CountryCode = countryCode };
                var result = citizen.ToUdenlandskBorgerType();
                Assert.Null(result.FoedselslandKode);
            }

            [Test]
            [Combinatorial]
            public void ToUdenlandskBorgerType_Valid_NullPersonIdentifikator(
                [ValueSource(typeof(CitizenTests), "RandomCprNumbers")] decimal cprNumber,
                [Values(1500, 762, 876)] short countryCode)
            {
                var citizen = new Citizen() { PNR = cprNumber, CountryCode = countryCode };
                var result = citizen.ToUdenlandskBorgerType();
                Assert.IsNullOrEmpty(result.PersonIdentifikator);
            }

            [Test]
            [Combinatorial]
            public void ToUdenlandskBorgerType_Valid_NullSprogCode(
                [ValueSource(typeof(CitizenTests), "RandomCprNumbers")] decimal cprNumber,
                [Values(1500, 762, 876)] short countryCode)
            {
                var citizen = new Citizen() { PNR = cprNumber, CountryCode = countryCode };
                var result = citizen.ToUdenlandskBorgerType();
                Assert.Null(result.SprogKode);
            }
        }

        [TestFixture]
        public class ToUkendtBorgerType
        {

            [Test]
            [ExpectedException]
            public void ToUkendtBorgerType_InvalidPNR_ThrowsException(
                [Values(0, 43, -99, 23.4)] decimal cprNumber)
            {
                var citizen = new Citizen() { PNR = cprNumber };
                citizen.ToUkendtBorgerType();
            }

            [Test]
            public void ToUkendtBorgerType_Valid_CorrectPNR(
                [ValueSource(typeof(CitizenTests), "RandomCprNumbers")] decimal cprNumber)
            {
                var citizen = new Citizen() { PNR = cprNumber };
                var result = citizen.ToUkendtBorgerType();
                Assert.AreEqual(Converters.ToCprNumber(cprNumber), result.PersonCivilRegistrationReplacementIdentifier);
            }
        }

        [TestFixture]
        public class ToTidspunktType
        {
            [Test]
            public void ToTidspunktType_EmptyFields_ReturnsEmpty()
            {
                var citizen = new Citizen();
                var result = citizen.ToTidspunktType();
                Assert.Null(result.ToDateTime());
            }

            [Test]
            public void ToTidspunktType_IrrelevantFields_ReturnsEmpty()
            {
                var citizen = new Citizen() { Birthdate = DateTime.Today, BirthdateUncertainty = ' ', NationalityChangeTimestamp = DateTime.Today, NationalityChangeTimestampUncertainty = ' ', PNRCreationDate = DateTime.Today, MunicipalityArrivalDateUncertainty = ' ' };
                var result = citizen.ToTidspunktType();
                Assert.Null(result.ToDateTime());
            }

            [Test]
            [Combinatorial]
            public void ToTidspunktType_RelevantFields_ReturnsValue(
                [Values(0, -1, -5)]int birthRegOffset,
                [Values(0, -1, -5)]int cprChurchOffset,
                [Values(0, -1, -5)]int cprPersonOffset,
                [Values(0, -1, -5)]int pnrMarkingOffset,
                [Values(0, -1, -5)]int municipalityArrivalOffset)
            {
                var today = DateTime.Today;
                var citizen = new Citizen()
                {
                    BirthRegistrationDate = today.AddYears(birthRegOffset),
                    BirthRegistrationDateUncertainty = ' ',
                    CprChurchTimestamp = today.AddYears(cprChurchOffset),
                    CprPersonTimestamp = today.AddYears(cprPersonOffset),
                    PNRMarkingDate = today.AddYears(pnrMarkingOffset),
                    PNRMarkingDateUncertainty = ' ',
                    MunicipalityArrivalDate = today.AddYears(municipalityArrivalOffset),
                    MunicipalityArrivalDateUncertainty = ' '
                };
                var result = citizen.ToTidspunktType();
                var maxOffset = Math.Max(birthRegOffset, Math.Max(cprChurchOffset, Math.Max(cprPersonOffset, Math.Max(pnrMarkingOffset, municipalityArrivalOffset))));
                if (today.AddYears(maxOffset) != result.ToDateTime().Value)
                {
                    System.Diagnostics.Debugger.Break();
                }
                Assert.AreEqual(today.AddYears(maxOffset), result.ToDateTime().Value);
            }
        }
    }
}
