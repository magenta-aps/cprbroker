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

        public CprServices.SearchMethod ToSearchMethod()
        {
            return new CprServices.SearchMethod(Properties.Resources.PnrLookup) { Name = Name };
        }

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

        public static readonly ServiceInfo StamPlus_Local = new ServiceInfo()
        {
            Name = "Stam+",
            UUID = "7baec3a5-26a5-4922-80fd-37f3bad3dfcb",
            Path = "/service/CPRLookup/CPRLookup/2"
        };

        public static readonly ServiceInfo NAVNE3_Local = new ServiceInfo()
        {
            Name = "navne3",
            UUID = "e3f26293-803e-4b8f-8a15-674a4f8abaad",
            Path = "/service/CPRLookup/CPRLookup/2"
        };

        public static readonly ServiceInfo FamilyPlus_Local = new ServiceInfo()
        {
            Name = "Familie+",
            UUID = "a34cf599-26e4-4609-bfbd-2490d3d1c3ab",
            Path = "/service/CPRLookup/CPRLookup/2"
        };

    }
}
