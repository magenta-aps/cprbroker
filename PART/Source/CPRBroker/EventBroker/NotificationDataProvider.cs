using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CprBroker.EventBroker.DAL;

namespace CprBroker.EventBroker
{
    public class NotificationDataProvider:INotificationManager
    {
        #region INotificationManager Members

        public bool Enqueue(Guid personUuid)
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
            return true;
        }

        #endregion

        #region IDataProvider Members

        public bool IsAlive()
        {
            return true;
        }

        public Version Version
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
