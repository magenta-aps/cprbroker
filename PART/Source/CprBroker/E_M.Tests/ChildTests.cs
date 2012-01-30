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
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    public class ChildTests
    {
        [TearDown]
        public void ClearUUIDList()
        {
            lastCprNumber = null;
            lastUuid = Guid.Empty;
        }

        string lastCprNumber;
        Guid lastUuid;

        Guid ToUuid(string cprNumber)
        {
            var ret = Guid.NewGuid();
            lastCprNumber = cprNumber;
            lastUuid = ret;
            return ret;
        }
        decimal[] TestCprNumbers
        {
            get { return Utilities.RandomCprNumbers(3); }
        }

        private Child CreateChild(decimal cprNumber)
        {
            return new Child() { PNR = cprNumber };
        }

        decimal[] WrongCprNumbers = new decimal[] { 0, 10m, 23m };

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToPersonFlerRelationType_WrongPNR_ThrowsException(
            [ValueSource("WrongCprNumbers")] decimal cprNumber)
        {
            var child = CreateChild(cprNumber);
            child.ToPersonFlerRelationType(ToUuid);
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void ToPersonFlerRelationType_Valid_NotNull(decimal cprNumber)
        {
            var child = CreateChild(cprNumber);
            var result = child.ToPersonFlerRelationType(ToUuid);
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void ToPersonFlerRelationType_Valid_HasReferenceID(decimal cprNumber)
        {
            var child = CreateChild(cprNumber);
            var result = child.ToPersonFlerRelationType(ToUuid);
            Assert.IsNotNull(result.ReferenceID);
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void ToPersonFlerRelationType_Valid_CorrectCprNumberPassed(decimal cprNumber)
        {
            var child = CreateChild(cprNumber);
            var result = child.ToPersonFlerRelationType(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.AreEqual(lastCprNumber, stringCprNumber);
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void ToPersonFlerRelationType_Valid_ValidUuid(decimal cprNumber)
        {
            var child = CreateChild(cprNumber);
            var result = child.ToPersonFlerRelationType(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.AreEqual(lastUuid.ToString(), result.ReferenceID.Item);
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void ToPersonFlerRelationType_Valid_NullCommentText(decimal cprNumber)
        {
            var child = CreateChild(cprNumber);
            var result = child.ToPersonFlerRelationType(ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.IsNull(result.CommentText);
        }

    }
}
