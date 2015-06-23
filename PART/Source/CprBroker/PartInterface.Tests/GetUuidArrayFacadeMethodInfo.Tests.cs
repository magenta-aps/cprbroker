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
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.PartInterface
{
    namespace GetUuidArrayFacadeMethodInfoTests
    {
        [TestFixture]
        public class ValidateInput
        {
            [Test]
            public void ValidateInput_Correct_OK([Values(1, 10, 1000)]int count)
            {
                var pnrs = Utilities.RandomCprNumbers(count);
                var facade = new GetUuidArrayFacadeMethodInfo(pnrs, "", "");
                var ret = facade.ValidateInput();
                Assert.AreEqual("200", ret.StatusKode);
                Assert.AreEqual("OK", ret.FejlbeskedTekst);
            }

            [Test]
            public void ValidateInput_Zero_Invalid()
            {
                var pnrs = Utilities.RandomCprNumbers(0);
                var facade = new GetUuidArrayFacadeMethodInfo(pnrs, "", "");
                var ret = facade.ValidateInput();
                Assert.AreNotEqual("200", ret.StatusKode);
                Assert.AreNotEqual("OK", ret.FejlbeskedTekst);
            }

            [Test]
            public void ValidateInput_OneNull_Invalid([Random(0, 10, 3)] int nullIndex)
            {
                var pnrs = Utilities.RandomCprNumbers(10);
                pnrs[nullIndex] = null;
                var facade = new GetUuidArrayFacadeMethodInfo(pnrs, "", "");
                var ret = facade.ValidateInput();
                Assert.AreNotEqual("200", ret.StatusKode);
                Assert.AreNotEqual("OK", ret.FejlbeskedTekst);
            }
        }

        [TestFixture]
        public class AreConsistentUuids
        {
            public class GetUuidArrayFacadeMethodInfoStub : GetUuidArrayFacadeMethodInfo
            {
                public GetUuidArrayFacadeMethodInfoStub()
                    : base(null, null, null)
                { }
            }
            [Test]
            public void AreConsistentUuids_Empty_True()
            {
                Assert.True(GetUuidArrayFacadeMethodInfo.AreConsistentUuids(
                    new string[] { },
                    new string[] { }
                ));
            }
            [Test]
            public void AreConsistentUuids_One_True()
            {
                Assert.True(GetUuidArrayFacadeMethodInfo.AreConsistentUuids(
                    new string[] { Utilities.RandomCprNumber() },
                    new string[] { Guid.NewGuid().ToString() }
                ));
            }

            [Test]
            public void AreConsistentUuids_Two_True()
            {
                Assert.True(GetUuidArrayFacadeMethodInfo.AreConsistentUuids(
                    new string[] { Utilities.RandomCprNumber(), Utilities.RandomCprNumber(), },
                    new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
                ));
            }

            [Test]
            public void AreConsistentUuids_Two_False()
            {
                var pnr = Utilities.RandomCprNumber();

                Assert.False(GetUuidArrayFacadeMethodInfo.AreConsistentUuids(
                    new string[] { pnr, pnr },
                    new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
                ));
            }
        }
    }
}
