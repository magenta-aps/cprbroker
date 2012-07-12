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
    }
}