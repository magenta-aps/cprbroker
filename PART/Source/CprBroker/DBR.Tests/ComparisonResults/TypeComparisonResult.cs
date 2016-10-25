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
        public List<PropertyComparisonResult> Properties { get; } = new List<PropertyComparisonResult>();

        public List<PropertyComparisonResult> Included { get { return Properties.Where(f => f.IsMatch).ToList(); } }
        public List<PropertyComparisonResult> Excluded { get { return Properties.Where(f => !f.IsMatch).ToList(); } }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(String.Format("* Type <{0}>, table <{1}>\r\n", ClassName, SourceName));
            sb.Append("** Matching\r\n");

            if (Included.Count > 0)
            {
                foreach (var prop in Included)
                {
                    sb.Append(prop.ToString());
                }
            }
            else
            {
                sb.Append("** (None)\r\n");
            }

            sb.Append("** Non matching\r\n");
            if (Excluded.Count > 0)
            {
                foreach (var prop in Excluded)
                {
                    sb.Append(prop.ToString());
                }
            }
            else
            {
                sb.Append("*** (None)\r\n");
            }
            return sb.ToString();
        }
    }
}
