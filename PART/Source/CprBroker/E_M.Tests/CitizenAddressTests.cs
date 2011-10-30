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
    class CitizenAddressTests
    {

        public CitizenAddressTests()
        {
            var citizen = new Citizen()
                {
                    AddressingName = "Beemen Beshara",
                    CareOfName = "Beemen Beshara",
                    //CityName = "Copenhagen",
                    Door = "1",
                    Floor = "7",
                    PNR = 120420070111m,                          
                    HousePostCode = new HousePostCode() 
                    {
                        MunicipalityCode = 561,
                        RoadCode = 112,
                        HouseNumber = "61",
                        PostCode = 123,
                        PostDistrict ="Gentofte",
                        RoadName ="Studiestraede"
                    }
                };

            ValidAddressTestValues = new Citizen[] { citizen };
            AllAddressTestValues = new Citizen[] { null, citizen };

        }

        Citizen[] ValidAddressTestValues = null;
        Citizen[] AllAddressTestValues = null;

        #region ToAdresseType
        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_All_NotNull(Citizen citizen)
        {
            var result = Citizen.ToAdresseType(citizen);
            Assert.NotNull(result);
        }

        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_All_ItemNotNull(Citizen citizen)
        {
            var result = Citizen.ToAdresseType(citizen);
            Assert.NotNull(result.Item);
        }

        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_Alll_ItemIsDanskAdresseType(Citizen citizen)
        {
            var result = Citizen.ToAdresseType(citizen);
            Assert.IsInstanceOf<CprBroker.Schemas.Part.DanskAdresseType> (result.Item);
        }
        #endregion

        #region ToDanskAdresseType
        [Test]
        public void ToDanskAdresseType_Null_NotNull()
        {
            var result = Citizen.ToDanskAdresseType(null); 
            Assert.IsNotNull(result);
        }

        [Test]
        public void ToDanskAdresseType_Null_UkendtAdresseIndikatorTrue()
        {
            var result = Citizen.ToDanskAdresseType(null);
            Assert.True(result.UkendtAdresseIndikator);
        }

        [Test]
        public void ToDanskAdresseType_Null_AddressCompleteNull()
        {
            var result = Citizen.ToDanskAdresseType(null);
            Assert.Null(result.AddressComplete);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_AddressCompleteNotNull(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.IsNotNull(result.AddressComplete);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_AddressPointNull(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.IsNull(result.AddressPoint);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_NoteTekstNull(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.IsNullOrEmpty(result.NoteTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_PolitiDistriktTekstNull(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.IsNullOrEmpty(result.PolitiDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_PostDistriktTekstNotNull(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.IsNotNullOrEmpty(result.PostDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SkoleDistriktTekstNull(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.IsNullOrEmpty(result.SkoleDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SocialDistriktTekstNull(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.IsNullOrEmpty(result.SocialDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SogneDistriktTekstNull(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.IsNullOrEmpty(result.SogneDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SpecielVejkodeIndikatorFalse(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.False(result.SpecielVejkodeIndikator);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SpecielVejkodeIndikatorSpecifiedFalse(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.False(result.SpecielVejkodeIndikatorSpecified);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_UkendtAdresseIndikatorFalse(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.False(result.UkendtAdresseIndikator);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_ValgkredsDistriktTekstNull(Citizen citizen)
        {
            var result = Citizen.ToDanskAdresseType(citizen);
            Assert.IsNullOrEmpty(result.ValgkredsDistriktTekst);
        }
        #endregion

        #region ToAddressCompleteType
        [Test]
        public void ToAddressCompleteType_Null_Null()
        {
            var result = Citizen.ToAddressCompleteType(null);
            Assert.IsNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressCompleteType_Valid_AddressAccessNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressCompleteType(citizen);
            Assert.IsNotNull(result.AddressAccess);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressCompleteType_Valid_AddressPostalNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressCompleteType(citizen);
            Assert.IsNotNull(result.AddressPostal);
        }
        #endregion

        #region ToAddressAccessType
        [Test]
        public void ToAddressAccessType_Null_Null()
        {
            var result = Citizen.ToAddressAccessType(null);
            Assert.IsNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_MunicipalityCodeNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressAccessType(citizen);
            Assert.IsNotNullOrEmpty(result.MunicipalityCode);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_StreetBuildingIdentifierNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressAccessType(citizen);
            Assert.IsNotNullOrEmpty(result.StreetBuildingIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_StreetCodeNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressAccessType(citizen);
            Assert.IsNotNullOrEmpty(result.StreetCode);
        }
        #endregion

        #region ToAddressPostalType
        [Test]
        public void ToAddressPostalType_Null_Null()
        {
            var result = Citizen.ToAddressPostalType(null);
            Assert.IsNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_NotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_CountryIdentificationCodeNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNull(result.CountryIdentificationCode);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_DistrictNameNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.DistrictName);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_DistrictSubdivisionIdentifierNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.DistrictSubdivisionIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_FloorIdentifierNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.FloorIdentifier);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_MailDeliverySublocationIdentifierNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.MailDeliverySublocationIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_PostCodeIdentifierNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.PostCodeIdentifier);
        }
        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_PostOfficeBoxIdentifierNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.PostOfficeBoxIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetBuildingIdentifierNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.StreetBuildingIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetNameNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.StreetName);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetNameForAddressingNameNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.StreetNameForAddressingName);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_SuiteIdentifierNotNull(Citizen citizen)
        {
            var result = Citizen.ToAddressPostalType(citizen);
            Assert.IsNotNullOrEmpty(result.SuiteIdentifier);
        }
        #endregion

    }
}
