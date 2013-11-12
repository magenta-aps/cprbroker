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
using CprBroker.Utilities;

namespace CprBroker.Tests.Utilities
{
    namespace StringsTests
    {
        [TestFixture]
        public class IsValidHostName
        {
            string[] ValidHostNames = new string[] { "hotmail.com", "personMaster", "1PersonMaster", "999pms.dk", "m.a.a", "code-valley", "c.m-ee.12kl", "ddd234", "kkk---ppp", "88-22", "f.92.15xyz", "12.code.abc" };
            string[] InvalidHostNames = new string[] { "", "-ddd", "def-", "22", "77.76", "klm.", "..", "....." };

            [Test]
            [TestCaseSource("ValidHostNames")]
            public void IsValidHostName_OK_ReturnsTrue(string hostName)
            {
                var result = CprBroker.Utilities.Strings.IsValidHostName(hostName);
                Assert.True(result);
            }

            [Test]
            [TestCaseSource("InvalidHostNames")]
            public void IsValidHostName_Invalid_ReturnsFalse(string hostName)
            {
                var result = CprBroker.Utilities.Strings.IsValidHostName(hostName);
                Assert.False(result);
            }
        }

        [TestFixture]
        public class EnsureStartString
        {
            [Test]
            [ExpectedException]
            public void EnsureStartString_NullStart_Exception()
            {
                var ss = "ssss";
                Strings.EnsureStartString(ss, null, true);
            }

            [Test]
            [ExpectedException]
            public void EnsureStartString_Null_Exception()
            {
                var ss = "ssss";
                Strings.EnsureStartString(null, ss, true);
            }
        }

        [TestFixture]
        public class IsModulus11OK
        {
            [Test]            
            public void IsModulus11OK_NormalCPR_True(
                [Values("2311783656","1608593655")]string pnr)
            {
                //System.Diagnostics.Debugger.Launch();
                var ret = Strings.IsModulus11OK(pnr);
                Assert.True(ret);
            }
            
            [Test]
            public void IsModulus11_Zeros_False(
                [Values("2311780000", "1608593668")]string pnr)
            {
                var ret = Strings.IsModulus11OK(pnr);
                Assert.False(ret);
            }

            [Test]
            public void IsModulus11_Wrong_False(
                [Values("2311783650", "1608593667")]string pnr)
            {
                var ret = Strings.IsModulus11OK(pnr);
                Assert.False(ret);
            }

            
        }

       
    }
}
