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
    class ManagerPartTests
    {
        class ListFacadeMethodInfoStub : ListFacadeMethodInfo
        {
            public ListFacadeMethodInfoStub(string[] uuids)
                : base(new ListInputType() { UUID = uuids }, Utilities.AppToken, "")
            { }

            public ListFacadeMethodInfoStub()
                : this(null)
            {
            }

            public override void Initialize()
            {
                this.SubMethodInfos = input.UUID.Select(uuid => new ReadSubMethodInfoStub(uuid)).ToArray();
            }
        }

        public class PartReadDataProviderStub : IPartReadDataProvider
        {
            public static List<string> knownCprNumbers = new List<string>();

            public RegistreringType1 Read(CprBroker.Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out CprBroker.Schemas.QualityLevel? ql)
            {
                ql = Schemas.QualityLevel.DataProvider;
                if (knownCprNumbers.Contains(uuid.CprNumber))
                {
                    return new RegistreringType1();
                }
                return null;
            }


            #region IDataProvider Members

            public bool IsAlive()
            {
                throw new NotImplementedException();
            }

            public Version Version
            {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }

        public class ReadSubMethodInfoStub : ReadSubMethodInfo
        {
            public static Dictionary<string, string> knownUuids = new Dictionary<string, string>();

            public ReadSubMethodInfoStub(string uuid)
                : base(new LaesInputType() { UUID = uuid }, LocalDataProviderUsageOption.UseFirst)
            {
            }

            public override List<IDataProvider> GetDataProviderList()
            {
                var ret = new List<IDataProvider>();
                ret.Add(new PartReadDataProviderStub());
                return ret;
            }

            protected override CprBroker.Schemas.PersonIdentifier UuidToPersonIdentifier(string uuidString)
            {
                if (knownUuids.ContainsKey(uuidString))
                {
                    return new CprBroker.Schemas.PersonIdentifier() { UUID = new Guid(uuidString), CprNumber = knownUuids[uuidString] };
                }
                return null;
            }

            public override bool IsValidResult(RegistreringType1 result)
            {
                return result != null;
            }

            public override string PossibleErrorReason()
            {
                return base.PossibleErrorReason();
            }
        }

        ListOutputType1 GetMethodOutput(string[] uuids)
        {
            return Manager.GetMethodOutput<ListOutputType1, LaesResultatType[]>(
                    new ListFacadeMethodInfoStub(uuids));
        }

        [TearDown]
        public void ClearKnownUuids()
        {
            ReadSubMethodInfoStub.knownUuids.Clear();
            PartReadDataProviderStub.knownCprNumbers.Clear();
        }

        [Test]
        [RequiresThread]
        public void List_InvalidAppToken_ReturnsBadRequest()
        {
            Schemas.QualityLevel? ql;
            var ret = Manager.Part.List("jkhfkjahkfj", "ahsdfkhkajh", new ListInputType(), out ql);
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        [RequiresThread]
        public void List_InvalidAppToken_StatusTextContainsToken()
        {
            Schemas.QualityLevel? ql;
            var ret = Manager.Part.List("jkhfkjahkfj", "ahsdfkhkajh", new ListInputType(), out ql);
            StringAssert.Contains("token", ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        public void List_NullUuids_ReturnsBadRequest()
        {
            var ret = GetMethodOutput(null);
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void List_ZeroUuids_ReturnsBadRequest()
        {
            var ret = GetMethodOutput(new string[0]);
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void List_InvalidUuids_ReturnsBadRequest(
            [Values("kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetMethodOutput(new string[] { inValidUuid });
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void List_InvalidUuids_ReturnsUuidInStatusText(
            [Values("kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetMethodOutput(new string[] { inValidUuid });
            StringAssert.Contains(inValidUuid, ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        public void List_InvalidUuids_ReturnsNullLaesResultat(
            [Values("kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetMethodOutput(new string[] { inValidUuid });
            Assert.Null(ret.LaesResultat);
        }

        // 400 (BAD CLIENT REQUEST)
        [Test]
        public void List_MixedValidAndInvalidUuids_ReturnsBadRequest(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string validUuid,
            [Values(null, "", "kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetMethodOutput(new string[] { validUuid, inValidUuid });
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void List_MixedValidAndInvalidUuids_ReturnsUuidInStatusText(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string validUuid,
            [Values("kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetMethodOutput(new string[] { validUuid, inValidUuid });
            StringAssert.Contains(inValidUuid, ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        public void List_MixedValidAndInvalidUuids_ReturnsNullLaesResultat(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string validUuid,
            [Values(null, "", "kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetMethodOutput(new string[] { validUuid, inValidUuid });
            Assert.Null(ret.LaesResultat);
        }

        // 200 (OK)
        [Test]
        public void List_KnownUuids_ReturnsOK(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string validUuid)
        {
            var cprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(validUuid, cprNumber);
            PartReadDataProviderStub.knownCprNumbers.Add(cprNumber);
            var ret = GetMethodOutput(new string[] { validUuid });
            Assert.AreEqual("200", ret.StandardRetur.StatusKode);
        }

        [Test]
        [Sequential]
        public void List_MixedKnownAndUnknownUuids_ReturnsPartial(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string knownUuid,
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unknownUuid)
        {
            var cprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(knownUuid, cprNumber);
            PartReadDataProviderStub.knownCprNumbers.Add(cprNumber);
            var ret = GetMethodOutput(new string[] { knownUuid, unknownUuid });
            Assert.AreEqual("206", ret.StandardRetur.StatusKode);
        }

        [Test]
        [Sequential]
        public void List_MixedKnownAndUnknownUuids_StatusTextContainsUnknown(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string knownUuid,
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unknownUuid)
        {
            var cprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(knownUuid, cprNumber);
            PartReadDataProviderStub.knownCprNumbers.Add(cprNumber);
            var ret = GetMethodOutput(new string[] { knownUuid, unknownUuid });
            StringAssert.Contains("Unknown", ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        [Sequential]
        public void List_MixedKnownAndUnknownUuids_StatusTextContainsUnknownUuid(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string knownUuid,
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unknownUuid)
        {
            var cprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(knownUuid, cprNumber);
            PartReadDataProviderStub.knownCprNumbers.Add(cprNumber);
            var ret = GetMethodOutput(new string[] { knownUuid, unknownUuid });
            StringAssert.Contains(unknownUuid, ret.StandardRetur.FejlbeskedTekst);
        }


        [Test]
        [Sequential]
        public void List_MixedFoundAndNotFoundData_ReturnsPartial(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string foundUuid,
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid)
        {
            var foundCprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(foundUuid, foundCprNumber);
            PartReadDataProviderStub.knownCprNumbers.Add(foundCprNumber);

            var unfoundCprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(unfoundCprNumber, unfoundCprNumber);

            var ret = GetMethodOutput(new string[] { foundUuid, unfoundUuid });
            Assert.AreEqual("206", ret.StandardRetur.StatusKode);
        }

        [Test]
        [Sequential]
        public void List_MixedFoundAndNotFoundData_StatusTextContainsNotFound(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string foundUuid,
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid)
        {
            var foundCprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(foundUuid, foundCprNumber);
            PartReadDataProviderStub.knownCprNumbers.Add(foundCprNumber);

            var unfoundCprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(unfoundUuid, unfoundCprNumber);
            
            var ret = GetMethodOutput(new string[] { foundUuid, unfoundUuid });
            StringAssert.Contains("Data provider failed", ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        [Sequential]
        public void List_MixedFoundAndNotFoundData_StatusTextContainsNotFoundUuids(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string foundUuid,
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid)
        {
            var foundCprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(foundUuid, foundCprNumber);
            PartReadDataProviderStub.knownCprNumbers.Add(foundCprNumber);

            var unfoundCprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(unfoundCprNumber, unfoundCprNumber);

            var ret = GetMethodOutput(new string[] { foundUuid, unfoundUuid });
            StringAssert.Contains(unfoundUuid, ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        [Sequential]
        public void List_UnknownUuids_ReturnsDataSourceUnavailable(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid1,
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid2)
        {
            var ret = GetMethodOutput(new string[] { unfoundUuid1, unfoundUuid2 });
            Assert.AreEqual("503", ret.StandardRetur.StatusKode);
        }

        [Test]
        [Sequential]
        public void List_NotFoundUuids_ReturnsDataSourceUnavailable(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid1,
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid2)
        {
            var unfoundCprNumber1 = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(unfoundUuid1, unfoundCprNumber1);

            var unfoundCprNumber2 = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(unfoundCprNumber2, unfoundCprNumber2);

            var ret = GetMethodOutput(new string[] { unfoundUuid1, unfoundUuid2 });
            Assert.AreEqual("503", ret.StandardRetur.StatusKode);
        }

    }
}
