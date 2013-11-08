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
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Objects
{
    namespace HistoricalNameTests
    {
        public class LoadAll
        {
            [Test]
            public void SSS()
            {
                var all = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var map = new Dictionary<string, Type>();
                map["026"] = typeof(HistoricalNameType);

                var rel = all
                    .Where(l => l.Code == "026")
                    .Select(l => l.ToWrapper(map) as HistoricalNameType)
                    .OrderBy(l => l.NameStartDate)
                    .GroupBy(l => l.PNR)
                    .Select(g => g.OrderBy(l => l.NameStartDate).First())
                    .GroupBy(p =>
                        {
                            var key = "";
                            if (!p.NameStartDate.HasValue)
                                return "Null";

                            var date = CprBroker.Utilities.Strings.PersonNumberToDate(p.PNR).Value;
                            if (p.NameStartDate.Value.Date == date)
                                return "Same Day";
                            if ((date - p.NameStartDate.Value).TotalDays < 30)
                                return "Month or less";
                            return "More than month";
                        })
                    .Select(g => new { Text = g.Key, Data = g.ToArray() })
                    .ToArray();
                var oo = "";
            }

            [Test]
            public void AAA()
            {
                var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);

                var grouped = all
                    .GroupBy(
                        p =>
                        {
                            var date = CprBroker.Utilities.Strings.PersonNumberToDate(p.PersonInformation.PNR).Value;
                            //DateTime? nameDate


                            if (p.HistoricalName.Count() == 0)
                            {
                                var n = p.CurrentNameInformation;
                                if (!n.NameStartDate.HasValue)
                                    return "1-1         Null         ";

                                if (n.NameStartDate.Value.Date == date)
                                    return "1-2         Same Day     ";
                                if ((date - n.NameStartDate.Value).TotalDays < 15)
                                    return "1-3       2 Weeks or less";
                                if ((date - n.NameStartDate.Value).TotalDays < 30)
                                    return "1-3         Month or less";
                                return "1-4         More than month";
                            }
                            var h = p.HistoricalName.OrderBy(n => n.NameStartDate).First();

                            if (!h.NameStartDate.HasValue)
                                return "2-1         Null       ";

                            if (h.NameStartDate.Value.Date == date)
                                return "2-2         Same Day   ";
                            if ((date - h.NameStartDate.Value).TotalDays < 15)
                                return "2-3     2 Weeks or less";
                            if ((date - h.NameStartDate.Value).TotalDays < 30)
                                return "2-3     Month or less  ";
                            return "2-4       More than month";
                        })
                        .Select(g => new { Status = g.Key, Data = g.ToArray() })
                        .OrderBy(g => g.Status)
                        .ToArray();
                object o = "";
            }
        }

    }
}
