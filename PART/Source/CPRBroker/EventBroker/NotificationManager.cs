using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.EventBroker.DAL;

namespace CprBroker.EventBroker
{
    // TODO: Merge with NotificationEngine or Manager.Subscriptions
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
