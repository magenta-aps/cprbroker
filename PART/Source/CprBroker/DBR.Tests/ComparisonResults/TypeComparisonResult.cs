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
            return ToString(1);
        }

        public string ToString(int level)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(String.Format("h{0}. Type <{1}>, table <{2}>:", level, ClassName, SourceName));
            sb.AppendLine();

            if (IgnoreCount.HasValue)
            {
                //sb.AppendFormat("** Ignore row count <{0}>\r\n", IgnoreCount);
            }

            Action<string, ExclusionStatus> append = (title, reason) =>
            {
                var props = PropertyComparisonResult.OfStatus(Properties, reason);
                if (props.Count() > 0)
                {
                    sb.AppendLine();
                    sb.AppendFormat("h{0}. {1} ({2} fields, {3})", level + 1, title, ReasonCount(reason), ReasonPercentage(reason).ToString("P0"));
                    sb.AppendLine();
                    sb.AppendLine();

                    foreach (var prop in props)
                    {
                        sb.Append(prop.ToString("* "));
                    }
                }
            };

            append("Exact match", ExclusionStatus.OK);

            append("Non matching by nature (e.g. timestamps)", ExclusionStatus.LocalUpdateRelated);

            append("Matching values with inconsistent format from DPR", ExclusionStatus.InconsistentFormatting);
            append("Matching values with random alternation between (null) and '0' in real DPR", ExclusionStatus.NullOrZero);

            append("Marker fields whose data can differ if the reason is too old(e.g. a departure marker cause by a departure that is older than 20 years)", ExclusionStatus.InsufficientHistory);

            append("Data is not provided by the source", ExclusionStatus.UnavailableAtSource);

            append("These values can be retrieved only for current data, but not for historical data", ExclusionStatus.MissingInHistoricalRecords);
            append("There is no clear rule for how DPR fills these values", ExclusionStatus.InconsistentObservations);

            append("Difference in values returned from DPR viderstilling due to a non-match in the source DPR column", ExclusionStatus.CopiedFromNonMatching);

            append("Non matching for dead people", ExclusionStatus.Dead);
            append("Other reasons", ExclusionStatus.Unknown);


            return sb.ToString();
        }

        public int ReasonCount(ExclusionStatus reason)
        {
            return PropertyComparisonResult.OfStatus(Properties, reason).Count();
        }

        public decimal ReasonPercentage(ExclusionStatus reason)
        {
            return ((decimal)ReasonCount(reason)) / Properties.Count;
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
