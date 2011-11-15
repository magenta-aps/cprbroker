using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    class CitizenAddressTests
    {
        private Citizen CreateCitizen()
        {
            return new Citizen()
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
                },
                Road = new Road()
                {
                    MunicipalityCode = 561,
                    RoadCode = 112,
                    RoadName = "Studistraede",
                    RoadAddressingName = "Studistraede"
                }
            };
        }
        public CitizenAddressTests()
        {
            var citizen = CreateCitizen();

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
        [Combinatorial]
        public void ToDanskAdresseType_ValidNormalCode_SpecielVejkodeIndikatorFalse(
            [ValueSource("ValidAddressTestValues")]Citizen citizen,
            [Values(1, 2, 77, 950)]short roadCode)
        {
            var val = citizen.RoadCode;
            citizen.RoadCode = roadCode;
            try
            {
                var result = citizen.ToDanskAdresseType();
                Assert.False(result.SpecielVejkodeIndikator);
            }
            finally
            {
                citizen.RoadCode = val;
            }
        }

        [Test]
        [Combinatorial]
        public void ToDanskAdresseType_ValidNormalCode_SpecielVejkodeIndikatorTrue(
            [ValueSource("ValidAddressTestValues")]Citizen citizen,
            [Values(9900, 9950)]short roadCode)
        {
            var val = citizen.RoadCode;
            citizen.RoadCode = roadCode;
            try
            {
                var result = citizen.ToDanskAdresseType();
                Assert.True(result.SpecielVejkodeIndikator);
            }
            finally
            {
                citizen.RoadCode = val;
            }
        }

        [Test]
        public void ToDanskAdresseType_Valid_SpecielVejkodeIndikatorSpecifiedTrue(
            [ValueSource("ValidAddressTestValues")] Citizen citizen,
            [Values(1, 2, 77, 950)]short roadCode
            )
        {
            var val = citizen.RoadCode;
            citizen.RoadCode = roadCode;
            try
            {
                var result = citizen.ToDanskAdresseType();
                Assert.True(result.SpecielVejkodeIndikatorSpecified);
            }
            finally
            {
                citizen.RoadCode = val;
            }
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
            [Values(-20, -1, 0, 10000, 10022)]short roadCode)
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
            var result = citizen.ToAddressCompleteType(null, null);
            Assert.IsNotNull(result.AddressAccess);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressCompleteType_Valid_AddressPostalNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressCompleteType(null, null);
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
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Road", MatchType = MessageMatch.Contains)]
        public void ToAddressPostalType_NullRoad_ThrowsException()
        {
            var citizen = new Citizen() { Road = null };
            citizen.ToAddressPostalType(null, null);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_NotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType(null, null);
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_CountryIdentificationCodeNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType(null, null);
            Assert.IsNotNull(result.CountryIdentificationCode);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_DistrictNameNotNull(Citizen citizen)
        {
            var postDistrict = Utilities.RandomString();
            var result = citizen.ToAddressPostalType(null, postDistrict);
            Assert.AreEqual(postDistrict, result.DistrictName);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_DistrictSubdivisionIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType(null, null);
            Assert.IsNotNullOrEmpty(result.DistrictSubdivisionIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_FloorIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType(null, null);
            Assert.IsNotNullOrEmpty(result.FloorIdentifier);
        }

        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_MailDeliverySublocationIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType(null, null);
            Assert.IsNotNullOrEmpty(result.MailDeliverySublocationIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_CorrectPostCodeIdentifier(Citizen citizen)
        {
            var postCode = Utilities.RandomString();
            var result = citizen.ToAddressPostalType(postCode, null);
            Assert.AreEqual(postCode, result.PostCodeIdentifier);
        }
        [Test]
        [Ignore]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_PostOfficeBoxIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType(null, null);
            Assert.IsNotNullOrEmpty(result.PostOfficeBoxIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetBuildingIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType(null, null);
            Assert.IsNotNullOrEmpty(result.StreetBuildingIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Valid_CorrectStreetName(
            [ValueSource(typeof(Utilities), "RandomStrings5")]string roadName)
        {
            var citizen = CreateCitizen();
            citizen.Road.RoadName = roadName;
            var result = citizen.ToAddressPostalType(null, null);
            Assert.AreEqual(roadName, result.StreetName);
        }

        [Test]
        public void ToAddressPostalType_Valid_CorrectStreetNameForAddressingName(
            [ValueSource(typeof(Utilities), "RandomStrings5")]string roadAddressingName)
        {
            var citizen = CreateCitizen();
            citizen.Road.RoadAddressingName = roadAddressingName;
            var result = citizen.ToAddressPostalType(null, null);
            Assert.AreEqual(roadAddressingName, result.StreetNameForAddressingName);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_SuiteIdentifierNotNull(Citizen citizen)
        {
            var result = citizen.ToAddressPostalType(null, null);
            Assert.IsNotNullOrEmpty(result.SuiteIdentifier);
        }
        #endregion

        #region GetPostCodeAndDistrict()

        public short[] RandomPostCodes
        {
            get
            {
                var ret = new short[5];
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = (short)Utilities.RandomShort();
                }
                return ret;
            }
        }

        public string[] RandomPostDistricts
        {
            get
            {
                var ret = new string[5];
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = Utilities.RandomString();
                }
                return ret;
            }
        }

        string foundPostCode;
        string foundPostDistrict;

        [Test]
        public void GetPostCodeAndDistrict_Empty_ReturnsEmpty()
        {
            var citizen = new Citizen();
            citizen.GetPostCodeAndDistrict(out foundPostCode, out foundPostDistrict);
            Assert.IsNullOrEmpty(foundPostCode);
            Assert.IsNullOrEmpty(foundPostDistrict);
        }

        [Test]
        [Sequential]
        public void GetPostCodeAndDistrict_DirectHousePostCode_ReturnsCorrect(
            [ValueSource("RandomPostCodes")] short postCode,
            [ValueSource("RandomPostDistricts")] string postDistrict)
        {
            var citizen = new Citizen()
            {
                HousePostCode = new HousePostCode() { PostCode = postCode, PostDistrict = postDistrict }
            };
            citizen.GetPostCodeAndDistrict(out foundPostCode, out foundPostDistrict);
            Assert.AreEqual(postCode.ToString(), foundPostCode);
            Assert.AreEqual(postDistrict, foundPostDistrict);
        }

        [Test]
        [Sequential]
        public void GetPostCodeAndDistrict_PostCodeByRoad_ReturnsCorrect(
            [ValueSource("RandomPostCodes")] short postCode,
            [ValueSource("RandomPostDistricts")] string postDistrict)
        {
            var citizen = new Citizen()
            {
                HousePostCode = null
            };
            citizen.RoadPostCodes.Add(new HousePostCode() { PostCode = postCode, PostDistrict = postDistrict });
            citizen.GetPostCodeAndDistrict(out foundPostCode, out foundPostDistrict);
            Assert.AreEqual(postCode.ToString(), foundPostCode);
            Assert.AreEqual(postDistrict, foundPostDistrict);
        }

        [Test]
        [Sequential]
        public void GetPostCodeAndDistrict_DoublePostCodeByRoad_ReturnsEmpty(
            [ValueSource("RandomPostCodes")] short postCode,
            [ValueSource("RandomPostDistricts")] string postDistrict)
        {
            var citizen = new Citizen()
            {
                HousePostCode = null
            };
            citizen.RoadPostCodes.Add(new HousePostCode() { PostCode = postCode, PostDistrict = postDistrict + "SSS" });
            citizen.RoadPostCodes.Add(new HousePostCode() { PostCode = postCode, PostDistrict = postDistrict });
            citizen.GetPostCodeAndDistrict(out foundPostCode, out foundPostDistrict);
            Assert.IsNullOrEmpty(foundPostCode);
            Assert.IsNullOrEmpty(foundPostDistrict);
        }

        [Test]
        [Sequential]
        public void GetPostCodeAndDistrict_PostCodeByEqualHouseRange_ReturnsCorrect(
            [ValueSource("RandomPostCodes")] short houseNumber,
            [ValueSource("RandomPostCodes")] short postCode,
            [ValueSource("RandomPostDistricts")] string postDistrict)
        {
            var citizen = new Citizen()
            {
                HousePostCode = null,
                HouseNumber = houseNumber.ToString(),
            };
            citizen.RoadPostCodes.Add(new HousePostCode() { PostCode = Utilities.RandomShort(), PostDistrict = Utilities.RandomString() });
            citizen.RoadPostCodes.Add(new HousePostCode() { PostCode = Utilities.RandomShort(), PostDistrict = Utilities.RandomString() });

            citizen.HouseRangePostCodes.Add(new HouseRangePostCode() { PostCode = postCode, FromHouseNumber = houseNumber.ToString(), ToHouseNumber = houseNumber.ToString() });

            citizen.GetPostCodeAndDistrict(out foundPostCode, out foundPostDistrict);
            Assert.AreEqual(postCode.ToString(), foundPostCode);
            Assert.IsNullOrEmpty(foundPostDistrict);
        }

        [Test]
        [Sequential]
        public void GetPostCodeAndDistrict_PostCodeByUnequalHouseRange_ReturnsCorrect(
            [ValueSource("RandomPostCodes")] short houseNumber,
            [ValueSource("RandomPostCodes")] short postCode,
            [ValueSource("RandomPostDistricts")] string postDistrict)
        {
            var citizen = new Citizen()
            {
                HousePostCode = null,
                HouseNumber = houseNumber.ToString(),
            };
            citizen.RoadPostCodes.Add(new HousePostCode() { PostCode = Utilities.RandomShort(), PostDistrict = Utilities.RandomString() });
            citizen.RoadPostCodes.Add(new HousePostCode() { PostCode = Utilities.RandomShort(), PostDistrict = Utilities.RandomString() });

            citizen.HouseRangePostCodes.Add(new HouseRangePostCode() { PostCode = postCode, FromHouseNumber = (houseNumber - 1).ToString(), ToHouseNumber = (houseNumber + 1).ToString() });

            citizen.GetPostCodeAndDistrict(out foundPostCode, out foundPostDistrict);
            Assert.AreEqual(postCode.ToString(), foundPostCode);
            Assert.IsNullOrEmpty(foundPostDistrict);
        }

        [Test]
        [Sequential]
        public void GetPostCodeAndDistrict_DoublePostCodeByRoadAndOutOfRangeHouseNumber_ReturnsEmpty(
            [ValueSource("RandomPostCodes")] short houseNumber,
            [ValueSource("RandomPostCodes")] short postCode,
            [ValueSource("RandomPostDistricts")] string postDistrict)
        {
            var citizen = new Citizen()
            {
                HousePostCode = null,
                HouseNumber = houseNumber.ToString(),
            };
            citizen.RoadPostCodes.Add(new HousePostCode() { PostCode = postCode, PostDistrict = postDistrict + "EEE" });
            citizen.RoadPostCodes.Add(new HousePostCode() { PostCode = postCode, PostDistrict = postDistrict });

            citizen.HouseRangePostCodes.Add(new HouseRangePostCode() { PostCode = postCode, FromHouseNumber = (houseNumber - 100).ToString(), ToHouseNumber = (houseNumber - 50).ToString() });
            citizen.HouseRangePostCodes.Add(new HouseRangePostCode() { PostCode = postCode, FromHouseNumber = (houseNumber + 50).ToString(), ToHouseNumber = (houseNumber + 100).ToString() });

            citizen.GetPostCodeAndDistrict(out foundPostCode, out foundPostDistrict);
            Assert.IsNullOrEmpty(foundPostCode);
            Assert.IsNullOrEmpty(foundPostDistrict);
        }

        #endregion

    }
}
