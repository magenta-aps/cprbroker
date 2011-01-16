using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Events
{
    public class DataChangeEventManager : IDataChangeEventManager
    {
        public static readonly object QueueLock = new object();

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

        public bool IsAlive()
        {
            return true;
        }

        public Version Version
        {
            get { return new Version(Versioning.Major, Versioning.Minor); }
        }
    }
}
