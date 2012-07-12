using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CPRDirect
{
    namespace ClearWrittenAddressTests
    {
        [TestFixture]
        public class Misc
        {
            [Test]
            public void CheckAddresses()
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var groupings = result.GroupBy(
                    person => new
                    {
                        //HasClearAddress = person.ClearWrittenAddress != null,
                        ValidClearAddress = !person.ClearWrittenAddress.IsEmpty,    // { related to each other }
                        CurrentAddress = person.CurrentAddressInformation != null,  // { related to each other }
                        ContactAddress = person.ContactAddress != null,
                        ForeignAddress = person.CurrentDepartureData != null,
                        ValidForeignAddress = person.CurrentDepartureData != null && !person.CurrentDepartureData.IsEmpty,
                        Status = person.PersonInformation.Status
                    })
                    //.Where(person => !person.Key.ValidClearAddress && !person.Key.ValidForeignAddress)
                    .OrderBy(g => g.Key.Status)
                    .Select(g => new { Key = g.Key, Value = g.ToArray() })
                    .ToArray();



                /* Findings
                 * If ClearWrittenAddress is empty, then CurrentAddressInformation is null (and vice versa)
                 * CurrentDepartureData can only contain value if CurrentAddressInformation is null
                 * Both CurrentAddressInformation and CurrentDepartureData is null if Status is 50,60,70 0r 90
                 * CurrentDepartureData (if not null) never contains empty values
                 */

                var p = result
                    .GroupBy(r => new { Empty = r.ClearWrittenAddress.IsEmpty })
                    .Select(g => new { Key = g.Key, Values = g.ToArray() })
                    .ToArray();
                //System.Diagnostics.Debugger.Launch();
                object o = "";

            }

        }

        [TestFixture]
        public class ToAddressPostalType
        {
            [Test]
            public void ToAddressPostalType_Normal_DanishCountryCode()
            {
                var db = new ClearWrittenAddressType() { PostCode = 0 };
                var ret = db.ToAddressPostalType();
                Assert.AreEqual("5100", ret.CountryIdentificationCode.Value);
            }
        }

        [TestFixture]
        public class IsEmpty
        {
            [Test]
            public void IsEmpty_Empty_True()
            {
                var db = new ClearWrittenAddressType() { PostCode = 0, MunicipalityCode = 0, StreetCode = 0 };
                var ret = db.IsEmpty;
                Assert.True(ret);
            }

            [Test]
            public void IsEmpty_StreetCode_False()
            {
                var db = new ClearWrittenAddressType() { PostCode = 0, MunicipalityCode = 0, StreetCode = 10 };
                var ret = db.IsEmpty;
                Assert.False(ret);
            }
        }

        [TestFixture]
        public class ToDanskAdresseType
        {
            [Test]
            public void ToDanskAdresseType_PostCodeValueOrNull_Ukendt(
                [Values(0, 10)] int postCode)
            {
                var db = new ClearWrittenAddressType() { PostCode = postCode, MunicipalityCode = 0, StreetCode = 0 };
                var ret = db.ToDanskAdresseType();
                Assert.AreEqual(db.IsEmpty, ret.UkendtAdresseIndikator);
            }
        }

        [TestFixture]
        public class ToAdresseType
        {
            [Test]
            public void ToAdresseType_Empty_DanishAddress()
            {
                var db = new ClearWrittenAddressType() { PostCode = 0, MunicipalityCode = 0, StreetCode = 0 };
                var ret = db.ToAdresseType();
                Assert.IsInstanceOf<DanskAdresseType>(ret.Item);
            }
        }
    }
}
