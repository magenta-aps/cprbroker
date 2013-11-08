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
using CprBroker.Providers.DPR;
using NUnit.Framework;

namespace BatchClient
{
    [TestFixture]
    class UtilitiesTests
    {
        [Test]
        public void DateFromDecimalString_Invalid_ReturnsNull(
            [Values(null, "jdhfkj", "20110101 ", "20110000", "20110101555", "2011")] string dateString)
        {
            var result = Utilities.DateFromDecimalString(dateString);
            Assert.Null(result);
        }

        [Test]
        public void DateFromDecimalString_Valid_ReturnsNotNull(
            [Values("20110101", "201101011255", "201212310000", "201100000099", "192900000099")] string dateString)
        {
            var result = Utilities.DateFromDecimalString(dateString);
            Assert.NotNull(result);
        }


        [Test]
        public void DateFromDecimalString_RealDPRDates_ReturnsNotNull(
            [Values(1, 10, 100, 1000, 10000, 100000, 1000000)] int count)
        {
            string fileName = "..\\..\\TestDates.txt";
            var lines = System.IO.File.ReadAllLines(fileName);
            count = Math.Min(lines.Length, count);
            Console.WriteLine(string.Format("Found <{0}>, processing <{1}>, remaining <{2}> lines", lines.Length, count, lines.Length - count));
            var errors = new List<string>();
            for (int iLine = 0; iLine < count; iLine++)
            {
                var dateString = lines[iLine];
                var result = Utilities.DateFromDecimalString(dateString);
                if (!result.HasValue)
                {
                    errors.Add(string.Format("Value = <{0}> At line <{1}>", dateString, iLine + 1));
                }
                if (dateString == "99")
                {
                    Console.WriteLine(result);
                }
            }
            Assert.AreEqual(0, errors.Count, string.Join("\r\n", errors.ToArray()));
        }
    }
}
