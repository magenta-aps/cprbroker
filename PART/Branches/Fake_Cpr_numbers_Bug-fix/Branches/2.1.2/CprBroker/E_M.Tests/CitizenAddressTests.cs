/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
        private CitizenReadyAddress CreateCitizen()
        {
            var ret = new CitizenReadyAddress()
            {
                AddressingName = "Beemen Beshara",
                CareOfName = "Beemen Beshara",
                CityName = "CPH City",
                PostCode = 1234,
                PostDistrict = "CPH District",
                Door = "1",
                Floor = "7",
                PNR = 120420070111m,
                HouseNumber = "sada",
                RoadName = "Studistraede Studistraede Studistraede Studistraede",
                RoadCode = 112
            };
            ret.Roads.Add(new Road()
                {
                    MunicipalityCode = 561,
                    RoadCode = ret.RoadCode,
                    RoadName = ret.RoadName,
                    RoadAddressingName = ret.RoadName.Substring(0,20),                    
                });
            return ret;
        }
        public CitizenAddressTests()
        {
            var citizen = CreateCitizen();

            ValidAddressTestValues = new CitizenReadyAddress[] { citizen };
            AllAddressTestValues = new CitizenReadyAddress[] { citizen };

        }

        CitizenReadyAddress[] ValidAddressTestValues = null;
        CitizenReadyAddress[] AllAddressTestValues = null;

        #region ToAdresseType
        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_All_NotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAdresseType();
            Assert.NotNull(result);
        }

        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_All_ItemNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAdresseType();
            Assert.NotNull(result.Item);
        }

        [Test]
        [TestCaseSource("AllAddressTestValues")]
        public void ToAdresseType_Alll_ItemIsDanskAdresseType(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAdresseType();
            Assert.IsInstanceOf<CprBroker.Schemas.Part.DanskAdresseType>(result.Item);
        }
        #endregion

        #region ToDanskAdresseType

        [Test]
        public void ToDanskAdresseType_Empty_UkendtAdresseIndikatorFalse()
        {
            var citizen = new CitizenReadyAddress() { RoadCode = 22 };
            citizen.Roads.Add(new Road() { RoadCode = 22 });
            var result = citizen.ToDanskAdresseType();
            Assert.False(result.UkendtAdresseIndikator);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_AddressCompleteNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNotNull(result.AddressComplete);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_AddressPointNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNull(result.AddressPoint);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_NoteTekstNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.NoteTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_PolitiDistriktTekstNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.PolitiDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_PostDistriktTekstNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNotNullOrEmpty(result.PostDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SkoleDistriktTekstNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.SkoleDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SocialDistriktTekstNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.SocialDistriktTekst);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_SogneDistriktTekstNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.IsNullOrEmpty(result.SogneDistriktTekst);
        }

        [Test]
        [Combinatorial]
        public void ToDanskAdresseType_ValidNormalCode_SpecielVejkodeIndikatorFalse(
            [ValueSource("ValidAddressTestValues")]CitizenReadyAddress citizen,
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
            [ValueSource("ValidAddressTestValues")]CitizenReadyAddress citizen,
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
            [ValueSource("ValidAddressTestValues")] CitizenReadyAddress citizen,
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
        public void ToDanskAdresseType_Valid_UkendtAdresseIndikatorFalse(CitizenReadyAddress citizen)
        {
            var result = citizen.ToDanskAdresseType();
            Assert.False(result.UkendtAdresseIndikator);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToDanskAdresseType_Valid_ValgkredsDistriktTekstNull(CitizenReadyAddress citizen)
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
            var citizen = new CitizenReadyAddress() { RoadCode = roadCode };
            var result = citizen.ToSpecielVejkodeIndikator();
            Assert.False(result);
        }

        [Test]
        public void ToSpecielVejkodeIndikator_HighCode_ReturnsTrue(
            [Values(9900, 9950, 9999)]short roadCode)
        {
            var citizen = new CitizenReadyAddress() { RoadCode = roadCode };
            var result = citizen.ToSpecielVejkodeIndikator();
            Assert.True(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToSpecielVejkodeIndikator_InvalidCode_ThrowsException(
            [Values(-20, -1, 0, 10000, 10022)]short roadCode)
        {
            var citizen = new CitizenReadyAddress() { RoadCode = roadCode };
            citizen.ToSpecielVejkodeIndikator();
        }
        #endregion

        #region ToAddressCompleteType

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressCompleteType_Valid_AddressAccessNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressCompleteType();
            Assert.IsNotNull(result.AddressAccess);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressCompleteType_Valid_AddressPostalNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressCompleteType();
            Assert.IsNotNull(result.AddressPostal);
        }
        #endregion

        #region ToAddressAccessType

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_MunicipalityCodeNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressAccessType();
            Assert.IsNotNullOrEmpty(result.MunicipalityCode);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_StreetBuildingIdentifierNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressAccessType();
            Assert.IsNotNullOrEmpty(result.StreetBuildingIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressAccessType_Valid_StreetCodeNotNull(CitizenReadyAddress citizen)
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
            var citizen = new CitizenReadyAddress() { };
            citizen.ToAddressPostalType();
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_NotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_CountryIdentificationCodeNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNull(result.CountryIdentificationCode);
        }

        [Test]
        public void ToAddressPostalType_Valid_CorrectDistrictName([ValueSource(typeof(Utilities), "RandomStrings5")]string postDistrict)
        {
            var citizen = CreateCitizen();
            citizen.PostDistrict = postDistrict;             
            var result = citizen.ToAddressPostalType();
            Assert.AreEqual(postDistrict, result.DistrictName);
        }

        [Test]        
        public void ToAddressPostalType_Valid_CorrectDistrictSubdivisionIdentifierFromCity([ValueSource(typeof(Utilities), "RandomStrings5")]string cityName)
        {
            var citizen = CreateCitizen();
            citizen.CityName = cityName;
            var result = citizen.ToAddressPostalType();
            Assert.AreEqual(cityName, result.DistrictSubdivisionIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_FloorIdentifierNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.FloorIdentifier);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_MailDeliverySublocationIdentifierEmpty(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNullOrEmpty(result.MailDeliverySublocationIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_CorrectPostCodeIdentifier(CitizenReadyAddress citizen)
        {
            var postCode = Utilities.RandomShort();
            citizen.PostCode = postCode;
            var result = citizen.ToAddressPostalType();
            Assert.AreEqual(postCode.ToString(), result.PostCodeIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_PostOfficeBoxIdentifierEmpty(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNullOrEmpty(result.PostOfficeBoxIdentifier);
        }
        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_StreetBuildingIdentifierNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.StreetBuildingIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Valid_CorrectStreetName(
            [ValueSource(typeof(Utilities), "RandomStrings5")]string roadName)
        {
            var citizen = CreateCitizen();
            citizen.RoadName = roadName;
            citizen.Roads[0].RoadName = roadName;
            var result = citizen.ToAddressPostalType();
            Assert.AreEqual(roadName, result.StreetName);
        }

        [Test]
        public void ToAddressPostalType_Valid_CorrectStreetNameForAddressingName(
            [ValueSource(typeof(Utilities), "RandomStrings5")]string roadAddressingName)
        {
            var citizen = CreateCitizen();
            citizen.Roads[0].RoadAddressingName = roadAddressingName;
            var result = citizen.ToAddressPostalType();
            Assert.AreEqual(roadAddressingName, result.StreetNameForAddressingName);
        }

        [Test]
        [TestCaseSource("ValidAddressTestValues")]
        public void ToAddressPostalType_Valid_SuiteIdentifierNotNull(CitizenReadyAddress citizen)
        {
            var result = citizen.ToAddressPostalType();
            Assert.IsNotNullOrEmpty(result.SuiteIdentifier);
        }
        #endregion

        #region GetActiveRoad

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetActiveRoad_NoRoads_ThrowsException()
        {
            var citizen = new CitizenReadyAddress();
            citizen.GetActiveRoad();
        }

        [Test]
        public void GetActiveRoad_OneRoad_ReturnsCorrect()
        {
            var citizen = new CitizenReadyAddress();
            var road = new Road();
            citizen.Roads.Add(road);
            var result = citizen.GetActiveRoad();
            Assert.AreEqual(road, result);
        }

        [Test]
        [Combinatorial]
        public void GetActiveRoad_MultipleRoad_ReturnsCorrect(
            [Values(2, 3, 5, 20, 78)]int count)
        {
            int maxOffset = -10000;
            var today = DateTime.Today;

            var citizen = new CitizenReadyAddress();
            for (int i = 0; i < count; i++)
            {
                var yearOffset = Utilities.Random.Next(-1000, 1000);
                var endDate = today.AddYears(yearOffset);
                var road = new Road() { RoadEndDate = endDate };
                citizen.Roads.Add(road);
                maxOffset = Math.Max(yearOffset, maxOffset);
            }
            var result = citizen.GetActiveRoad();
            Assert.AreEqual(today.AddYears(maxOffset), result.RoadEndDate);
        }

        #endregion

    }
}
