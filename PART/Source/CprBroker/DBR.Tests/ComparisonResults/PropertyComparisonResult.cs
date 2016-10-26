using CprBroker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DBR.ComparisonResults
{
    public enum ExclusionReason
    {
        Dead,
        LocalUpdateRelated,
        UnavailableAtSource,
        InsufficientHistory,
        Unknown
    }

    public class PropertyComparisonResult
    {
        public string PropertyName { get; set; }
        public string SourceName { get; set; }
        public bool IsExcluded { get; set; }
        public string Remarks { get; set; }
        public ExclusionReason? ExclusionReason { get; set; }

        public override string ToString()
        {
            return string.Format("*** {0}{1}{2}",
                string.Format("Property <{0}>, ", this.PropertyName),
                this.SourceName != null ? string.Format("Column <{0}>, ", this.SourceName) : null,
                string.Format("Excluded <{0}>\r\n", this.IsExcluded)
                );
        }

        public PropertyComparisonResult()
        {
        }

        public PropertyComparisonResult(string name, string comment, ExclusionReason? reason = null)
        {
            PropertyName = name;
            SourceName = null;
            IsExcluded = true;
            Remarks = comment;
            ExclusionReason = reason;
        }

        public static PropertyComparisonResult FromLinqProperty(PropertyInfo prop, bool isExcluded, ExclusionReason? reason = null)
        {
            return new PropertyComparisonResult()
            {
                PropertyName = prop.Name,
                SourceName = DataLinq.GetColumnName(prop),
                IsExcluded = isExcluded,
                ExclusionReason = reason,
                Remarks = null,
            };
        }

        public static IEnumerable<PropertyComparisonResult> Included(IEnumerable<PropertyComparisonResult> source)
        {
            return source.Where(f => !f.IsExcluded);
        }

        public static IEnumerable<PropertyComparisonResult> ExcludedAlways(IEnumerable<PropertyComparisonResult> source)
        {
            return source.Where(f => f.IsExcluded && f.ExclusionReason != ComparisonResults.ExclusionReason.Dead).ToList();
        }

        public static PropertyComparisonResult[] Excluded90(IEnumerable<PropertyComparisonResult> source)
        {
            return source.Where(s => s.IsExcluded && s.ExclusionReason == ComparisonResults.ExclusionReason.Dead).ToArray();
        }
    }
}
