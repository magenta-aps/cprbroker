using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.EventBroker.Notifications
{
    public partial class CprBrokerEventEnqueuer : PeriodicTaskExecuter
    {
        protected readonly EventsService.Events EventsService = new CprBroker.EventBroker.EventsService.Events();

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
            this.EventsService.ApplicationHeaderValue = new CprBroker.EventBroker.EventsService.ApplicationHeader()
            {
                ApplicationToken = Constants.BaseApplicationToken.ToString(),
                UserToken = ""
            };
            this.EventsService.Url = Config.Properties.Settings.Default.EventsServiceUrl;
        }

        

    }
}
