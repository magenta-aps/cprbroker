using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.ServicePlatform
{
    public class ServiceInfo
    {
        public string Name { get; private set; }
        public string UUID { get; private set; }
        public string Path { get; private set; }

        private ServiceInfo()
        { }

        public static readonly ServiceInfo CPRSubscription = new ServiceInfo()
        {
            Name = "CPR Subscription",
            UUID = "9cdccc2f-3243-11e2-8fef-d4bed98c5934",
            Path = "/service/CPRSubscription/CPRSubscription/1/"
        };

        public static readonly ServiceInfo ADRSOG1 = new ServiceInfo() 
        {
            Name = "ADRSOG1",
            UUID = "6538f113-b1e6-4b21-8fe1-28b7bf78a1bd",
            Path = "/service/CPRLookup/CPRLookup/2"
        };

        public static readonly ServiceInfo ForwardToCprService = new ServiceInfo()
        {
            Name = "CPR Service",
            UUID = "8ee213c7-25a9-11e2-8770-d4bed98c63db",
            Path = "/service/CPRService/CPRService/1"
        };

    }
}
