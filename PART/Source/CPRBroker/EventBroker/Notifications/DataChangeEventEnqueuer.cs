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

        protected override void  PerformTimerAction()
        {
            using (var dataContext = new DAL.EventBrokerDataContext())
            {
                DateTime startDate = DateTime.Now;
                //DateTime endDate = startDate - PollInterval;
                // TODO: Get data change events from CPR Broker
                //dataContext.EnqueueDataChangeEventNotifications(startDate, endDate, DateTime.Now, (int)DAL.SubscriptionType.SubscriptionTypes.DataChange);
            }
        }
    }
}
