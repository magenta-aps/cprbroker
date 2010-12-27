using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;

namespace CprBroker.EventBroker
{
    public interface INotificationManager:IDataProvider
    {
        bool Enqueue(Guid personUuid);
    }
}
