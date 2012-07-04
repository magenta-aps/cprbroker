using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities.ConsoleApps;

namespace BatchClient
{
    class DoNothing : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            var count = 1000;
            var ret = new List<string>();
            for (int i = 0; i < count; i++)
            {
                ret.Add(RandomCprNumber().ToString().PadLeft(10, '0'));
            }
            return ret.ToArray();
        }

        static Random Random = new Random();
        public static decimal RandomCprNumber()
        {
            var day = Random.Next(1, 29).ToString("00");
            var month = Random.Next(1, 13).ToString("00");
            var year = Random.Next(1, 100).ToString("00");
            var part1 = Random.Next(1000, 9999).ToString();
            return decimal.Parse(day + month + year + part1);
        }

        public override void ProcessPerson(string joinedPnrBatch)
        {
            var wait = Random.Next(50, 400);
            System.Threading.Thread.Sleep(wait);
        }
    }
}
