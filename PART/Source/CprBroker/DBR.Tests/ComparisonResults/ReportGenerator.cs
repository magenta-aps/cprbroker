using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;

namespace CprBroker.Tests.DBR.ComparisonResults
{
    public class ReportGenerator
    {
        public Type[] ComparisonTypes
        {
            get
            {
                var baseComparisonType = typeof(Comparison.Person.PersonComparisonTest<>);
                var types = baseComparisonType
                    .Assembly
                    .GetTypes()
                    .Where(t => Reflection.IsTypeDerivedFromGenericType(t, baseComparisonType) && !t.IsGenericType)
                    .ToArray();
                return types;
            }
        }

        public string GenerateReport()
        {
            return ""
                + "h3. Fields \r\n\r\n"
                + string.Join(
                    Environment.NewLine,
                    this.ComparisonTypes.Select(t => TypeComparisonResult.FromComparisonClass(t).ToString()))
                ;
        }

    }
}
