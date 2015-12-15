using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;

namespace CprBroker.Tests.DBR.Comparison
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

            return string.Format(
                "h3. Fields \r\n\r\n"
                + GenerateExclusionReport(types)
                + "\r\n"
                + "h3. Ignored count \r\n\r\n"
                + GenerateConutExclusionReport(types)
                );
        }

        public string GenerateExclusionReport(Type[] types)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var t in types)
            {
                var dprType = t.BaseType.GetGenericArguments().FirstOrDefault();
                if (DataLinq.IsTable(dprType))
                {
                    var cmpObj = Reflection.CreateInstance(t);
                    sb.Append(String.Format("* Type <{0}>, table <{1}>\r\n", dprType.Name, DataLinq.GetTableName(dprType)));
                    var props = DataLinq.GetColumnProperties(dprType);
                    var propNames = t.InvokeMember("ExcludedProperties", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty, null, cmpObj, null) as string[];
                    if (propNames.Length > 0)
                    {
                        foreach (var propName in propNames)
                        {
                            var prop = props.Where(p => p.Name.Equals(propName)).Single();
                            sb.Append(string.Format("** Property <{0}>, Column <{1}>\r\n", prop.Name, DataLinq.GetColumnName(prop)));
                        }
                    }
                    else
                    {
                        sb.Append("** (None)\r\n");
                    }
                }
            }
            return sb.ToString();
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
