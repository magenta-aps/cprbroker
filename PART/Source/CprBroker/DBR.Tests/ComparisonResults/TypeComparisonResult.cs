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

        public static TypeComparisonResult FromComparisonClass(Type t)
        {
            var cmpObj = Reflection.CreateInstance(t) as IComparisonType;
            var tableType = cmpObj.TargetType;


            var typeMatch = new TypeComparisonResult() { ClassName = tableType.Name, SourceName = cmpObj.SourceName };

            var ignoreCountProp = t.GetProperty("IgnoreCount");
            typeMatch.IgnoreCount = ignoreCountProp == null ?
                null
                :
                (bool?)ignoreCountProp.GetValue(cmpObj);

            var allProperties = cmpObj.DataProperties();
            var excludedProperties = cmpObj.ExcludedPropertiesInformation;

            var fieldMatches = allProperties
                .Select(prop =>
                {
                    var exProp = excludedProperties.FirstOrDefault(ex => prop.Name.Contains(ex.PropertyName));
                    return PropertyComparisonResult.FromLinqProperty(prop, exProp == null ? ExclusionStatus.OK : exProp.ExclusionStatus);
                });

            typeMatch.Properties.AddRange(fieldMatches);

            return typeMatch;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(String.Format("* Type <{0}>, table <{1}>\r\n", ClassName, SourceName));

            if (IgnoreCount.HasValue)
            {
                sb.AppendFormat("** Ignore row count <{0}>\r\n", IgnoreCount);
            }

            Action<string, ExclusionStatus> append = (title, reason) =>
            {
                var props = PropertyComparisonResult.OfStatus(Properties, reason);
                if (props.Count() > 0)
                {
                    sb.AppendFormat("** ({0}) {1}", ReasonPercentage(reason).ToString("P0"), title);
                    foreach (var prop in props)
                    {
                        sb.Append(prop.ToString("*** "));
                    }
                }
            };

            append("Exact match\r\n", ExclusionStatus.OK);

            append("Matching values with inconsistent format from DPR\r\n", ExclusionStatus.InconsistentFormatting);
            append("Matching values with random alternation between (null) and '0' in real DPR\r\n", ExclusionStatus.NullOrZero);
            append("Non matching by nature (e.g. timestamps)\r\n", ExclusionStatus.LocalUpdateRelated);
            append("Data is not provided by the source\r\n", ExclusionStatus.UnavailableAtSource);
            append("Data can differ if the reason is too old\r\n", ExclusionStatus.InsufficientHistory);
            append("Difference in valuse returned from DPR viderstilling due to a non-match in the source DPR column\r\n", ExclusionStatus.CopiedFromNonMatching);
            append("Non matching for dead people\r\n", ExclusionStatus.Dead);
            append("Other reasons\r\n", ExclusionStatus.Unknown);


            return sb.ToString();
        }

        public decimal ReasonPercentage(ExclusionStatus reason)
        {
            return ((decimal)PropertyComparisonResult.OfStatus(Properties, reason).Count()) / Properties.Count;
        }

        public Dictionary<ExclusionStatus, decimal> ReasonPercentages
        {
            get
            {
                var reasons = Enum.GetValues(typeof(ExclusionStatus)).Cast<ExclusionStatus>().ToList();

                return reasons.ToDictionary(
                    r => r,
                    r => ReasonPercentage(r)
                    );
            }
        }
    }
}
