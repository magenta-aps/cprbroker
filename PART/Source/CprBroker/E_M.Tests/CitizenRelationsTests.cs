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
    partial class CitizenRelationsTests
    {
        private Dictionary<string, Guid> uuids = new Dictionary<string, Guid>();
        [TearDown]
        public void ClearUUIDList()
        {
            uuids.Clear();
        }
        Guid ToUuid(string cprNumber)
        {
            var ret = Guid.NewGuid();
            uuids[cprNumber] = ret;
            return ret;
        }

        char DateCertaintyTrue = ' ';
        public decimal[] RandomCprNumbers
        {
            get { return Utilities.RandomCprNumbers(10); }
        }
        public decimal[] RandomCprNumbers1
        {
            get { return Utilities.RandomCprNumbers(1); }
        }
        public decimal[] InvalidCprNumbers = new decimal[] { 10m, 6567576m };
        public decimal[] InvalidCprNumbersWithNull = new decimal[] { 0, 10m, 6567576m };

        private void AssertCprNumbers(decimal[] cprNumbers, PersonFlerRelationType[] result)
        {
            Assert.AreEqual(cprNumbers.Length, result.Length);
            for (int i = 0; i < cprNumbers.Length; i++)
            {
                var stringCprNumber = Converters.ToCprNumber(cprNumbers[i]);
                Assert.IsNotNull(result[i]);
                Assert.IsNotNull(result[i].ReferenceID);
                Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
                Assert.AreEqual(uuids[stringCprNumber].ToString(), result[i].ReferenceID.Item);
            }
        }

        private void AssertCprNumbers(decimal[] cprNumbers, PersonRelationType[] result)
        {
            Assert.AreEqual(cprNumbers.Length, result.Length);
            for (int i = 0; i < cprNumbers.Length; i++)
            {
                var stringCprNumber = Converters.ToCprNumber(cprNumbers[i]);
                Assert.IsNotNull(result[i]);
                Assert.IsNotNull(result[i].ReferenceID);
                Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
                Assert.AreEqual(uuids[stringCprNumber].ToString(), result[i].ReferenceID.Item);
            }
        }

        #region ToChildren

        [Test]
        public void ToChildren_MaleWithNullChildrenAsFather_ThrowsException()
        {
            var citizen = new Citizen() { Gender = 'M', ChildrenAsFather = null };
            var result = citizen.ToChildren(ToUuid);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ToChildren_FemaleWithNullChildrenAsMother_ReturnsEmpty()
        {
            var citizen = new Citizen() { Gender = 'K', ChildrenAsMother = null };
            var result = citizen.ToChildren(ToUuid);
            Assert.IsEmpty(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToChildren_NullConverter_ThrowsException()
        {
            var citizen = new Citizen() { };
            citizen.ToChildren(null);
        }

        private Child[] CreateChildren(int count)
        {
            var childCprNumbers = Utilities.RandomCprNumbers(count);
            var children = childCprNumbers.Select(pnr => new Child() { PNR = pnr });
            return children.ToArray();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToChildren_InvalidGender_ThrowsException(
            [Random(0, 100, 5)] int count,
            [Values(' ', 'L', 'w')] char gender)
        {
            var citizen = new Citizen() { Gender = gender };
            citizen.ChildrenAsFather.AddRange(CreateChildren(count));
            citizen.ChildrenAsMother.AddRange(CreateChildren(count));
            citizen.ToChildren(ToUuid);
        }

        [Test]
        public void ToChildren_MaleWithMotherChildren_ReturnsEmptyArray(
            [Random(0, 100, 5)] int count)
        {
            var citizen = new Citizen() { Gender = 'M' };
            citizen.ChildrenAsMother.AddRange(CreateChildren(count));
            var result = citizen.ToChildren(ToUuid);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ToChildren_CorrectMale_CorrectLength(
            [Random(0, 100, 5)] int count)
        {
            var citizen = new Citizen() { Gender = 'M' };
            citizen.ChildrenAsFather.AddRange(CreateChildren(count));
            var result = citizen.ToChildren(ToUuid);
            Assert.AreEqual(citizen.ChildrenAsFather.Count, result.Length);
        }

        [Test]
        public void ToChildren_CorrectMale_Correct(
            [Random(0, 100, 5)] int count)
        {
            count = 2;
            var citizen = new Citizen() { Gender = 'M' };
            citizen.ChildrenAsFather.AddRange(CreateChildren(count));
            var result = citizen.ToChildren(ToUuid);

            for (int i = 0; i < citizen.ChildrenAsFather.Count; i++)
            {
                var stringCprNumber = Converters.ToCprNumber(citizen.ChildrenAsFather[i].PNR);
                Assert.AreEqual(uuids[stringCprNumber].ToString(), result[i].ReferenceID.Item);
            }
        }

        [Test]
        public void ToChildren_FemaleWithFatherChildren_ReturnsEmptyArray(
            [Random(0, 100, 5)] int count)
        {
            var citizen = new Citizen() { Gender = 'K' };
            citizen.ChildrenAsFather.AddRange(CreateChildren(count));
            var result = citizen.ToChildren(ToUuid);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ToChildren_CorrectFemale_CorrectLength(
            [Random(0, 100, 5)] int count)
        {
            var citizen = new Citizen() { Gender = 'K' };
            citizen.ChildrenAsMother.AddRange(CreateChildren(count));
            var result = citizen.ToChildren(ToUuid);
            Assert.AreEqual(citizen.ChildrenAsMother.Count, result.Length);
        }

        [Test]
        public void ToChildren_CorrectFemale_Correct(
            [Random(0, 100, 5)] int count)
        {
            var citizen = new Citizen() { Gender = 'K' };
            citizen.ChildrenAsMother.AddRange(CreateChildren(count));
            var result = citizen.ToChildren(ToUuid);

            for (int i = 0; i < citizen.ChildrenAsMother.Count; i++)
            {
                var stringCprNumber = Converters.ToCprNumber(citizen.ChildrenAsMother[i].PNR);
                Assert.AreEqual(uuids[stringCprNumber].ToString(), result[i].ReferenceID.Item);
            }
        }

        #endregion

        DateTime[] TestDates = new DateTime[] { new DateTime(2010, 5, 5), new DateTime(2011, 7, 7) };
        DateTime[] TestDatesWithNull = new DateTime[] { new DateTime(), new DateTime(2010, 5, 5), new DateTime(2011, 7, 7) };

        #region ToMaritalStatusDate

        [Test]
        public void ToMaritalStatusDate_NullDate_ReturnsNull()
        {
            var citizen = new Citizen();
            var result = citizen.ToMaritalStatusDate();
            Assert.False(result.HasValue);
        }

        [Test]
        [TestCaseSource("TestDates")]
        public void ToMaritalStatusDate_InvalidUncertainty_ReturnsNull(DateTime maritalStatusDate)
        {
            var citizen = new Citizen() { MaritalStatusTimestamp = maritalStatusDate, MaritalStatusTimestampUncertainty = 'w' };
            var result = citizen.ToMaritalStatusDate();
            Assert.False(result.HasValue);
        }

        [Test]
        [TestCaseSource("TestDates")]
        public void ToMaritalStatusDate_Valid_ReturnsValue(DateTime maritalStatusDate)
        {
            var citizen = new Citizen() { MaritalStatusTimestamp = maritalStatusDate, MaritalStatusTimestampUncertainty = DateCertaintyTrue };
            var result = citizen.ToMaritalStatusDate();
            Assert.True(result.HasValue);
        }
        #endregion

        #region ToMaritalStatusEndDate

        [Test]
        public void ToMaritalStatusTerminationDate_NullDate_ReturnsNull()
        {
            var citizen = new Citizen();
            var result = citizen.ToMaritalStatusTerminationDate();
            Assert.False(result.HasValue);
        }

        [Test]
        [TestCaseSource("TestDates")]
        public void ToMaritalStatusTerminationDate_InvalidUncertainty_ReturnsNull(DateTime maritalStatusDate)
        {
            var citizen = new Citizen() { MaritalStatusTerminationTimestamp = maritalStatusDate, MaritalStatusTerminationTimestampUncertainty = 'w' };
            var result = citizen.ToMaritalStatusTerminationDate();
            Assert.False(result.HasValue);
        }

        [Test]
        [TestCaseSource("TestDates")]
        public void ToMaritalStatusTerminationDate_Valid_ReturnsNull(DateTime maritalStatusDate)
        {
            var citizen = new Citizen() { MaritalStatusTerminationTimestamp = maritalStatusDate, MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue };
            var result = citizen.ToMaritalStatusTerminationDate();
            Assert.Null(result);
        }
        #endregion

        #region ToSpousePNR
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [TestCaseSource("InvalidCprNumbers")]
        public void ToSpousePNR_InvalidSpousePNR_ThrowsException(decimal spousePNR)
        {
            var citizen = new Citizen() { SpousePNR = spousePNR };
            citizen.ToSpousePNR();
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToSpousePNR_Valid_ThrowsException(decimal spousePNR)
        {
            var citizen = new Citizen() { SpousePNR = spousePNR };
            var result = citizen.ToSpousePNR();
            Assert.AreEqual(Converters.ToCprNumber(spousePNR), result);
        }
        #endregion

        #region ToSpouses

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToSpouses_NullConverter_ThrowsException()
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber() };
            citizen.ToSpouses(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToToSpouses_InvalidMaritalStatus_ThrowsException(
            [Values(' ', 'W')] char maritalStatus)
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus };
            citizen.ToSpouses(ToUuid);
        }

        [Test]
        public void ToSpouses_IrrelevantMaritalStatus_ZeroElements(
            [Values('U', 'P', 'O', 'L')] char maritalStatus)
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus };
            var result = citizen.ToSpouses(ToUuid);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void ToSpouses_InconsistentDates_ReturnsCorrect(
            [Values('E', 'F')]char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = DateTime.Today,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue,
                MaritalStatusTerminationTimestamp = DateTime.Today.AddDays(-2),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };
            var result = citizen.ToSpouses(ToUuid);
            Assert.AreEqual(citizen.MaritalStatusTimestamp, result[0].Virkning.TilTidspunkt.ToDateTime());
        }

        [Test]
        [Ignore]
        [ExpectedException(typeof(ArgumentException))]
        public void ToSpouses_TerminationDateForUnterminatedPartnership_ThrowsException()
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = 'G',
                MaritalStatusTerminationTimestamp = DateTime.Today.AddDays(-2),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };
            citizen.ToSpouses(ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToSpouses_InvalidSpousePnr_Exception(
            [Values('G', 'F', 'E')]char maritalStatus,
            [ValueSource("InvalidCprNumbers")]decimal spousePnr)
        {
            var citizen = new Citizen() { MaritalStatus = maritalStatus, SpousePNR = spousePnr };
            citizen.ToSpouses(ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToSpouses_ZeroSpousePnr_Exception(
            [Values('G', 'F', 'E')]char maritalStatus)
        {
            var citizen = new Citizen() { MaritalStatus = maritalStatus, SpousePNR = 0 };
            citizen.ToSpouses(ToUuid);
        }

        [Test]
        public void ToSpouses_Valid_Has1Element(
            [ValueSource("TestDates")] DateTime maritalStatusDate,
            [Values('G', 'F', 'E')] char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue
            };

            var result = citizen.ToSpouses(ToUuid);
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void ToSpouses_Valid_CorrectUuidReturned(
            [ValueSource("TestDates")] DateTime maritalStatusDate,
            [Values('G', 'F', 'E')] char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue
            };

            var result = citizen.ToSpouses(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(citizen.SpousePNR);
            Assert.AreEqual(uuids[stringCprNumber].ToString(), result[0].ReferenceID.Item);
        }

        [Test]
        [Combinatorial]
        public void ToSpouses_Unterminated_NullEndDate(
            [ValueSource("TestDates")] DateTime maritalStatusDate,
            [Values('G')] char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue,
                MaritalStatusTerminationTimestamp = maritalStatusDate.AddDays(1),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };

            var result = citizen.ToSpouses(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(citizen.SpousePNR);
            Assert.Null(result[0].Virkning.TilTidspunkt.ToDateTime());
        }

        [Test]
        [Combinatorial]
        public void ToSpouses_Terminated_HasEndDate(
            [ValueSource("TestDates")] DateTime maritalStatusDate,
            [Values('F', 'E')] char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue,
                MaritalStatusTerminationTimestamp = maritalStatusDate.AddDays(1),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };

            var result = citizen.ToSpouses(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(citizen.SpousePNR);
            Assert.NotNull(result[0].Virkning.TilTidspunkt.ToDateTime());
        }
        #endregion

        #region ToRegisteredPartners

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToRegisteredPartners_NullConverter_ThrowsException()
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber() };
            citizen.ToRegisteredPartners(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToRegisteredPartners_InvalidMaritalStatus_ThrowsException(
            [Values(' ', 'W')] char maritalStatus)
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus };
            citizen.ToRegisteredPartners(ToUuid);
        }

        [Test]
        public void ToRegisteredPartners_IrrelevantMaritalStatus_ZeroElements(
            [Values('E', 'F', 'G', 'U')] char maritalStatus)
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus };
            var result = citizen.ToRegisteredPartners(ToUuid);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void ToRegisteredPartners_InconsistentDates_ThrowsException(
            [Values('L', 'O')]char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = DateTime.Today,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue,
                MaritalStatusTerminationTimestamp = DateTime.Today.AddDays(-2),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };
            var result = citizen.ToRegisteredPartners(ToUuid);
            Assert.AreEqual(citizen.MaritalStatusTimestamp, result[0].Virkning.TilTidspunkt.ToDateTime());
        }

        [Test]
        [Ignore]
        [ExpectedException(typeof(ArgumentException))]
        public void ToRegisteredPartners_TerminationDateForUnterminatedPartnership_ThrowsException()
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = 'P',
                MaritalStatusTerminationTimestamp = DateTime.Today.AddDays(-2),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };
            citizen.ToRegisteredPartners(ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToRegisteredPartners_InvalidSpousePnr_Exception(
            [Values('L', 'O', 'P')]char maritalStatus,
            [ValueSource("InvalidCprNumbers")]decimal spousePnr)
        {
            var citizen = new Citizen() { MaritalStatus = maritalStatus, SpousePNR = spousePnr };
            citizen.ToRegisteredPartners(ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToRegisteredPartners_ZeroSpousePnr_Exception(
            [Values('L', 'O', 'P')]char maritalStatus)
        {
            var citizen = new Citizen() { MaritalStatus = maritalStatus, SpousePNR = 0 };
            citizen.ToRegisteredPartners(ToUuid);
        }

        [Test]
        public void ToRegisteredPartners_Valid_Has1Element(
            [ValueSource("TestDates")] DateTime maritalStatusDate,
            [Values('P', 'O', 'L')] char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue
            };

            var result = citizen.ToRegisteredPartners(ToUuid);
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void ToRegisteredPartners_Valid_CorrectUuidReturned(
            [ValueSource("TestDates")] DateTime maritalStatusDate,
            [Values('P', 'O', 'L')] char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue
            };

            var result = citizen.ToRegisteredPartners(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(citizen.SpousePNR);
            Assert.AreEqual(uuids[stringCprNumber].ToString(), result[0].ReferenceID.Item);
        }

        [Test]
        [Combinatorial]
        public void ToRegisteredPartners_Unterminated_NullEndDate(
            [ValueSource("TestDates")] DateTime maritalStatusDate,
            [Values('P')] char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue,
                MaritalStatusTerminationTimestamp = maritalStatusDate.AddDays(1),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };

            var result = citizen.ToRegisteredPartners(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(citizen.SpousePNR);
            Assert.Null(result[0].Virkning.TilTidspunkt.ToDateTime());
        }

        [Test]
        [Combinatorial]
        public void ToRegisteredPartners_Terminated_HasEndDate(
            [ValueSource("TestDates")] DateTime maritalStatusDate,
            [Values('O', 'L')] char maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue,
                MaritalStatusTerminationTimestamp = maritalStatusDate.AddDays(1),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };

            var result = citizen.ToRegisteredPartners(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(citizen.SpousePNR);
            Assert.NotNull(result[0].Virkning.TilTidspunkt.ToDateTime());
        }
        #endregion

        #region ToFather

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToFather_NullConverter_ThrowsException()
        {
            var citizen = new Citizen();
            citizen.FatherPNR = Utilities.RandomCprNumber();
            citizen.ToFather(null);
        }

        [Test]
        [TestCaseSource("InvalidCprNumbersWithNull")]
        [ExpectedException(typeof(ArgumentException))]
        public void ToFather_InvalidFatherPNR_Exception(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            citizen.ToFather(ToUuid);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_NotNull(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = citizen.ToFather(ToUuid);
            Assert.NotNull(result);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_SingleArrayElement(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = citizen.ToFather(ToUuid);
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_ElementNotNull(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = citizen.ToFather(ToUuid);
            Assert.NotNull(result[0]);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_ReferenceIDNotNull(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = citizen.ToFather(ToUuid);
            Assert.IsNotNull(result[0].ReferenceID);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_CorrectPnrPassed(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = citizen.ToFather(ToUuid);
            Assert.NotNull(result);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_CorrectUuid(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = citizen.ToFather(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.AreEqual(uuids[stringCprNumber].ToString(), result[0].ReferenceID.Item);
        }
        #endregion

        #region ToMother

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToMother_NullConverter_ThrowsException()
        {
            var citizen = new Citizen();
            citizen.MotherPNR = Utilities.RandomCprNumber();
            citizen.ToMother(null);
        }


        [Test]
        [TestCaseSource("InvalidCprNumbersWithNull")]
        [ExpectedException(typeof(ArgumentException))]
        public void ToMother_InvalidMotherPNR_Exception(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            citizen.ToMother(ToUuid);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_NotNull(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = citizen.ToMother(ToUuid);
            Assert.NotNull(result);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_SingleArrayElement(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = citizen.ToMother(ToUuid);
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_ElementNotNull(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = citizen.ToMother(ToUuid);
            Assert.NotNull(result[0]);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_ReferenceIDNotNull(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = citizen.ToMother(ToUuid);
            Assert.IsNotNull(result[0].ReferenceID);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_CorrectPnrPassed(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = citizen.ToMother(ToUuid);
            Assert.NotNull(result);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_CorrectUuid(decimal cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = citizen.ToMother(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.AreEqual(uuids[stringCprNumber].ToString(), result[0].ReferenceID.Item);
        }
        #endregion
    }
}
