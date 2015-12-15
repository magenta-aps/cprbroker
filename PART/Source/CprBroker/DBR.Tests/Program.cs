using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Tests.DBR
{
    class Program
    {
        public static void Main()
        {
            var cmp = new Comparison.ReportGenerator();
            var report = ""
                + cmp.GenerateReport(typeof(Comparison.Person.PersonComparisonTest<>))
                //+ cmp.GenerateReport(typeof(Comparison.Geo.GeoLookupComparisonTest<>))
                ;

            Console.WriteLine(report);
        }
    }
}
