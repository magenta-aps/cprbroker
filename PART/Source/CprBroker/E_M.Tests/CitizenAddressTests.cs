﻿using System;
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
                        PostDistrict = "Gentofte",
                        RoadName = "Studiestraede"
                    }
                };

            ValidAddressTestValues = new Citizen[] { citizen };
            AllAddressTestValues = new Citizen[] { citizen };

        }

        Citizen[] ValidAddressTestValues = null;
        Citizen[] AllAddressTestValues = null;

        #region ToAdresseType
        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_All_NotNull(Citizen citizen)
        {
            var result = citizen.ToAdresseType();
            Assert.NotNull(result);
        }

        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_All_ItemNotNull(Citizen citizen)
        {
            var result = citizen.ToAdresseType();
            Assert.NotNull(result.Item);
        }

        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_Alll_ItemIsDanskAdresseType(Citizen citizen)
        {
            var result = citizen.ToAdresseType();
            Assert.IsInstanceOf<CprBroker.Schemas.Part.DanskAdresseType>(result.Item);
        }
        #endregion

        #region ToDanskAdresseType

        [Test]
        [Ignore]
        public void ToDanskAdresseType_Null_UkendtAdresseIndikatorTrue()
        {
            var citizen = new Citizen();
            var result = citizen.ToDanskAdresseType();
            Assert.True(result.UkendtAdresseIndikator);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_AddressCompleteNotNull(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNotNull(result.AddressComplete);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_AddressPointNull(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNull(result.AddressPoint);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_NoteTekstNull(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.NoteTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_PolitiDistriktTekstNull(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.PolitiDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_PostDistriktTekstNotNull(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNotNullOrEmpty(result.PostDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SkoleDistriktTekstNull(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.SkoleDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SocialDistriktTekstNull(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.SocialDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SogneDistriktTekstNull(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.SogneDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SpecielVejkodeIndikatorFalse(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.False(result.SpecielVejkodeIndikator);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SpecielVejkodeIndikatorSpecifiedFalse(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.False(result.SpecielVejkodeIndikatorSpecified);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_UkendtAdresseIndikatorFalse(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.False(result.UkendtAdresseIndikator);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_ValgkredsDistriktTekstNull(Citizen citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.ValgkredsDistriktTekst);
        }
        #endregion

        #region ToSpecielVejkodeIndikator
        [Test]
        public void ToSpecielVejkodeIndikator_LowCode_ReturnsFalse(
            [Values(1, 4, 5, 34, 500, 700, 899)]short roadCode)
        {
            var citizen = new Citizen() { RoadCode = roadCode };
            var result = citizen.ToSpecielVejkodeIndikator();
            Assert.False(result);
        }

        [Test]
        public void ToSpecielVejkodeIndikator_HighCode_ReturnsTrue(
            [Values(9900, 9950, 9999)]short roadCode)
        {
            var citizen = new Citizen() { RoadCode = roadCode };
            var result = citizen.ToSpecielVejkodeIndikator();
            Assert.True(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToSpecielVejkodeIndikator_InvalidCode_ThrowsException(
            [Values(-20,-1, 0, 10000,10022)]short roadCode)
        {
            var citizen = new Citizen() { RoadCode = roadCode };
            citizen.ToSpecielVejkodeIndikator();
        }
        #endregion

        #region ToAddressCompleteType

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressCompleteType_Valid_AddressAccessNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressCompleteType();
            Assert.IsNotNull(result.AddressAccess);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressCompleteType_Valid_AddressPostalNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressCompleteType();
            Assert.IsNotNull(result.AddressPostal);
        }
        #endregion

        #region ToAddressAccessType

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_MunicipalityCodeNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressAccessType();
            Assert.IsNotNullOrEmpty(result.MunicipalityCode);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_StreetBuildingIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressAccessType();
            Assert.IsNotNullOrEmpty(result.StreetBuildingIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_StreetCodeNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressAccessType();
            Assert.IsNotNullOrEmpty(result.StreetCode);
        }
        #endregion

        #region ToAddressPostalType

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_NotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_CountryIdentificationCodeNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNull(result.CountryIdentificationCode);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_DistrictNameNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.DistrictName);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_DistrictSubdivisionIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.DistrictSubdivisionIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_FloorIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.FloorIdentifier);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_MailDeliverySublocationIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.MailDeliverySublocationIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_PostCodeIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.PostCodeIdentifier);
        }
        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_PostOfficeBoxIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.PostOfficeBoxIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetBuildingIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.StreetBuildingIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetNameNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.StreetName);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetNameForAddressingNameNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.StreetNameForAddressingName);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_SuiteIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.SuiteIdentifier);
        }
        #endregion

    }
}
