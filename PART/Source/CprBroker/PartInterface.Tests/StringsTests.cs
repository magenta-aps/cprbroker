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
using CprBroker.PartInterface;

namespace CprBroker.Tests.PartInterface
{
    namespace StringsTests
    {
        [TestFixture]
        public class IsModulus11OK
        {
            [Test]
            public void IsModulus11OK_NormalCPR_True(
                [Values("2311783656", "1608593655")]string pnr)
            {
                var ret = CprBroker.PartInterface.Strings.IsModulus11OK(pnr);
                Assert.True(ret);
            }

            [Test]
            public void IsModulus11_Zeros_False(
                [Values("2311780000", "1608590000")]string pnr)
            {
                var ret = CprBroker.PartInterface.Strings.IsModulus11OK(pnr);
                Assert.False(ret);
            }

            [Test]
            public void IsModulus11_Wrong_False(
                [Values("2311783650", "1608593667")]string pnr)
            {
                var ret = CprBroker.PartInterface.Strings.IsModulus11OK(pnr);
                Assert.False(ret);
            }
        }

        [TestFixture]
        public class TrimAddressString
        {
            [Test]
            public void TrimAddressString_Null_Null()
            {
                var ret = Strings.TrimAddressString(null);
                Assert.Null(ret);
            }

            [Test]
            public void TrimAddressString_EmptyOrSemiEmpty_Empty(
                [Values("", " ", "0 0", " 0 0 0 ")] string s)
            {
                var ret = Strings.TrimAddressString(s);
                Assert.AreEqual("", ret);
            }

            [Test]
            [Sequential]
            public void TrimAddressString_abc_abc(
                [Values("abc", "   abc", "   abc   ", "00abc", "00  abc", "  00abc  ")] string input)
            {
                var ret = Strings.TrimAddressString(input);
                Assert.AreEqual("abc", ret);
            }
        }


    }
}
