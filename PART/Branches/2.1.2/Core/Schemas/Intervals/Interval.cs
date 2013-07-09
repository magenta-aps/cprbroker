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
    public class Interval
    {
        public DateTime? StartTS { get; private set; }
        public DateTime? EndTS { get; private set; }
        public List<ITimedType> Data = new List<ITimedType>();

        public static Interval[] CreateFromData(params ITimedType[] dataObjects)
        {
            return CreateFromData<Interval>(dataObjects.AsQueryable());
        }

        public static TInterval[] CreateFromData<TInterval>(IQueryable<ITimedType> dataObjects)
            where TInterval : Interval, new()
        {
            var allTags = dataObjects.Select(d => d.Tag).Distinct().OrderBy(d => d).ToArray();

            var groupedByStartTime = dataObjects.GroupBy(d => d.ToStartTS()).OrderBy(g => g.Key).ToArray();
            var ret = new List<TInterval>();

            var previousDataObjects = new List<ITimedType>();
            TInterval previousInterval = null;

            for (int iTimeGroup = 0; iTimeGroup < groupedByStartTime.Count(); iTimeGroup++)
            {
                // TODO: Handle cases where StartDate is null - are these cases possible!!??
                var timeGroup = groupedByStartTime[iTimeGroup];
                var interval = new TInterval() { StartTS = timeGroup.Key };
                interval.Data.AddRange(timeGroup.ToArray());

                var missingTags = allTags.Except(interval.Data.Select(d => d.Tag));

                foreach (var missingTag in missingTags)
                {
                    var tagObject = previousDataObjects.Where(o => string.Equals(o.Tag, missingTag)).LastOrDefault();

                    if (tagObject != null)
                    {
                        // Make sure effect has not ended. Not sure if this scenario is possible
                        var o = tagObject as ITimedType;
                        if (CprBroker.Utilities.Dates.DateRangeIncludes(o.ToStartTS(), o.ToEndTS(), interval.StartTS))
                        {
                            interval.Data.Add(tagObject);
                        }
                    }
                }
                interval.EndTS = interval.Data
                    .Select(d =>
                    {
                        var e = d.ToEndTS();
                        return e.HasValue ? e : DateTime.MaxValue;
                    })
                    .Min();
                if (interval.EndTS == DateTime.MaxValue)
                {
                    interval.EndTS = null;
                }
                if (previousInterval != null)
                {
                    // interval.StartTime cannot be null here because the only possible null is in first interval
                    if (!previousInterval.EndTS.HasValue || previousInterval.EndTS.Value > interval.StartTS.Value)
                    {
                        previousInterval.EndTS = interval.StartTS;
                    }
                }

                ret.Add(interval);
                previousDataObjects.AddRange(timeGroup.ToArray());
                previousInterval = interval;
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
