using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.EventBroker.Data
{
    /// <summary>
    /// Represents the SubscriptionType table
    /// </summary>
    partial class SubscriptionType
    {
        public enum SubscriptionTypes
        {
            DataChange = 1,
            Birthdate = 2
        }
    }
}
