using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using NUnit.Framework;

namespace DPRClientTester
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
