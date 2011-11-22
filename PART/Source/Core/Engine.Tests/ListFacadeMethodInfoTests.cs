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
    class ListFacadeMethodInfoTests
    {
        [Test]
        public void ValidateInput_Null_Fails()
        {
            var facade = new ListFacadeMethodInfo(null, "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("400", result.StatusKode);
        }

        [Test]
        public void ValidateInput_NullUuidArray_Fails()
        {
            var facade = new ListFacadeMethodInfo(new ListInputType(), "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("400", result.StatusKode);
        }

        [Test]
        public void ValidateInput_ZeroUuids_Fails()
        {
            var facade = new ListFacadeMethodInfo(new ListInputType() { UUID = new string[0] }, "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("400", result.StatusKode);
        }

        [Test]
        [Combinatorial]
        public void ValidateInput_MixedValidAndInvalidUuids_Fails(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string validUuid,
            [Values(null, "", "kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var facade = new ListFacadeMethodInfo(new ListInputType() { UUID = new string[] { validUuid, inValidUuid } }, "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("400", result.StatusKode);
        }

        [Test]
        [Combinatorial]
        public void ValidateInput_MixedValidAndInvalidUuids_FailedUuidInStatusText(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string validUuid,
            [Values("kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var facade = new ListFacadeMethodInfo(new ListInputType() { UUID = new string[] { validUuid, inValidUuid } }, "", "");
            var result = facade.ValidateInput();
            StringAssert.Contains(inValidUuid, result.FejlbeskedTekst);
        }

        [Test]
        [Combinatorial]
        public void ValidateInput_RandomValidUuids_OK(
            [Values(10, 20, 200)]int count)
        {
            var facade = new ListFacadeMethodInfo(new ListInputType() { UUID = Utilities.RandomGuidStrings(count) }, "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("200", result.StatusKode);
        }

        [Test]
        [Combinatorial]
        public void ValidateInput_RandomValidUuids_EmptyStatusText(
            [Values(10, 20, 200)]int count)
        {
            var facade = new ListFacadeMethodInfo(new ListInputType() { UUID = Utilities.RandomGuidStrings(count) }, "", "");
            var result = facade.ValidateInput();
            Assert.AreEqual("OK", result.FejlbeskedTekst);
        }

        [Test]
        public void Initialize_Valid_CorrectCount(
            [Values(10, 20, 200)]int count)
        {
            var facade = new ListFacadeMethodInfo(new ListInputType() { UUID = Utilities.RandomGuidStrings(count) }, "", "");
            facade.Initialize();
            Assert.AreEqual(count, facade.SubMethodInfos.Length);
        }

        [Test]
        public void Initialize_Valid_CorrectSubMethodTypes(
            [Values(10, 20, 200)]int count)
        {
            var facade = new ListFacadeMethodInfo(new ListInputType() { UUID = Utilities.RandomGuidStrings(count) }, "", "");
            facade.Initialize();
            CollectionAssert.AllItemsAreInstancesOfType(facade.SubMethodInfos, typeof(ReadSubMethodInfo));
        }
    }
}
