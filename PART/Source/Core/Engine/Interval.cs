using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine
{
    public class Interval
    {
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public List<ICurrentType> Data = new List<ICurrentType>();

        public static Interval[] CreateFromData(params ICurrentType[] dataObjects)
        {
            return CreateFromData(dataObjects.AsQueryable());
        }
        public static Interval[] CreateFromData(IQueryable<ICurrentType> dataObjects)
        {
            var allTags = dataObjects.Select(d => d.Tag).Distinct().OrderBy(d => d).ToArray();

            var groupedByStartTime = dataObjects.GroupBy(d => d.StartDate).OrderBy(g => g.Key).ToArray();
            var ret = new List<Interval>();

            var previousDataObjects = new List<ICurrentType>();

            for (int iTimeGroup = 0; iTimeGroup < groupedByStartTime.Count(); iTimeGroup++)
            {
                // TODO: Handle cases where StartDate is null - are these cases possible!!??
                var timeGroup = groupedByStartTime[iTimeGroup];
                var interval = new Interval() { StartTime = timeGroup.Key };
                interval.Data.AddRange(timeGroup.ToArray());

                var missingTags = allTags.Except(interval.Data.Select(d => d.Tag));

                foreach (var missingTag in missingTags)
                {
                    var tagObject = previousDataObjects.Where(o => string.Equals(o.Tag, missingTag)).LastOrDefault();

                    if (tagObject != null)
                    {
                        if (tagObject is IHistoryType)
                        {
                            // Make sure effect has not ended. Not sure if this scenario is possible
                            var o = tagObject as IHistoryType;
                            // TODO: What if interval.StartTime is null?
                            if (CprBroker.Utilities.Dates.DateRangeIncludes(o.StartDate, o.EndDate, interval.StartTime.Value, true))
                            {
                                interval.Data.Add(tagObject);
                            }
                        }
                        else
                        {
                            interval.Data.Add(tagObject);
                        }
                    }
                }
                interval.EndTime = interval.Data
                    .Where(d => d is IHistoryType)
                    .Select(d => (d as IHistoryType).EndDate)
                    .OrderBy(d => d as DateTime?)
                    .FirstOrDefault();
                // TODO: What if interval.StartTime is null?
                if (ret.LastOrDefault() != null && ret.Last().EndTime.Value > interval.StartTime.Value)
                {
                    ret.Last().EndTime = interval.StartTime;
                }

                ret.Add(interval);
                previousDataObjects.AddRange(timeGroup.ToArray());
            }
            return ret.ToArray();
        }
    }

    public interface ICurrentType
    {
        DateTime? StartDate { get; }
        string Tag { get; }
    }

    public interface IHistoryType : ICurrentType
    {
        DateTime? EndDate { get; }
    }
}
