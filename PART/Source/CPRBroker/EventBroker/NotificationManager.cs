using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.EventBroker.DAL;

namespace CprBroker.EventBroker
{
    public class NotificationManager
    {
        public static void Enqueue(Guid personUuid)
        {
            using (var dataContext = new EventBrokerDataContextDataContext())
            {
                var pp = new DAL.DataChangeEvent()
                {
                    DataChangeEventId = Guid.NewGuid(),
                    UUID = personUuid,
                    ReceivedData = DateTime.Now
                };
                dataContext.DataChangeEvents.InsertOnSubmit(pp);
                dataContext.SubmitChanges();
            }
        }
    }
}
