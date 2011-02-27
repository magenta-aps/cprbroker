using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;

namespace CprBroker.Engine.Events
{
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
                using (var dataContext = new DAL.Events.DataChangeEventDataContext())
                {
                    var events = dataContext.DataChangeEvents.Take(maxCount).ToArray();

                    var ret = Array.ConvertAll<DAL.Events.DataChangeEvent, CprBroker.Schemas.Part.Events.DataChangeEventInfo>(
                        events,
                        ev => new Schemas.Part.Events.DataChangeEventInfo()
                        {
                            EventId = ev.DataChangeEventId,
                            PersonUuid = ev.PersonUuid,
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
            using (var dataConvext = new DAL.Part.PartDataContext())
            {
                var persons = dataConvext.PersonMappings.AsQueryable();
                if (personUuidToStartAfter.HasValue)
                {
                    persons = persons.Where(p => p.UUID.CompareTo(personUuidToStartAfter.Value) > 0);
                }
                persons = persons.OrderBy(p => p.UUID);
                persons = persons.Take(maxCount);

                return Array.ConvertAll<DAL.Part.PersonMapping, Schemas.Part.Events.PersonBirthdate>
                (
                    persons.ToArray(),
                    p => new Schemas.Part.Events.PersonBirthdate()
                    {
                        PersonUuid = p.UUID,
                        // TODO: Handle invalid Cpr numbers that will return null here
                        Birthdate = Strings.PersonNumberToDate(p.CprNumber).Value,
                    }
                );
            }
        }

        #endregion
    }
}
