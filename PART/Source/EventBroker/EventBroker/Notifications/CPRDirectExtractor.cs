using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.EventBroker.Notifications
{
    public class CPRDirectExtractor : PeriodicTaskExecuter
    {
        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return TimeSpan.FromDays(1);
        }

        protected override void PerformTimerAction()
        {
            ExtractManager.ImportDataProviderFolders();
        }

    }
}
