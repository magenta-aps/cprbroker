using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Engine
{
    [TestFixture]
    class ReadFacadeMethodInfoTests
    {

        [Test]
        public void ValidateInput_Null_ReturnsBadRequest()
        {
            var facade = new ReadFacadeMethodInfo(null, LocalDataProviderUsageOption.UseFirst, "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("400", result.StatusKode);
        }

        [Test]
        public void ValidateInput_InvalidUuid_ReturnsBadRequest(
            [Values(null, "", "kalskldjas", "2610802222", "Data kljaslkj")]string uuid)
        {
            var facade = new ReadFacadeMethodInfo(new LaesInputType() { UUID = uuid }, LocalDataProviderUsageOption.UseFirst, "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("400", result.StatusKode);
        }

        public void ValidateInput_SemiValidUuid_ReturnsBadRequest(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string uuid)
        {
            var facade = new ReadFacadeMethodInfo(new LaesInputType() { UUID = uuid + "E" }, LocalDataProviderUsageOption.UseFirst, "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("400", result.StatusKode);
        }

        [Test]
        public void ValidateInput_RandomUuid_ReturnsOK(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string uuid)
        {
            var facade = new ReadFacadeMethodInfo(new LaesInputType() { UUID = uuid }, LocalDataProviderUsageOption.UseFirst, "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("200", result.StatusKode);
        }

        [Test]
        public void Initialize_RandonUuid_OneSubMethod(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string uuid)
        {
            var facade = new ReadFacadeMethodInfo(new LaesInputType() { UUID = uuid }, LocalDataProviderUsageOption.UseFirst, "", "");
            facade.Initialize();
            Assert.AreEqual(1, facade.SubMethodInfos.Length);
        }

        [Test]
        public void Initialize_RandonUuid_SubMethodOfCorrectType(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string uuid)
        {
            var facade = new ReadFacadeMethodInfo(new LaesInputType() { UUID = uuid }, LocalDataProviderUsageOption.UseFirst, "", "");
            facade.Initialize();
            Assert.IsInstanceOf<ReadSubMethodInfo>(facade.SubMethodInfos[0]);
        }
    }
}
