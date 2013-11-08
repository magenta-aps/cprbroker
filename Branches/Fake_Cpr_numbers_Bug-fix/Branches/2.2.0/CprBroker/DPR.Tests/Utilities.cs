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
using CprBroker.Schemas;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DPR
{
    public class Utilities
    {

        public static void ValidateNulls<TSource, TResult>(TSource address, TResult result)
            where TSource : class
            where TResult : class
        {
            if (address == null)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsNotNull(result);
            }
        }

        public static readonly Random Random = new Random();
        public enum NumDigits
        {
            Ten,
            Nine,
            NoneSpecified
        }
        public static decimal RandomCprNumber(NumDigits numDigits = NumDigits.NoneSpecified)
        {
            string day = "";
            switch (numDigits)
            {
                case NumDigits.NoneSpecified:
                    day = Random.Next(1, 29).ToString("00");
                    break;
                case NumDigits.Ten:
                    day = Random.Next(10, 29).ToString("00");
                    break;
                case NumDigits.Nine:
                    day = Random.Next(1, 10).ToString("00");
                    break;
            }

            var month = Random.Next(1, 13).ToString("00");
            var year = Random.Next(1, 100).ToString("00");
            var part1 = Random.Next(1000, 9999).ToString();
            return decimal.Parse(day + month + year + part1);
        }

        public static decimal[] RandomCprNumbers(int count, NumDigits numDigits = NumDigits.NoneSpecified)
        {
            var cprNumbers = new List<decimal>();

            for (int i = 0; i < count; i++)
            {
                cprNumbers.Add(RandomCprNumber(numDigits));
            }
            return cprNumbers.ToArray();
        }

        public static short RandomShort()
        {
            return (short)Random.Next(short.MaxValue);
        }

        public static short[] RandomShorts(int count)
        {
            var ret = new short[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = RandomShort();
            }
            return ret;
        }

        public static string RandomString()
        {
            return Guid.NewGuid().ToString().Substring(0, 10);
        }

        public static string[] RandomStrings(int count)
        {
            var ret = new string[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = RandomString();
            }
            return ret;
        }

        public static decimal RandomDecimalDate()
        {
            var day = Random.Next(1, 29);
            var month = Random.Next(1, 13);
            var year = Random.Next(1900, 2100);
            var date = new DateTime(year, month, day);
            return decimal.Parse(date.ToString("yyyyMMdd"));
        }

        public static decimal[] RandomDecimalDates(int count)
        {
            var ret = new decimal[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = RandomDecimalDate();
            }
            return ret;
        }
    }
}
