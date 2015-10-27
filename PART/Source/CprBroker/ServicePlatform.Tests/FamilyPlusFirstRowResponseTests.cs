using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Tests.CPRDirect;
using CprBroker.Schemas.Part;
using System.IO;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.ServicePlatform.Responses;
using System.Xml;

namespace CprBroker.Tests.ServicePlatform
{
    public class FamilyPlusFirstRowResponseTestBase : BaseResponseTests
    {
        public FamilyPlusFirstRowResponse GetResponse(string pnr)
        {
            var txt = GetResponse(pnr, "Familie+");
            var w = new FamilyPlusFirstRowResponse(txt);
            return w;
        }
    }

    [TestFixture]
    public class ToBirthDateTests : FamilyPlusFirstRowResponseTestBase
    {
        [Test]
        [TestCaseSource("PNRs")]
        public void ToNameStartDate_NotNull(string pnr)
        {
            var birthDate = GetResponse(pnr).MainItem.ToBirthdate();
            Assert.True(birthDate.HasValue);
        }
    }

    [TestFixture]
    public class ToNameStartDateTests : FamilyPlusFirstRowResponseTestBase
    {
        [Test]
        [TestCaseSource("PNRs")]
        public void ToNameStartDate_NotNull(string pnr)
        {
            var nameDate = GetResponse(pnr).MainItem.ToNameStartDate();
            Console.WriteLine(nameDate);

            if (nameDate.HasValue)
            {
                Assert.True(nameDate.HasValue);
            }
            else
            {
                var name3HasValue = false;
                name3HasValue = new FamilyPlusFirstRowResponse(GetResponse(pnr, ServiceInfo.NAVNE3_Local.Name)).MainItem.ToNameStartDate().HasValue;
                Assert.False(name3HasValue);
            }
        }
    }

}
