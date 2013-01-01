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
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Objects
{
    namespace ProtectionTypeTests
    {
        public class ValuesEnumerator
        {
            public static ProtectionType.ProtectionCategoryCodes[] GetValues()
            {
                return Enum.GetValues(typeof(ProtectionType.ProtectionCategoryCodes)).OfType<ProtectionType.ProtectionCategoryCodes>().ToArray();
            }
        }

        [TestFixture]
        public class HasProtection
        {
            [Test]
            public void HasProtection_CorrectType_True(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category)
            {
                var pro = new ProtectionType { ProtectionCategoryCode = category };
                var ret = pro.HasProtection(DateTime.Today, category);
                Assert.True(ret);
            }

            [Test]
            public void HasProtection_CorrectTypeWithDateRange_True(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category,
                [Values(0, 1, 2)]int offset)
            {
                DateTime today = DateTime.Today;
                var pro = new ProtectionType { ProtectionCategoryCode = category, StartDate = today.AddDays(-offset), EndDate = today.AddDays(offset) };
                var ret = pro.HasProtection(today, category);
                Assert.True(ret);
            }

            [Test]
            public void HasProtection_CorrectTypeOutOfDateRange_False(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category)
            {
                DateTime today = DateTime.Today;
                var pro = new ProtectionType { ProtectionCategoryCode = category, StartDate = today.AddDays(1), EndDate = today.AddDays(2) };
                var ret = pro.HasProtection(today, category);
                Assert.False(ret);
            }
        }

        [TestFixture]
        public class HasProtection2
        {
            [Test]
            [ExpectedException]
            public void HasProtection_Null_Exception(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category)
            {
                ProtectionType.HasProtection(null, DateTime.Today, category);
            }

            [Test]
            public void HasProtection_CorrectCategory_OK(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category)
            {
                var ret = ProtectionType.HasProtection(new ProtectionType[] { new ProtectionType() { ProtectionCategoryCode = category } }, DateTime.Today, category);
                Assert.True(ret);
            }
        }
    }
}
