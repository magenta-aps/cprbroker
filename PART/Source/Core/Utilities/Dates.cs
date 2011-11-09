using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Utilities
{
    public static class Dates
    {
        public static bool DateRangeIncludes(DateTime? startDate, DateTime? endDate, DateTime effectDate, bool nullStartDateOK)
        {
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                throw new ArgumentException(string.Format("startDate <{0}> must me less than or equal o endDate <{1}>", startDate, endDate));
            }
            bool startDateRange = nullStartDateOK ?
                !startDate.HasValue || startDate.Value <= effectDate
                : startDate.HasValue && startDate.Value <= effectDate;
            if (startDateRange)
            {
                return !endDate.HasValue || endDate.Value >= effectDate;
            }
            return false;
        }
    }
}
