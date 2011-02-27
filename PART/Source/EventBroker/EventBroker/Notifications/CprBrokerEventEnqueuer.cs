using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CprBroker.Utilities;

namespace CprBroker.EventBroker.Notifications
{
    public partial class CprBrokerEventEnqueuer : PeriodicTaskExecuter
    {
        protected readonly EventsService.EventsClient EventsService
            = new CprBroker.EventBroker.EventsService.EventsClient(
                new System.ServiceModel.WSHttpBinding(),
                new System.ServiceModel.EndpointAddress(Config.Properties.Settings.Default.EventsServiceUrl)
            );
        protected EventBroker.EventsService.ApplicationHeader ApplicationHeader = new CprBroker.EventBroker.EventsService.ApplicationHeader();

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
        }

    }
}
