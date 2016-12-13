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
                var baseDbComparisonType = typeof(Comparison.Person.PersonComparisonTest<>);
                var dbTypes = baseDbComparisonType
                    .Assembly
                    .GetTypes()
                    .Where(t => Reflection.IsTypeDerivedFromGenericType(t, baseDbComparisonType) && !t.IsGenericType && t.BaseType.GetGenericArguments()[0]!= typeof(object))
                    .ToArray();

                var baseDiversionComparisonType = typeof(DiversionComparison.DiversionComparisonTest);
                var diversionTypes = baseDiversionComparisonType
                    .Assembly
                    .GetTypes()
                    .Where(t => baseDiversionComparisonType.IsAssignableFrom(t) && typeof(IComparisonType).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToArray();

                return dbTypes.Union(diversionTypes).ToArray();
            }
        }

        public string GenerateReport()
        {
            return ""
                + string.Join(
                    Environment.NewLine,
                    this.ComparisonTypes.Select(t => TypeComparisonResult.FromComparisonClass(t).ToString(2)))
                ;
        }

    }
}
