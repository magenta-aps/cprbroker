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
        /// <summary>
        /// Data in DPR is inconsistent if a person is inactive (Status = 20, 70, 80 or 90)
        /// </summary>
        Dead,

        /// <summary>
        /// Field is an update timestamp, must be different between the two systems
        /// </summary>
        LocalUpdateRelated,

        /// <summary>
        /// Field is not available in the source data
        /// </summary>
        UnavailableAtSource,

        /// <summary>
        /// The field can have a different value if there is not enough historical data
        /// Example: departure marker that is based on a departure that is older than 20 years
        /// </summary>
        InsufficientHistory,

        /// <summary>
        /// DPR sometimes puts a null instead of zero without a clear rule
        /// This is not an error
        /// </summary>
        NullOrZero,

        /// <summary>
        /// Matching values, but DPR uses different formatting randomly for the same values
        /// This is not an error
        /// </summary>
        InconsistentFormatting,

        /// <summary>
        /// Unknown
        /// </summary>
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
            return ToString("*** ");
        }

        public string ToString(string prefix)
        {
            return string.Format("{0}{1}{2}{3}",
                prefix,
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

        public static IEnumerable<PropertyComparisonResult> Excluded90(IEnumerable<PropertyComparisonResult> source)
        {
            return OfReason(source, ComparisonResults.ExclusionReason.Dead);
        }

        public static IEnumerable<PropertyComparisonResult> OfReason(IEnumerable<PropertyComparisonResult> source, ExclusionReason reason)
        {
            return source.Where(s => s.IsExcluded && s.ExclusionReason == reason);
        }
    }
}
