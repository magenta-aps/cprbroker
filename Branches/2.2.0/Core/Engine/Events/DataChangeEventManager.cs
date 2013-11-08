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
using CprBroker.Utilities;

namespace CprBroker.Engine.Events
{
    /// <summary>
    /// Data provider that allows the Event Broker to get information from Cpr Broker
    /// </summary>
    public class DataChangeEventManager : IDataChangeEventManager
    {
        public static readonly object QueueLock = new object();


        #region IDataProvider Members
        public bool IsAlive()
        {
            return true;
        }

        public Version Version
        {
            get { return new Version(Constants.Versioning.Major, Constants.Versioning.Minor); }
        }
        #endregion


        #region IDataChangeEventManager Members

        public Schemas.Part.Events.DataChangeEventInfo[] DequeueEvents(int maxCount)
        {
            lock (QueueLock)
            {
                using (var dataContext = new Data.Events.DataChangeEventDataContext())
                {
                    var events = dataContext.DataChangeEvents.Take(maxCount).ToArray();

                    var ret = Array.ConvertAll<Data.Events.DataChangeEvent, CprBroker.Schemas.Part.Events.DataChangeEventInfo>(
                        events,
                        ev => new Schemas.Part.Events.DataChangeEventInfo()
                        {
                            EventId = ev.DataChangeEventId,
                            PersonUuid = ev.PersonUuid,
                            PersonRegistrationId = ev.PersonRegistrationId,
                            ReceivedDate = ev.ReceivedDate
                        }
                     );
                    dataContext.DataChangeEvents.DeleteAllOnSubmit(events);
                    dataContext.SubmitChanges();
                    return ret;
                }
            }
        }

        public CprBroker.Schemas.Part.Events.PersonBirthdate[] GetPersonBirthdates(Guid? personUuidToStartAfter, int maxCount)
        {
            using (var dataConvext = new Data.Part.PartDataContext())
            {
                var persons = dataConvext.PersonMappings.AsQueryable();
                if (personUuidToStartAfter.HasValue)
                {
                    persons = persons.Where(p => p.UUID.CompareTo(personUuidToStartAfter.Value) > 0);
                }
                persons = persons.OrderBy(p => p.UUID);
                persons = persons.Take(maxCount);

                return Array.ConvertAll<Data.Part.PersonMapping, Schemas.Part.Events.PersonBirthdate>
                (
                    persons.ToArray(),
                    p => new Schemas.Part.Events.PersonBirthdate()
                    {
                        PersonUuid = p.UUID,
                        // TODO: Handle invalid Cpr numbers that will return null here
                        Birthdate = Strings.PersonNumberToDate(p.CprNumber),
                    }
                );
            }
        }

        #endregion
    }
}
