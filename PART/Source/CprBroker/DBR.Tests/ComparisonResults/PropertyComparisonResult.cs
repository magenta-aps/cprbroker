using CprBroker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DBR.ComparisonResults
{
    public enum ExclusionStatus
    {
        /// <summary>
        /// OK Status - not excluded
        /// </summary>
        OK,

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
        /// Field is in diversion response, its source DPR column is not a 100% match
        /// </summary>
        CopiedFromNonMatching,

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

        InconsistentObservations,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }

    public class PropertyComparisonResult
    {
        public string PropertyName { get; set; }
        public string SourceName { get; set; }
        public bool IsExcluded { get { return ExclusionStatus != ExclusionStatus.OK; } }
        public string Remarks { get; set; }
        public ExclusionStatus ExclusionStatus { get; set; } = ExclusionStatus.OK;

        public override string ToString()
        {
            return ToString(null);
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

        public PropertyComparisonResult(string name, string comment, ExclusionStatus reason = ExclusionStatus.Unknown)
        {
            PropertyName = name;
            SourceName = null;
            Remarks = comment;
            ExclusionStatus = reason;
        }

        public static PropertyComparisonResult FromLinqProperty(PropertyInfo prop, ExclusionStatus reason = ExclusionStatus.Unknown)
        {
            return new PropertyComparisonResult()
            {
                PropertyName = prop.Name,
                SourceName = DataLinq.GetColumnName(prop),
                ExclusionStatus = reason,
                Remarks = null,
            };
        }

        public static IEnumerable<PropertyComparisonResult> Included(IEnumerable<PropertyComparisonResult> source)
        {
            return source.Where(f => !f.IsExcluded);
        }

        public static IEnumerable<PropertyComparisonResult> ExcludedAlways(IEnumerable<PropertyComparisonResult> source)
        {
            return source.Where(f => f.IsExcluded && f.ExclusionStatus != ComparisonResults.ExclusionStatus.Dead).ToList();
        }

        public static IEnumerable<PropertyComparisonResult> OfStatus(IEnumerable<PropertyComparisonResult> source, params ExclusionStatus[] reasons)
        {
            if (reasons == null)
                reasons = new ExclusionStatus[] { };

            return source.Where(s => reasons.Contains(s.ExclusionStatus));
        }

        public static IEnumerable<PropertyComparisonResult> ExceptStatus(IEnumerable<PropertyComparisonResult> source, params ExclusionStatus[] reasons)
        {
            if (reasons == null)
                reasons = new ExclusionStatus[] { };

            return source.Where(s => !reasons.Contains(s.ExclusionStatus));
        }
    }
}
