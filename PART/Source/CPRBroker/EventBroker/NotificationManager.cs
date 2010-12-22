using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.DAL.Events;

namespace CprBroker.EventBroker
{
    public class NotificationManager
    {
        public static void Enqueue(Guid personUuid)
        {
            using (var dataContext = new EventBrokerDataContext())
            {
                var pp = new DataChangeEvent()
                {
                    DataChangeEventId = Guid.NewGuid(),
                    UUID = personUuid,
                    ReceivedDate = DateTime.Now
                };
                dataContext.DataChangeEvents.InsertOnSubmit(pp);
                dataContext.SubmitChanges();
            }
        }
    }
}
