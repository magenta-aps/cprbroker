using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CprBroker.Schemas.Part
{
    public interface IRegisteredInterval
    {
        DateTime? StartTS { get; set; }
        DateTime? EndTS { get; set; }
        DateTime? RegistrationDate { get; }
    }

    public class RegisteredInterval
    {
        public static TInterval[] MergeIntervals<TInterval>(params TInterval[] sourceIntervals)
            where TInterval : IRegisteredInterval
        {
            var sortedIntervals = sourceIntervals
                     .OrderByDescending(oio => oio.StartTS.HasValue ? oio.StartTS.Value : DateTime.MinValue)
                     .ThenByDescending(oio => oio.RegistrationDate.Value)
                     .ToArray();

            var ret = new List<TInterval>();
            if (sortedIntervals.Length > 0)
            {
                ret.Add(sortedIntervals.First());
            }

            foreach (var interval in sortedIntervals.Skip(1))
            {
                if (VirkningType.ToStartDateTimeOrMinValue(interval.StartTS) < VirkningType.ToStartDateTimeOrMinValue(ret.First().StartTS))
                {
                    if (!interval.EndTS.HasValue || interval.EndTS.Value > VirkningType.ToStartDateTimeOrMinValue(ret.First().StartTS))
                        interval.EndTS = VirkningType.ToStartDateTimeOrMinValue(ret.First().StartTS);

                    ret.Insert(0, interval);
                }
                else
                {
                    // Cannot insert any previous intervals if latest inserted interval has no start date
                }
            }

            return ret.ToArray();
        }

    }
}
