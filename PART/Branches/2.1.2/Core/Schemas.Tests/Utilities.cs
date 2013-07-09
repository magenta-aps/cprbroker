using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Tests.Schemas
{
    public static class Utilities
    {
        public static readonly Random Random = new Random();
        public static string RandomCprNumber()
        {
            var day = Random.Next(1, 29).ToString("00");
            var month = Random.Next(1, 13).ToString("00");
            var year = Random.Next(1, 100).ToString("00");
            var part1 = Random.Next(1000, 9999).ToString();
            return day + month + year + part1;
        }
    }
}
