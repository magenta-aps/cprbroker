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
using CprBroker.Engine.Part;

namespace CprBroker.Tests.PartInterface
{
    namespace UuidCacheTests
    {
        [TestFixture]
        public class FillCache
        {
            [Test]
            public void FillCache_FakeUuidMethod_ReturnsSameCount([Random(0, 1000, 5)]int count)
            {
                var pnrs = Utilities.RandomCprNumbers(count);
                var cache = new UuidCache() { GetUuidArrayMethod = (arr) => arr.Select(p => Guid.NewGuid() as Guid?).ToArray() };
                var ret = cache.FillCache(pnrs);
                Assert.AreEqual(count, ret);
            }

            [Test]
            [ExpectedException]
            public void FillCache_NullMethod_Exception([Random(0, 1000, 5)]int count)
            {
                var pnrs = Utilities.RandomCprNumbers(count);
                var cache = new UuidCache() { GetUuidArrayMethod = null };
                var ret = cache.FillCache(pnrs);
            }

            [Test]
            [ExpectedException(ExpectedMessage = "abcdefg", MatchType = MessageMatch.Contains)]
            public void FillCache_Exception_Propagated([Random(0, 1000, 5)]int count)
            {
                var pnrs = Utilities.RandomCprNumbers(count);
                var cache = new UuidCache() { GetUuidArrayMethod = (kjasdlk) => { throw new Exception("abcdefg"); } };
                var ret = cache.FillCache(pnrs);
            }

            [Test]
            public void FillCache_Null_Zero()
            {
                var cache = new UuidCache();
                var ret = cache.FillCache(null);
                Assert.AreEqual(0, ret);
            }

            [Test]
            public void FillCache_InvalidUuidCount_Zero([Random(10, 1000, 5)]int count)
            {
                var cache = new UuidCache() { GetUuidArrayMethod = (arr) => arr.Take(arr.Length - 1).Select(p => Guid.NewGuid() as Guid?).ToArray() };
                var pnrs = Utilities.RandomCprNumbers(count);
                var sss = cache.GetUuidArrayMethod(pnrs);
                Assert.AreEqual(count - 1, sss.Count());
                var ret = cache.FillCache(pnrs);
                Assert.AreEqual(0, ret);
            }
        }
    }
}
