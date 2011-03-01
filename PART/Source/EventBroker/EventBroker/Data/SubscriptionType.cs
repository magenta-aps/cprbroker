using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.EventBroker.Data
{
    partial class SubscriptionType
    {
        public enum SubscriptionTypes
        {
            DataChange = 1,
            Birthdate = 2
        }
    }
}
