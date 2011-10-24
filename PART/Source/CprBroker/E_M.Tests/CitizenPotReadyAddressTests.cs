using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    class CitizenPotReadyAddressTests
    {

        public CitizenPotReadyAddressTests()
        {
            var address = new CitizenPotReadyAddress()
                {
                    AddressingName = "Beemen Beshara",
                    CareOfName = "Beemen Beshara",
                    CityName = "Copenhagen",
                    Door = "1",
                    Floor = "7",
                    HouseNumber = "61",
                    MunicipalityCode = 561,
                    PNR = 120420070111m,
                    PostCode = 123,
                    PostDistrict ="Gentofte",
                    RoadCode = 112,
                    RoadName ="Studiestraede"
            };

            ValidAddressTestValues = new CitizenPotReadyAddress[] { address };
            AllAddressTestValues = new CitizenPotReadyAddress[] { null, address };

        }

        CitizenPotReadyAddress[] ValidAddressTestValues = null;
        CitizenPotReadyAddress[] AllAddressTestValues = null;

        #region ToAdresseType
        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_All_NotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAdresseType(address);
            Assert.NotNull(result);
        }

        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_All_ItemNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAdresseType(address);
            Assert.NotNull(result.Item);
        }

        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_Alll_ItemIsDanskAdresseType(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAdresseType(address);
            Assert.IsInstanceOf<CprBroker.Schemas.Part.DanskAdresseType> (result.Item);
        }
        #endregion

        #region ToDanskAdresseType
        [Test]
        public void ToDanskAdresseType_Null_NotNull()
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(null); 
            Assert.IsNotNull(result);
        }

        [Test]
        public void ToDanskAdresseType_Null_UkendtAdresseIndikatorTrue()
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(null);
            Assert.True(result.UkendtAdresseIndikator);
        }

        [Test]
        public void ToDanskAdresseType_Null_AddressCompleteNull()
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(null);
            Assert.Null(result.AddressComplete);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_AddressCompleteNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.IsNotNull(result.AddressComplete);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_AddressPointNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.IsNull(result.AddressPoint);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_NoteTekstNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.IsNullOrEmpty(result.NoteTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_PolitiDistriktTekstNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.IsNullOrEmpty(result.PolitiDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_PostDistriktTekstNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.IsNotNullOrEmpty(result.PostDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SkoleDistriktTekstNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.IsNullOrEmpty(result.SkoleDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SocialDistriktTekstNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.IsNullOrEmpty(result.SocialDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SogneDistriktTekstNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.IsNullOrEmpty(result.SogneDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SpecielVejkodeIndikatorFalse(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.False(result.SpecielVejkodeIndikator);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SpecielVejkodeIndikatorSpecifiedFalse(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.False(result.SpecielVejkodeIndikatorSpecified);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_UkendtAdresseIndikatorFalse(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.False(result.UkendtAdresseIndikator);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_ValgkredsDistriktTekstNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            Assert.IsNullOrEmpty(result.ValgkredsDistriktTekst);
        }
        #endregion

        #region ToAddressCompleteType
        [Test]
        public void ToAddressCompleteType_Null_Null()
        {
            var result = CitizenPotReadyAddress.ToAddressCompleteType(null);
            Assert.IsNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressCompleteType_Valid_AddressAccessNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressCompleteType(address);
            Assert.IsNotNull(result.AddressAccess);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressCompleteType_Valid_AddressPostalNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressCompleteType(address);
            Assert.IsNotNull(result.AddressPostal);
        }
        #endregion

        #region ToAddressAccessType
        [Test]
        public void ToAddressAccessType_Null_Null()
        {
            var result = CitizenPotReadyAddress.ToAddressAccessType(null);
            Assert.IsNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_MunicipalityCodeNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressAccessType(address);
            Assert.IsNotNullOrEmpty(result.MunicipalityCode);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_StreetBuildingIdentifierNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressAccessType(address);
            Assert.IsNotNullOrEmpty(result.StreetBuildingIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_StreetCodeNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressAccessType(address);
            Assert.IsNotNullOrEmpty(result.StreetCode);
        }
        #endregion

        #region ToAddressPostalType
        [Test]
        public void ToAddressPostalType_Null_Null()
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(null);
            Assert.IsNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_NotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_CountryIdentificationCodeNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNull(result.CountryIdentificationCode);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_DistrictNameNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.DistrictName);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_DistrictSubdivisionIdentifierNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.DistrictSubdivisionIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_FloorIdentifierNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.FloorIdentifier);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_MailDeliverySublocationIdentifierNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.MailDeliverySublocationIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_PostCodeIdentifierNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.PostCodeIdentifier);
        }
        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_PostOfficeBoxIdentifierNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.PostOfficeBoxIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetBuildingIdentifierNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.StreetBuildingIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetNameNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.StreetName);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetNameForAddressingNameNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.StreetNameForAddressingName);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_SuiteIdentifierNotNull(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            Assert.IsNotNullOrEmpty(result.SuiteIdentifier);
        }
        #endregion

    }
}
