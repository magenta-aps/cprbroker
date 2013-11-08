/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public interface IInterval
    {
        DateTime? StartTS { get; }
        DateTime? EndTS { get; }
    }

    public class Interval : IInterval
    {
        public virtual DateTime? StartTS { get; private set; }
        public virtual DateTime? EndTS { get; private set; }
        public List<ITimedType> Data = new List<ITimedType>();

        public DateTime? StartDate
        {
            get
            {
                if (StartTS.HasValue)
                    return StartTS.Value.Date;
                else
                    return null;
            }
        }

        // TODO: Remove this method if only used in tests
        public static Interval[] CreateFromData(params ITimedType[] dataObjects)
        {
            return CreateFromData<Interval>(dataObjects.AsQueryable());
        }

        public static TInterval[] CreateFromData<TInterval>(IQueryable<ITimedType> dataObjects)
            where TInterval : Interval, new()
        {
            var tags = dataObjects.Select(o => o.Tag).Distinct().ToArray();
            return CreateFromData<TInterval>(dataObjects, tags);
        }

        public static TInterval[] CreateFromData<TInterval>(IQueryable<ITimedType> dataObjects, DataTypeTags[] allTags)
            where TInterval : Interval, new()
        {

            dataObjects = dataObjects.Where(o => allTags.Contains(o.Tag));

            // Filter objects based on correction markers
            dataObjects = CorrectionMarker.Filter(dataObjects);

            // In case we have no correction records for history, check for possible overwrites
            dataObjects = Overwrite.Filter(dataObjects);

            // TODO: Also filter out objects that have StartDate<EndDate and an uncertainty marker on either dates (Like some HistoricalChurchInformation records)

            // sort by start date
            var sortedByStartDate = dataObjects
                .Select(o => new { StartTS = VirkningType.ToStartDateTimeOrMinValue(o.ToStartTS()), Object = o })
                .OrderBy(o => o.StartTS)
                .ToArray();

            var ret = new List<TInterval>();

            // Group by start date
            foreach (var dataObject in sortedByStartDate)
            {
                var currentInterval = ret.LastOrDefault();
                bool sameInterval =
                        currentInterval != null
                    &&
                    (
                        (dataObject.StartTS == currentInterval.StartTS.Value)
                        ||
                        (
                                currentInterval.StartDate.Value == currentInterval.StartTS.Value
                            && dataObject.StartTS.Date == currentInterval.StartDate
                        )
                    );

                if (sameInterval)
                {
                    currentInterval.Data.Add(dataObject.Object);
                    if (currentInterval.StartDate.Value == currentInterval.StartTS.Value)
                        currentInterval.StartTS = dataObject.StartTS;
                }
                else
                {
                    currentInterval = new TInterval()
                    {
                        StartTS = dataObject.StartTS,
                        EndTS = null,
                        Data = new List<ITimedType>(new ITimedType[] { dataObject.Object })
                    };
                    ret.Add(currentInterval);
                }
            }

            // Filter to only intervals that either have a non null start or that have objects that can mark interval start with null value
            ret = ret.Where(intvl => intvl.Data.Where(o => o.ToStartTS().HasValue || CreateIntervalIfStartTsIsNullAttribute.GetValue(o.GetType())).FirstOrDefault() != null).ToList();

            // Fill the missing information
            foreach (var currentInterval in ret)
            {
                var missingTags = allTags.Where(tag => currentInterval.Data.Where(o => o.Tag == tag).Count() == 0);
                foreach (var missingTag in missingTags)
                {
                    var missingTagObject = dataObjects.Where(
                        o => o.Tag == missingTag
                            && Utilities.Dates.DateRangeIncludes(o.ToStartTS(), o.ToEndTS(), currentInterval.StartTS)
                            ).FirstOrDefault();
                    if (missingTagObject != null)
                        currentInterval.Data.Add(missingTagObject);
                }
            }

            // Now set the end dates
            for (int i = 0; i < ret.Count; i++)
            {
                var currentInterval = ret[i];
                if (currentInterval.StartTS == DateTime.MinValue)
                    currentInterval.StartTS = null;

                // TODO: Do now allow Protection intervals to set end date for the last interval. Does that differ if the end date is in the future?
                currentInterval.EndTS = currentInterval.Data.Where(o => o.ToEndTS().HasValue).Select(o => o.ToEndTS()).Min();
                if (i < ret.Count - 1)
                {
                    var nextInterval = ret[i + 1];
                    if (currentInterval.EndTS == null || currentInterval.EndTS > nextInterval.StartTS)
                        currentInterval.EndTS = nextInterval.StartTS;
                }
            }

            // Now extend the last interval if it was closed by a type that cannot close open ends
            var lastInterval = ret.LastOrDefault();
            if (lastInterval != null && lastInterval.EndTS.HasValue)
            {
                var suggestedData = lastInterval.Data.Where(dataObject => CannotCloseOpenEndIntervalsAttribute.GetValue(dataObject.GetType()) == false).ToList();
                var suggestedEndDate = suggestedData.Where(d => d.ToEndTS().HasValue).Select(d => d.ToEndTS()).Max();
                if (
                        suggestedData.Count < lastInterval.Data.Count
                    && (!suggestedEndDate.HasValue || suggestedEndDate.Value > lastInterval.EndTS.Value)
                    )
                {
                    var suggestedInterval = new TInterval()
                    {
                        Data = suggestedData,
                        StartTS = lastInterval.EndTS,
                        EndTS = suggestedEndDate
                    };
                    ret.Add(suggestedInterval);
                }

            }

            return ret.ToArray();
        }

        public T GetData<T>() where T : class,ITimedType
        {
            return Data.Where(d => d is T).FirstOrDefault() as T;
        }

        public T GetData<T>(DataTypeTags tag) where T : class,ITimedType
        {
            return Data.Where(d => d is T && d.Tag == tag).FirstOrDefault() as T;
        }

        public VirkningType ToVirkningType()
        {
            return VirkningType.Create(this.StartTS, this.EndTS);
        }

    }


}
