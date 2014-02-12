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

namespace CprBroker.Tests.Engine
{
    namespace IPerCallDataProviderHelperTests
    {

        [TestFixture]
        public class GetOperationCost
        {
            public class DummyPerCallDataProvider : IPerCallDataProvider
            {
                public string[] OperationKeys { get; set; }

                public Dictionary<string, string> ConfigurationProperties { get; set; }

                public DataProviderConfigPropertyInfo[] ConfigurationKeys { get; set; }

                public bool IsAlive()
                { return true; }

                public Version Version
                { get { return null; } }
            }

            [Test]
            [ExpectedException]
            public void GetOperationCost_NullProv_Exception()
            {
                IPerCallDataProviderHelper.GetOperationCost(null, "aklskalfj");
            }

            [Test]
            public void GetOperationCost_NonExistingProperty_Zero()
            {
                var ret = IPerCallDataProviderHelper.GetOperationCost(new DummyPerCallDataProvider() { ConfigurationProperties = new Dictionary<string, string>() }, Utilities.RandomCprNumber());
                Assert.AreEqual(0m, ret);
            }

            [Test]
            public void GetOperationCost_ExistingProperty_CorrectValue(
                [Values(-1, 3, 7, 87.34)]decimal cost)
            {
                var prov = new DummyPerCallDataProvider() { ConfigurationProperties = new Dictionary<string, string>() };
                var opName = "OP";
                prov.ConfigurationProperties[IPerCallDataProviderHelper.ToOperationCostPropertyName(opName)] = cost.ToString();
                var ret = IPerCallDataProviderHelper.GetOperationCost(prov, opName);
                Assert.AreEqual(cost, ret);
            }
        }

        [TestFixture]
        public class CanCallOnline
        {
            static readonly string[] CorrectPNRs = new string[] { "2311783656", "1608593655" };
            static readonly string[] WrongPNRs = new string[] { "2311783650", "1608593667" };


            [Test]
            public void CanCallOnline_Mod11Enabled_CorrectPNR_True(
                [ValueSource("CorrectPNRs")]string pnr)
            {

                var ret = IPerCallDataProviderHelper.CanCallOnline(true, pnr);
                Assert.True(ret);
            }

            [Test]
            public void CanCallOnline_Mod11Enabled_WrongPNR_False(
                [ValueSource("WrongPNRs")]string pnr)
            {

                var ret = IPerCallDataProviderHelper.CanCallOnline(true, pnr);
                Assert.False(ret);
            }

            [Test]
            public void CanCallOnline_Mod11Disabled_CorrectPNR_True(
                [ValueSource("CorrectPNRs")]string pnr)
            {
                var ret = IPerCallDataProviderHelper.CanCallOnline(false, pnr);
                Assert.True(ret);
            }

            [Test]
            public void CanCallOnline_Mod11Disabled_WrongPNR_True(
                [ValueSource("WrongPNRs")]string pnr)
            {
                var ret = IPerCallDataProviderHelper.CanCallOnline(false, pnr);
                Assert.True(ret);
            }
        }
    }
}
