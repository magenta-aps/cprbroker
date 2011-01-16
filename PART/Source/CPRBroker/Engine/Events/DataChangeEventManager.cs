using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Events
{
    public class DataChangeEventManager : IDataChangeEventManager
    {
        public Schemas.Part.Events.DataChangeEventInfo[] DequeueEvents(int maxCount)
        {
            using (var dataContext = new DAL.Events.DataChangeEventDataContext())
            {
                var events = from ev in dataContext.DataChangeEvents
                             orderby ev.ReceivedDate
                             select new Schemas.Part.Events.DataChangeEventInfo()
                             {
                                 EventId = ev.DataChangeEventId,
                                 PersonUuid = ev.PersonUuid,
                                 ReceivedDate = ev.ReceivedDate
                             };
                return events.Take(maxCount).ToArray();
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
