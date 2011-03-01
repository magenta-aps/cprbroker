using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CprBroker.Utilities;

namespace CprBroker.EventBroker.Notifications
{
    /// <summary>
    /// Base class for getting information for Event Broker from Cpr Broker
    /// </summary>
    public partial class CprBrokerEventEnqueuer : PeriodicTaskExecuter
    {
        protected readonly EventsService.Events EventsService = new CprBroker.EventBroker.EventsService.Events();

        private EventBroker.EventsService.ApplicationHeader ApplicationHeader = new CprBroker.EventBroker.EventsService.ApplicationHeader();

        public CprBrokerEventEnqueuer()
        {
            InitializeComponent();
        }

        public CprBrokerEventEnqueuer(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            InitializeEventsService();
        }

        protected override void OnBeforeStart()
        {
            InitializeEventsService();
        }

        private void InitializeEventsService()
        {
            ApplicationHeader = new CprBroker.EventBroker.EventsService.ApplicationHeader()
            {
                ApplicationToken = Constants.EventBrokerApplicationToken.ToString(),
                UserToken = ""
            };

            EventsService.ApplicationHeaderValue = ApplicationHeader;
            EventsService.Url = Config.Properties.Settings.Default.EventsServiceUrl;
        }

    }
}
