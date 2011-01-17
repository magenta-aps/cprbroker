using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.EventBroker.Notifications
{
    public partial class DataChangeEventEnqueuer : PeriodicTaskExecuter
    {
        private EventsService.Events EventsService = new CprBroker.EventBroker.EventsService.Events();
        public int BatchSize = 10;

        public DataChangeEventEnqueuer()
        {
            InitializeComponent();

            InitializeEventsService();
        }

        public DataChangeEventEnqueuer(IContainer container)
        {
            container.Add(this);
            InitializeComponent();

            InitializeEventsService();
        }

        private void InitializeEventsService()
        {
            this.EventsService.ApplicationHeaderValue = new CprBroker.EventBroker.EventsService.ApplicationHeader()
            {
                ApplicationToken = Constants.BaseApplicationToken.ToString(),
                UserToken = ""
            };
        }

        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return TimeSpan.FromMinutes(1);
        }

        protected override void  PerformTimerAction()
        {
            bool moreChangesExist = true;

            while (moreChangesExist)
            {
                var changedPeople = EventsService.DequeueDataChangeEvents(BatchSize);
                moreChangesExist = changedPeople.Length == BatchSize;

                using (var dataContext = new DAL.EventBrokerDataContext())
                {
                    var dbObjects = Array.ConvertAll<EventsService.DataChangeEventInfo, DAL.DataChangeEvent>(
                        changedPeople,
                        p => new DAL.DataChangeEvent()
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

                    dataContext.EnqueueDataChangeEventNotifications(DateTime.Now, (int)DAL.SubscriptionType.SubscriptionTypes.DataChange);

                    //TODO: Move this logic to above stored procedure
                    dataContext.DataChangeEvents.DeleteAllOnSubmit(dbObjects);
                    dataContext.SubmitChanges();
                }
            }
        }
    }
}
