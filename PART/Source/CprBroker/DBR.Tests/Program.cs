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
            var cmp = new ComparisonResults.ReportGenerator();
            var report = cmp.GenerateReport();
            

            Console.WriteLine(report);
        }
    }
}
