using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;

namespace CprBroker.Tests.DBR.ComparisonResults
{
    public class ReportGenerator
    {
        public string GenerateReport(Type comparisonType)
        {
            var types = comparisonType
                .Assembly
                .GetTypes()
                .Where(t => Reflection.IsTypeDerivedFromGenericType(t, comparisonType) && !t.IsGenericType)
                .ToArray();

            var typeInfo = GetExclusionInformation(types);
            return string.Format(
                "h3. Fields \r\n\r\n"
                + string.Join("\r\n", typeInfo.Select(ti => ti.ToString()))
                + "\r\n"
                + "h3. Ignored count \r\n\r\n"
                + GenerateConutExclusionReport(types)
                );
        }

        public List<TypeComparisonResult> GetExclusionInformation(Type[] types)
        {
            var ret = new List<TypeComparisonResult>();
            foreach (var t in types)
            {
                var cmp = TypeComparisonResult.FromComparisonClass(t);
                if (cmp != null)
                    ret.Add(cmp);
            }
            return ret;
        }

        public String GenerateConutExclusionReport(Type[] types)
        {
            var ss = types
                .Select(t => new
                {
                    T = t,
                    Ign = (bool)t.InvokeMember(
                        "IgnoreCount",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty,
                        null,
                        Reflection.CreateInstance(t),
                        null)
                })
                .Where(o => o.Ign)
                .Select(o => string.Format(
                    "* Type <{0}> table <{1}>",
                    o.T.BaseType.GetGenericArguments().FirstOrDefault().Name,
                    DataLinq.GetTableName(o.T.BaseType.GetGenericArguments().FirstOrDefault())
                    ))
                .ToArray();

            return string.Join("\r\n", ss);
        }
    }
}
