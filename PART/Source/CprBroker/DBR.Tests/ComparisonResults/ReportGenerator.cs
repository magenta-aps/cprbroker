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
                var dprType = t.BaseType.GetGenericArguments().FirstOrDefault();

                if (DataLinq.IsTable(dprType))
                {
                    var typeMatch = new TypeComparisonResult() { ClassName = dprType.Name, SourceName = Utilities.DataLinq.GetTableName(dprType) };
                    ret.Add(typeMatch);

                    var cmpObj = Reflection.CreateInstance(t);

                    var allProperties = DataLinq.GetColumnProperties(dprType);
                    var excludedPropertyNames = t.InvokeMember("ExcludedProperties", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty, null, cmpObj, null) as string[];

                    var fieldMatches =
                        from prop in allProperties
                        join exPropName in excludedPropertyNames on prop.Name equals exPropName into outer
                        from exPorpName2 in outer.DefaultIfEmpty()
                        select new PropertyComparisonResult()
                        {
                            PropertyName = prop.Name,
                            SourceName = DataLinq.GetColumnName(prop),
                            IsMatch = exPorpName2 == null,
                            Remarks = null,
                        };

                    typeMatch.Properties.AddRange(fieldMatches);
                }
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
