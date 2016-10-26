using CprBroker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DBR.ComparisonResults
{
    public class TypeComparisonResult
    {
        public string ClassName { get; set; }
        public string SourceName { get; set; }
        public string Remarks { get; set; }
        public bool? IgnoreCount { get; set; } = null;

        public List<PropertyComparisonResult> Properties { get; } = new List<PropertyComparisonResult>();

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(String.Format("* Type <{0}>, table <{1}>\r\n", ClassName, SourceName));

            if (IgnoreCount.HasValue)
            {
                sb.AppendFormat("** Ignore row count <{0}>\r\n", IgnoreCount);
            }

            Action<string, IEnumerable<PropertyComparisonResult>> append = (title, props) =>
            {
                if (props.Count() > 0)
                {
                    sb.Append(title);
                    foreach (var prop in props)
                    {
                        sb.Append(prop.ToString("*** "));
                    }
                }
            };

            append("** Exact match\r\n", PropertyComparisonResult.Included(Properties));

            append("** Matching values with inconsistent format from DPR\r\n", PropertyComparisonResult.OfReason(Properties, ExclusionReason.InconsistentFormatting));
            append("** Matching values with random alternation between (null) and '0' in real DPR\r\n", PropertyComparisonResult.OfReason(Properties, ExclusionReason.NullOrZero));
            append("** Non matching by nature (e.g. timestamps)\r\n", PropertyComparisonResult.OfReason(Properties, ExclusionReason.LocalUpdateRelated));
            append("** Data is not provided by the source\r\n", PropertyComparisonResult.OfReason(Properties, ExclusionReason.UnavailableAtSource));
            append("** Data can differ if the reason is too old\r\n", PropertyComparisonResult.OfReason(Properties, ExclusionReason.InsufficientHistory));
            append("** Non matching for dead people\r\n", PropertyComparisonResult.OfReason(Properties, ExclusionReason.Dead));
            append("** Other reasons\r\n", PropertyComparisonResult.OfReason(Properties, ExclusionReason.Unknown, null));


            return sb.ToString();
        }

        public static TypeComparisonResult FromComparisonClass(Type t)
        {
            var cmpObj = Reflection.CreateInstance(t) as IComparisonType;
            var tableType = cmpObj.TargetType;

            //if (DataLinq.IsTable(tableType))
            {
                var typeMatch = new TypeComparisonResult() { ClassName = tableType.Name, SourceName = cmpObj.SourceName };

                var ignoreCountProp = t.GetProperty("IgnoreCount");
                typeMatch.IgnoreCount = ignoreCountProp == null ?
                    null
                    :
                    (bool?)ignoreCountProp.GetValue(cmpObj);

                var allProperties = cmpObj.DataProperties();
                var excludedProperties = cmpObj.ExcludedPropertiesInformation;

                var fieldMatches =
                    from prop in allProperties
                    join exProp in excludedProperties on prop.Name equals exProp.PropertyName into exProps
                    from exPorpName2 in exProps.DefaultIfEmpty()
                    select PropertyComparisonResult.FromLinqProperty(prop, exPorpName2 != null, exPorpName2?.ExclusionReason);

                typeMatch.Properties.AddRange(fieldMatches);

                return typeMatch;
            }
            //else
            {
                return null;
            }
        }
    }
}
