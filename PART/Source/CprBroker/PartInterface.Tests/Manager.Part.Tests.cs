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
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Data.DataProviders;

namespace CprBroker.Tests.Engine
{
    [TestFixture]
    class ManagerPartTests
    {
        #region Stubs

        class ReadFacadeMethodInfoStub : ReadFacadeMethodInfo
        {
            public ReadFacadeMethodInfoStub(string uuid)
                : base(new LaesInputType() { UUID = uuid }, SourceUsageOrder.LocalThenExternal, Utilities.AppToken, "")
            { }

            public ReadFacadeMethodInfoStub()
                : this(null)
            {
            }

            public override void Initialize()
            {
                this.SubMethodInfos = new SubMethodInfo[] { new ReadSubMethodInfoStub(Input.UUID) };
            }
        }

        class ListFacadeMethodInfoStub : ListFacadeMethodInfo
        {
            public ListFacadeMethodInfoStub(string[] uuids)
                : base(new ListInputType() { UUID = uuids }, CprBroker.Schemas.SourceUsageOrder.LocalThenExternal, Utilities.AppToken, "")
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
                ql = CprBroker.Schemas.QualityLevel.DataProvider;
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
                : base(new LaesInputType() { UUID = uuid }, SourceUsageOrder.LocalThenExternal)
            {
            }

            public override IEnumerable<IDataProvider> GetDataProviderList(DataProvidersConfigurationSection section, DataProvider[] dbProviders)
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

        #endregion

        #region Common Methods

        ListOutputType1 GetListMethodOutput(string[] uuids)
        {
            return Manager.GetMethodOutput<ListOutputType1, LaesResultatType[]>(
                    new ListFacadeMethodInfoStub(uuids));
        }

        LaesOutputType GetReadMethodOutput(string uuid)
        {
            return Manager.GetMethodOutput<LaesOutputType, LaesResultatType>(
                    new ReadFacadeMethodInfoStub(uuid));
        }

        [TearDown]
        public void ClearKnownUuids()
        {
            ReadSubMethodInfoStub.knownUuids.Clear();
            PartReadDataProviderStub.knownCprNumbers.Clear();
        }

        #endregion

        #region List

        [Test]
        [RequiresThread]
        public void List_InvalidAppToken_ReturnsBadRequest()
        {
            CprBroker.Schemas.QualityLevel? ql;
            var ret = PartManager.List("jkhfkjahkfj", "ahsdfkhkajh", new ListInputType(), SourceUsageOrder.LocalThenExternal, out ql);
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        [RequiresThread]
        public void List_InvalidAppToken_StatusTextContainsToken()
        {
            CprBroker.Schemas.QualityLevel? ql;
            var ret = PartManager.List("jkhfkjahkfj", "ahsdfkhkajh", new ListInputType(), SourceUsageOrder.LocalThenExternal, out ql);
            StringAssert.Contains("token", ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        public void List_NullUuids_ReturnsBadRequest()
        {
            var ret = GetListMethodOutput(null);
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void List_ZeroUuids_ReturnsBadRequest()
        {
            var ret = GetListMethodOutput(new string[0]);
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void List_InvalidUuids_ReturnsBadRequest(
            [Values("kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetListMethodOutput(new string[] { inValidUuid });
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void List_InvalidUuids_ReturnsUuidInStatusText(
            [Values("kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetListMethodOutput(new string[] { inValidUuid });
            StringAssert.Contains(inValidUuid, ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        public void List_InvalidUuids_ReturnsNullLaesResultat(
            [Values("kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetListMethodOutput(new string[] { inValidUuid });
            Assert.Null(ret.LaesResultat);
        }

        // 400 (BAD CLIENT REQUEST)
        [Test]
        public void List_MixedValidAndInvalidUuids_ReturnsBadRequest(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string validUuid,
            [Values(null, "", "kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetListMethodOutput(new string[] { validUuid, inValidUuid });
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void List_MixedValidAndInvalidUuids_ReturnsUuidInStatusText(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string validUuid,
            [Values("kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetListMethodOutput(new string[] { validUuid, inValidUuid });
            StringAssert.Contains(inValidUuid, ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        public void List_MixedValidAndInvalidUuids_ReturnsNullLaesResultat(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string validUuid,
            [Values(null, "", "kalskldjas", "2610802222", "Data kljaslkj")]string inValidUuid)
        {
            var ret = GetListMethodOutput(new string[] { validUuid, inValidUuid });
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
            var ret = GetListMethodOutput(new string[] { validUuid });
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
            var ret = GetListMethodOutput(new string[] { knownUuid, unknownUuid });
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
            var ret = GetListMethodOutput(new string[] { knownUuid, unknownUuid });
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
            var ret = GetListMethodOutput(new string[] { knownUuid, unknownUuid });
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

            var ret = GetListMethodOutput(new string[] { foundUuid, unfoundUuid });
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

            var ret = GetListMethodOutput(new string[] { foundUuid, unfoundUuid });
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

            var ret = GetListMethodOutput(new string[] { foundUuid, unfoundUuid });
            StringAssert.Contains(unfoundUuid, ret.StandardRetur.FejlbeskedTekst);
        }

        [Test]
        [Sequential]
        public void List_UnknownUuids_ReturnsDataSourceUnavailable(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid1,
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid2)
        {
            var ret = GetListMethodOutput(new string[] { unfoundUuid1, unfoundUuid2 });
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

            var ret = GetListMethodOutput(new string[] { unfoundUuid1, unfoundUuid2 });
            Assert.AreEqual("503", ret.StandardRetur.StatusKode);
        }

        #endregion

        #region Read

        [Test]
        public void Read_InvalidUuid_ReturnsBadRequest(
            [Values(null, "", "jkahkjhkj", "uqy876hkjhjk")]string uuid)
        {
            var ret = GetReadMethodOutput(uuid);
            Assert.AreEqual("400", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void Read_UnknownUuid_ReturnsDataSourceUnavailable(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unknownUuid)
        {
            var ret = GetReadMethodOutput(unknownUuid);
            Assert.AreEqual("503", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void Read_NotFoundUuidData_ReturnsDataSourceUnavailable(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string unfoundUuid)
        {
            ReadSubMethodInfoStub.knownUuids.Add(unfoundUuid, Utilities.RandomCprNumber());
            var ret = GetReadMethodOutput(unfoundUuid);
            Assert.AreEqual("503", ret.StandardRetur.StatusKode);
        }

        [Test]
        public void Read_FoundUuid_ReturnsOK(
            [ValueSource(typeof(Utilities), "RandomGuids5")]string foundUuid)
        {
            var foundCprNumber = Utilities.RandomCprNumber();
            ReadSubMethodInfoStub.knownUuids.Add(foundUuid, foundCprNumber);
            PartReadDataProviderStub.knownCprNumbers.Add(foundCprNumber);
            var ret = GetReadMethodOutput(foundUuid);
            Assert.AreEqual("200", ret.StandardRetur.StatusKode);
        }

        #endregion
    }
}
