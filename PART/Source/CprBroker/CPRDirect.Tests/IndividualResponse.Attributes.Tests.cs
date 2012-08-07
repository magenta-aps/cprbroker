using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace IndividualResponse.Attributes
    {
        [TestFixture]
        public class ToBirthDate
        {
            [Test]
            [ExpectedException]
            public void ToBirthDate_NullInformation_Exception()
            {
                var info = new IndividualResponseType();
                info.ToBirthDate();
            }

            [Test]
            public void ToBirthDate_Birthdate_OK()
            {
                var info = new IndividualResponseType() { PersonInformation = new PersonInformationType() { Birthdate = DateTime.Today, BirthdateUncertainty = ' ' } };
                var ret = info.ToBirthDate();
                Assert.AreEqual(DateTime.Today, ret);
            }

            [Test]
            public void ToBirthDate_NoBirthdate_FromPnr()
            {
                var info = new IndividualResponseType()
                {
                    PersonInformation = new PersonInformationType()
                    {
                        PNR = DateTime.Today.ToString("ddMMyy4111")
                    }
                };
                var ret = info.ToBirthDate();
                Assert.AreEqual(DateTime.Today, ret);
            }
        }

        [TestFixture]
        public class GetFolkeregisterAdresseSource
        {
            [Test]
            public void GetFolkeregisterAdresseSource_Empty_Dummy()
            {
                var db = new IndividualResponseType();
                var ret = db.GetFolkeregisterAdresseSource();
                Assert.IsInstanceOf<DummyAddressSource>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddress_Dummy()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 0, StreetCode = 0 } };
                var ret = db.GetFolkeregisterAdresseSource();
                Assert.IsInstanceOf<DummyAddressSource>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddressAndAddress_Dummy()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 0, StreetCode = 0 }, CurrentAddressInformation = new CurrentAddressInformationType() };
                var ret = db.GetFolkeregisterAdresseSource();
                Assert.IsInstanceOf<DummyAddressSource>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddressWithAndAddress_AddressWrapper()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 12, StreetCode = 0 }, CurrentAddressInformation = new CurrentAddressInformationType() }; ;
                var ret = db.GetFolkeregisterAdresseSource();
                Assert.IsInstanceOf<CurrentAddressWrapper>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddressWithEmptyDeparture_Dummy()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 12, StreetCode = 0 }, CurrentDepartureData = new CurrentDepartureDataType() }; ;
                var ret = db.GetFolkeregisterAdresseSource();
                Assert.IsInstanceOf<DummyAddressSource>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddressWithDeparture_Departure()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 12, StreetCode = 0 }, CurrentDepartureData = new CurrentDepartureDataType() { ForeignAddress1 = "DDD" } };
                var ret = db.GetFolkeregisterAdresseSource();
                Assert.IsInstanceOf<CurrentDepartureDataType>(ret);
            }

        }

        [TestFixture]
        public class ToFoedestedNavn
        {
            [Test]
            public void ToFoedestedNavn_NameWithNoDateOrHistory_Null()
            {
                var pnr = Utilities.RandomCprNumberString();
                var res = new IndividualResponseType()
                {
                    PersonInformation = new PersonInformationType() { PNR = pnr, Birthdate = CprBroker.Utilities.Strings.PersonNumberToDate(pnr) },
                    CurrentNameInformation = new CurrentNameInformationType() { PNR = pnr, FirstName_s = "1", LastName = "2" }
                };
                var ret = res.ToFoedestedNavn();
                Assert.IsNullOrEmpty(ret);
            }

            [Test]
            public void ToFoedestedNavn_NameWithDate_NotNull(
                [Values(0, 1, 2, 10, 14)]int dayOffset)
            {
                string pnr = Utilities.RandomCprNumberString();
                DateTime birthDate = CprBroker.Utilities.Strings.PersonNumberToDate(pnr).Value;
                DateTime nameDate = birthDate.AddDays(dayOffset);
                var res = new IndividualResponseType()
                {
                    PersonInformation = new PersonInformationType() { PNR = pnr, Birthdate = birthDate },
                    CurrentNameInformation = new CurrentNameInformationType() { PNR = pnr, FirstName_s = "1", LastName = "2", NameStartDate = nameDate }
                };

                var ret = res.ToFoedestedNavn();
                Assert.IsNotNullOrEmpty(ret);
            }

            [Test]
            public void ToFoedestedNavn_NameWithFarDate_Null(
                [Values(15, 20, 30)]int dayOffset)
            {
                string pnr = Utilities.RandomCprNumberString();
                DateTime birthDate = CprBroker.Utilities.Strings.PersonNumberToDate(pnr).Value;
                DateTime nameDate = birthDate.AddDays(dayOffset);
                var res = new IndividualResponseType()
                {
                    PersonInformation = new PersonInformationType() { PNR = pnr, Birthdate = birthDate },
                    CurrentNameInformation = new CurrentNameInformationType() { PNR = pnr, FirstName_s = "1", LastName = "2", NameStartDate = nameDate }
                };

                var ret = res.ToFoedestedNavn();
                Assert.IsNullOrEmpty(ret);
            }

        }
    }
}