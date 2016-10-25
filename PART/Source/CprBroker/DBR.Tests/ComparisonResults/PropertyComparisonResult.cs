using CprBroker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DBR.ComparisonResults
{
    public class PropertyComparisonResult
    {
        public string PropertyName { get; set; }
        public string SourceName { get; set; }
        public bool IsMatch { get; set; }
        public string Remarks { get; set; }

        public override string ToString()
        {
            return string.Format("*** Property <{0}>, Column <{1}>, Match <{2}>\r\n", this.PropertyName, this.SourceName, this.IsMatch);
        }

        public PropertyComparisonResult()
        {
        }

        public PropertyComparisonResult(string name, string comment)
        {
            PropertyName = name;
            SourceName = null;
            IsMatch = false;
            Remarks = comment;
        }

        public static PropertyComparisonResult FromLinqProperty(PropertyInfo prop, bool isMatch)
        {
            return new PropertyComparisonResult()
            {
                PropertyName = prop.Name,
                SourceName = DataLinq.GetColumnName(prop),
                IsMatch = isMatch,
                Remarks = null,
            };
        }
    }
}
