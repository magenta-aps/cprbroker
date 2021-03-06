﻿/* ***** BEGIN LICENSE BLOCK *****
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
    public partial class VirkningType
    {
        public static VirkningType Create(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue)
            {
                var effectiveToDate = toDate;

                // If date range boundaries contain no time componenbt
                if (toDate.Value.TimeOfDay.TotalMilliseconds == 0)
                {
                    // Set toDate to end of day
                    effectiveToDate = toDate.Value.AddDays(1).AddMilliseconds(-1);
                }

                if (effectiveToDate < fromDate)
                {
                    throw new ArgumentException(string.Format("fromDate ({0}) should be less than or equal to toDate ({1})", fromDate, toDate));
                }
            }
            return new VirkningType()
            {
                // Actor not supported
                AktoerRef = null,
                // CommentText not supported
                CommentText = null,
                FraTidspunkt = TidspunktType.Create(fromDate),
                TilTidspunkt = TidspunktType.Create(toDate)
            };
        }

        public static VirkningType Compose(params VirkningType[] partialEffects)
        {
            if (partialEffects == null || partialEffects.Where(ef => ef == null || ef.FraTidspunkt == null || ef.TilTidspunkt == null).Count() > 0)
            {
                throw new ArgumentNullException("partialEffects");
            }

            var fromDate =
                partialEffects
                .Where(pe => pe.FraTidspunkt.ToDateTime().HasValue)
                .Select(pe => pe.FraTidspunkt.ToDateTime())
                .Min();

            var to =
                partialEffects
                .Where(pe => pe.TilTidspunkt.ToDateTime().HasValue)
                .Select(pe => pe.TilTidspunkt.ToDateTime())
                .Max();

            return VirkningType.Create(fromDate, to);
        }

        public static bool IsDoubleOpen(VirkningType v)
        {
            if (v == null)
                return true;
            if (!TidspunktType.ToDateTime(v.FraTidspunkt).HasValue && !TidspunktType.ToDateTime(v.TilTidspunkt).HasValue)
                return true;
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", FraTidspunkt, TilTidspunkt);
        }

        public bool Intersects(VirkningType otherEffect)
        {
            var v1 = VirkningType.Create(this.FraTidspunkt.ToDateTime(), this.TilTidspunkt.ToDateTime());
            var v2 = VirkningType.Create(otherEffect.FraTidspunkt.ToDateTime(), otherEffect.TilTidspunkt.ToDateTime());

            foreach (var v in new VirkningType[] { v1, v2 })
            {
                if (!v.FraTidspunkt.ToDateTime().HasValue)
                {
                    v.FraTidspunkt = TidspunktType.Create(DateTime.MinValue);
                }
                if (!v.TilTidspunkt.ToDateTime().HasValue)
                {
                    v.TilTidspunkt = TidspunktType.Create(DateTime.MaxValue);
                }
            }
            return v1.FraTidspunkt.ToDateTime().Value < v2.TilTidspunkt.ToDateTime().Value
                && v2.FraTidspunkt.ToDateTime().Value < v1.TilTidspunkt.ToDateTime().Value;
        }

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

        public static bool DateRangeIncludes(DateTime? startDate, DateTime? endDate, DateTime? effectDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                throw new ArgumentException(string.Format("startDate <{0}> must me less than or equal o endDate <{1}>", startDate, endDate));
            }
            if (!effectDate.HasValue)
            {
                return true;
            }
            else
            {
                var startDateRange = !startDate.HasValue || startDate.Value <= effectDate.Value;
                var endDateRange = !endDate.HasValue || endDate.Value >= effectDate.Value;
                return startDateRange && endDateRange;
            }
        }

        public static DateTime ToStartDateTimeOrMinValue(DateTime? startTS)
        {
            if (startTS.HasValue)
                return startTS.Value;
            else
                return DateTime.MinValue;
        }

        public static T[] OrderByStartDate<T>(T[] objects, bool ascending)
            where T : ITypeWithVirkning
        {
            if (objects == null)
            {
                return null;
            }
            else
            {
                var ret = objects
                    .OrderBy(o => o.Virkning.FraTidspunkt.ToDateTime())
                    .ThenBy(o => o.Virkning.TilTidspunkt.ToDateTime())
                    .ToArray();

                if (!ascending)
                    ret = ret.Reverse().ToArray();

                return ret;
            }
        }

        public static T[] TakeLatest<T>(T[] objects)
            where T : ITypeWithVirkning
        {
            var sorted = OrderByStartDate<T>(objects, false);

            if (sorted == null)
            {
                return null;
            }
            else
            {
                return sorted.Take(1).ToArray();
            }
        }

        public static T[] TrimFuture<T>(T[] objects, DateTime? lastDate = null)
            where T : ITypeWithVirkning
        {
            if (objects == null)
                return null;

            if (lastDate == null)
                lastDate = DateTime.Now;


            return objects
                .Where(o =>
                {
                    var fra = o.Virkning.FraTidspunkt.ToDateTime() ?? DateTime.MinValue;
                    return fra < lastDate.Value;
                }
                ).ToArray();
        }
    }
}
