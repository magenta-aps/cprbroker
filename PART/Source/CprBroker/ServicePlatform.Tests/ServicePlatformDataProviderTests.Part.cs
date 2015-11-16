using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.CprServices;
using CprBroker.Providers.ServicePlatform.Responses;
using CprBroker.Providers.CprServices.Responses;
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Tests.ServicePlatform
{
    namespace ServicePlatformDataProviderTests
    {

        [TestFixture]
        public class PutSubscription : BaseResponseTests
        {
            [SetUp]
            public void InitContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "NUnit");
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void PutSubscription_OK(string pnr)
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var ret = prov.PutSubscription(new Schemas.PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() });
                Assert.True(ret);
            }
        }

        [TestFixture]
        public class ToRegistreringType1 : BaseResponseTests
        {
            public RegistreringType1 CallMethod(string pnr)
            {
                var cache = UuidCacheFactory.Create();

                var stamPlus = GetResponse(pnr, ServiceInfo.StamPlus_Local.Name);
                var familyPlus = GetResponse(pnr, ServiceInfo.FamilyPlus_Local.Name);

                var prov = new ServicePlatformDataProvider();
                return prov.ToRegistreringType1(stamPlus, familyPlus, cpr => cache.GetUuid(cpr));
            }

            [TestCaseSource("PNRs")]
            public void ToRegistreringType1_NotNull(string pnr)
            {
                Assert.NotNull(CallMethod(pnr));
            }

            [TestCaseSource("PNRs")]
            public void ToRegistreringType1_GenderOk(string pnr)
            {
                var ret = CallMethod(pnr);

                PersonGenderCodeType gender = (long.Parse(pnr) % 2) == 1 ? PersonGenderCodeType.male : PersonGenderCodeType.female;
                Assert.AreEqual(gender, ret.AttributListe.Egenskab.First().PersonGenderCode);
            }

            [TestCaseSource("PNRs")]
            public void ToRegistreringType1_SaveResult(string pnr)
            {
                var ret = CallMethod(pnr);
                var retXml = CprBroker.Utilities.Strings.SerializeObject(ret);
                var path = string.Format("..\\..\\Resources\\{0}.Part.xml", pnr);
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(retXml);
                doc.Save(path);
            }
        }
    }
}
