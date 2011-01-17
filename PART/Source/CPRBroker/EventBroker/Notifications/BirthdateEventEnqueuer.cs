using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.EventBroker.Notifications
{
    public partial class BirthdateEventEnqueuer : CprBrokerEventEnqueuer 
    {
        public int BatchSize = 10;

        public BirthdateEventEnqueuer()
            : base()
        {
            InitializeComponent();
        }

        public BirthdateEventEnqueuer(IContainer container)
            : base(container)
        {
            container.Add(this);
            InitializeComponent();
        }

        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            DateTime endToday = DateTime.Today.AddDays(1);

            // Take care of the case if service is started at the very end of a day
            TimeSpan interval = endToday - DateTime.Now;
            if (interval < TimeSpan.FromMinutes(1))
                interval = TimeSpan.FromMinutes(1);

            return interval;
        }

        protected override void PerformTimerAction()
        {
            SynchronisePersonBirthdates();
            EnqueueBirthdateSvents();
        }

        private void SynchronisePersonBirthdates()
        {
            bool morePersons = true;
            Guid? lastPersonGuid = null;
            while (morePersons)
            {
                var personBirthdates = EventsService.GetPersonBirthdates(lastPersonGuid, BatchSize);

                if (personBirthdates.Length > 0)
                {
                    lastPersonGuid = personBirthdates[personBirthdates.Length - 1].PersonUuid;
                }
                morePersons = personBirthdates.Length < BatchSize;

                using (var dataContext = new DAL.EventBrokerDataContext())
                {
                    var newPersons =
                    (
                        from pb in personBirthdates
                        where !(from dpb in dataContext.PersonBirthdates select dpb.PersonUuid).Contains(pb.PersonUuid)
                        select new DAL.PersonBirthdate()
                        {
                            PersonUuid = pb.PersonUuid,
                            Birthdate = pb.Birthdate
                        }
                    );

                    dataContext.PersonBirthdates.InsertAllOnSubmit(newPersons);
                    dataContext.SubmitChanges();
                }
            }
        }

        private void EnqueueBirthdateSvents()
        {
            using (var dataContext = new DAL.EventBrokerDataContext())
            {
                DateTime today = DateTime.Today;

                foreach (var subscription in dataContext.Subscriptions)
                {
                    try
                    {
                        dataContext.EnqueueBirthdateEventNotifications(subscription.SubscriptionId, today);
                    }
                    catch (Exception ex)
                    {
                        string message = string.Format("Failed to enqueue birthdate notifications for {0}", subscription.SubscriptionId);
                        CprBroker.Engine.Local.Admin.LogException(ex, message);
                    }
                }
            }
        }
    }
}
