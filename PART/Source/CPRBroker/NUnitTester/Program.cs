using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CprBroker.NUnitTester
{
    public class Program
    {
        public static void Main()
        {
            var o = new Subscriptions.ChangeSubscriptionType();
            o.NotificationChannel = new Subscriptions.FileShareChannelType() { Path = "c:\\" };

            var xml = CprBroker.DAL.Utilities.SerializeObject(o);
            object s = "";
            

        }
    }
}