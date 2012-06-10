using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    public static class Utilities
    {
        private static Random Random = new Random();
        public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly char[] AlphabetChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToArray();

        public static char[] RandomChar(char[] arr, int count, params char[] exclude)
        {
            return RandomElement<char>(arr, count, exclude);
        }
        public static T[] RandomElement<T>(T[] arr, int count, params T[] exclude)
        {
            var ret = new List<T>();
            while (ret.Count < count)
            {
                int index = Random.Next(0, arr.Length);
                T val = arr[index];
                if (!exclude.Contains(val) && !ret.Contains(val))
                    ret.Add(val);
            }
            return ret.ToArray();
        }

        public static string RandomCprNumberString()
        {
            return Converters.ToPnrStringOrNull(Utilities.RandomCprNumber().ToString());
        }

        public static decimal RandomCprNumber()
        {
            var day = Random.Next(1, 29).ToString("00");
            var month = Random.Next(1, 13).ToString("00");
            var year = Random.Next(1, 100).ToString("00");
            var part1 = Random.Next(1000, 9999).ToString();
            return decimal.Parse(day + month + year + part1);
        }

        public static decimal[] RandomCprNumbers(int count)
        {
            var cprNumbers = new List<decimal>();

            for (int i = 0; i < count; i++)
            {
                cprNumbers.Add(RandomCprNumber());
            }
            return cprNumbers.ToArray();
        }
    }
}
