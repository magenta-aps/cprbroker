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
using CprBroker.Schemas.Part;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.HistoryContinuity
{
    public abstract class Base<TInterface> where TInterface : ITimedType
    {
        protected abstract TInterface GetCurrent(IndividualResponseType pers);
        protected abstract List<TInterface> GetHistorical(IndividualResponseType pers);


        protected virtual bool ShouldHaveCurrent(IndividualResponseType pers)
        {
            return true;
        }

        protected virtual bool TimeOfDayMatters(IndividualResponseType pers)
        {
            return true;
        }

        public IndividualResponseType GetPerson(string pnr)
        {
            var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            return all.Where(p => p.PersonInformation.PNR == pnr).First();
        }

        public List<TInterface> GetObjects(IndividualResponseType pers)
        {
            var names = new List<TInterface>();
            names.AddRange(
                GetHistorical(pers)
                .OrderBy(n => n.ToStartTS())
                .ToArray());

            var current = GetCurrent(pers);
            if (current != null)
                names.Add(current);

            return names;
        }

        [Test]
        [TestCaseSource(typeof(Utilities), "PNRs")]
        public virtual void HistoryContinues(string pnr)
        {
            var pers = GetPerson(pnr);

            var objects = GetObjects(pers);
            for (int i = 0; i < objects.Count - 1; i++)
            {
                var current = objects[i];
                var next = objects[i + 1];
                if (this.TimeOfDayMatters(pers))
                {
                    Assert.AreEqual(current.ToEndTS().Value, next.ToStartTS().Value);
                }
                else
                {
                    Assert.AreEqual(current.ToEndTS().Value.Date, next.ToStartTS().Value.Date);
                }
            }
        }

        [Test]
        [TestCaseSource(typeof(Utilities), "PNRs")]
        public virtual void Current_NotNull(string pnr)
        {
            var pers = GetPerson(pnr);
            var obj = GetCurrent(pers);
            if (this.ShouldHaveCurrent(pers))
            {
                Assert.NotNull(obj);
            }
            else
            {
                Assert.Null(obj);
            }
        }

        //[Test]
        //[TestCaseSource("PNRs")]
        public void HistoryStartsAtBirth(string pnr)
        {
            var pers = GetPerson(pnr);

            var objects = GetObjects(pers);

            Assert.AreEqual(objects.First().ToStartTS(), pers.PersonInformation.Birthdate.Value);
        }

        [Test]
        [TestCaseSource(typeof(Utilities), "PNRs")]
        public virtual void HistoryRecordsHaveDifferentStartAndEndDates(string pnr)
        {
            var pers = GetPerson(pnr);
            var his = GetHistorical(pers);
            var all = new List<TInterface>();
            all.AddRange(his);

            foreach (var o in all)
            {
                if (o.ToStartTS().HasValue && o.ToEndTS().HasValue)
                {
                    Assert.AreNotEqual(o.ToStartTS().Value, o.ToEndTS().Value);
                }
                else if (o.ToStartTS().HasValue || o.ToEndTS().HasValue)
                {
                    Assert.AreNotEqual(o.ToStartTS(), o.ToEndTS());
                }
                else
                {
                    // Both null
                    throw new Exception("Both start and end dates cannot be null");
                }

            }
        }
    }
}
