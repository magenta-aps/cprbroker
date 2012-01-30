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
using System.Data;
using System.Data.SqlClient;

namespace PersonMasterTestClient
{
    [TestFixture]
    class Tests
    {

        public static string[] RandomCprNumbers(int count)
        {
            var cprNumbers = new List<string>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                var day = random.Next(1, 29).ToString("00");
                var month = random.Next(1, 13).ToString("00");
                var year = random.Next(1, 100).ToString("00");
                var part1 = random.Next(1000, 9999).ToString();
                cprNumbers.Add(day + month + year + part1);
            }
            return cprNumbers.ToArray();
        }

        public static string[] SerialCprNumbers(int count)
        {
            var cprNumbers = new List<string>();

            for (int iDay = 1; iDay <= 28; iDay++)
            {
                for (int iMonth = 1; iMonth <= 12; iMonth++)
                {
                    for (int iYear = 1; iYear <= 99; iYear++)
                    {
                        for (int iPart = 1; iPart <= 9999; iPart++)
                        {
                            var day = iDay.ToString("00");
                            var month = iMonth.ToString("00");
                            var year = iYear.ToString("00");
                            var part1 = iPart.ToString("0000");

                            if (cprNumbers.Count < count)
                            {
                                cprNumbers.Add(day + month + year + part1);
                            }
                            else
                            {
                                return cprNumbers.ToArray();
                            }
                        }
                    }
                }
            }
            return cprNumbers.ToArray();
        }

        private string[] InvalidCprNumbers(int count)
        {
            string[] ret = new string[count];
            Random r = new Random();
            for (int i = 0; i < count; i++)
            {
                string cprNumber = r.Next().ToString() + r.Next().ToString();
                if (cprNumber.Length == 10)
                {
                    cprNumber = "99" + cprNumber.Substring(2);
                }
                ret[i] = cprNumber;
            }
            return ret;
        }

        private const string PersonMasterConnectionString = "";
        public int[] CprCounts = new[] { 0, 1, 2, 4, 8, 16, 32, 64, 128, 256, 512 /*, 1024, 2048, 4096, 8192, 16384, 32768, 65536*/ };

        [Test]
        public void TestGetUuidArrayOfRandomCpr(
            [ValueSource("CprCounts")] int count)
        {
            var cprNumbers = RandomCprNumbers(count);
            personmaster.BasicOpClient client = new personmaster.BasicOpClient();
            string aux = null;

            var ret = client.GetObjectIDsFromCprArray("", cprNumbers.ToArray(), ref aux);
            ValidateOutput(cprNumbers, ret);
        }

        [Test]
        public void TestGetUuidArrayOfCprSequence(
            [ValueSource("CprCounts")] int count)
        {
            var cprNumbers = SerialCprNumbers(count);
            personmaster.BasicOpClient client = new personmaster.BasicOpClient();
            string aux = null;

            var ret = client.GetObjectIDsFromCprArray("", cprNumbers.ToArray(), ref aux);
            ValidateOutput(cprNumbers, ret);
        }

        [Test]
        public void TestInvalidCprNumbers(
            [ValueSource("CprCounts")] int count)
        {
            var cprNumbers = InvalidCprNumbers(count);

            personmaster.BasicOpClient client = new personmaster.BasicOpClient();
            string aux = null;
            var ret = client.GetObjectIDsFromCprArray("", cprNumbers.ToArray(), ref aux);
            Assert.NotNull(aux, "Aux is null");
            if (count > 0)
            {
                Assert.Greater(aux.Length, 0, "Aux is empty");
            }
            for (int i = 0; i < count; i++)
            {
                Assert.IsNull(ret[i], string.Format("Cpr number {0} did not fail", cprNumbers[i]));
            }
        }

        [Test]
        public void TestNullValues(
            [ValueSource("CprCounts")] int count)
        {
            string[] cprNumbers=new string[count];
            personmaster.BasicOpClient client = new personmaster.BasicOpClient();
            string aux = null;
            var ret = client.GetObjectIDsFromCprArray("", cprNumbers.ToArray(), ref aux);
            Assert.NotNull(ret, "Output array is null");
            Assert.AreEqual(count, ret.Length);
        }

        public void ValidateOutput(string[] cprNumbers, Guid?[] objectIds)
        {
            Assert.IsNotNull(objectIds, "Return value is null");
            Assert.AreEqual(cprNumbers.Length, objectIds.Length, "Return value length does not equal input length");
            foreach (var uuid in objectIds)
            {
                Assert.AreNotEqual(Guid.Empty, uuid, "UUID is empty");
                Assert.AreEqual(1, objectIds.Where((id) => id == uuid).Count(), "Repeated UUID : " + uuid.ToString());
            }
        }

    }
}
