using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Tests.Engine
{
    public static class Utilities
    {
        public static Guid[] RandomGuids(int count)
        {
            var ret = new Guid[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = Guid.NewGuid();
            }
            return ret;
        }

        public static string[] RandomGuidStrings(int count)
        {
            return Utilities.RandomGuids(count).Select(id => id.ToString()).ToArray();
        }

        public static string[] RandomGuids5
        {
            get
            {
                return Utilities.RandomGuidStrings(5);
            }
        }

        public static string AppToken
        {
            get
            {
                return CprBroker.Utilities.Constants.BaseApplicationToken.ToString();
            }
        }

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
