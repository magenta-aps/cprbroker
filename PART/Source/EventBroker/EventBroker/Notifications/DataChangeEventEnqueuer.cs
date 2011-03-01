using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.EventBroker.Notifications
{
    /// <summary>
    /// Gets data change events for Event Broker from Cpr Broker
    /// </summary>
    public partial class DataChangeEventEnqueuer : CprBrokerEventEnqueuer
    {
        public int BatchSize = 10;

        public DataChangeEventEnqueuer()
        {
            InitializeComponent();
        }

        public DataChangeEventEnqueuer(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return TimeSpan.FromMinutes(1);
        }

        protected override void PerformTimerAction()
        {
            bool moreChangesExist = true;

            while (moreChangesExist)
            {
                var resp = EventsService.DequeueDataChangeEvents(BatchSize);
                var changedPeople = resp.Item;
                moreChangesExist = changedPeople.Length == BatchSize;

                using (var dataContext = new Data.EventBrokerDataContext())
                {
                    var dbObjects = Array.ConvertAll<EventsService.DataChangeEventInfo, Data.DataChangeEvent>(
                        changedPeople,
                        p => new Data.DataChangeEvent()
                        {
                            DataChangeEventId = p.EventId,
                            DueDate = p.ReceivedDate,
                            PersonUuid = p.PersonUuid,
                            ReceivedDate = DateTime.Now
                        }
                    );

                    dataContext.DataChangeEvents.InsertAllOnSubmit(dbObjects);
                    dataContext.SubmitChanges();

                    DateTime startDate = DateTime.Now;

                    dataContext.EnqueueDataChangeEventNotifications(DateTime.Now, (int)Data.SubscriptionType.SubscriptionTypes.DataChange);

                    //TODO: Move this logic to above stored procedure
                    dataContext.DataChangeEvents.DeleteAllOnSubmit(dbObjects);
                    dataContext.SubmitChanges();
                }
            }
        }
    }
}
