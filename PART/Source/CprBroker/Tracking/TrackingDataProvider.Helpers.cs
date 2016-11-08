using CprBroker.DBR;
using CprBroker.Engine;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    partial class TrackingDataProvider
    {
        async Task<bool> RemoveSubscription(BrokerContext brokerContext, IPutSubscriptionDataProvider prov, PersonIdentifier personIdentifier)
        {
            BrokerContext.Current = brokerContext;
            if (prov.IsSharingSubscriptions) // Shared subsciptions should not be deleted because they can cause other systems to stop getting updates
                return true;
            else
                return prov.RemoveSubscription(personIdentifier);
        }

        async Task<bool> DeletePersonFromAllExtracts(BrokerContext brokerContext, PersonIdentifier personIdentifier)
        {
            BrokerContext.Current = brokerContext;
            Extract.DeletePersonFromAllExtracts(personIdentifier.CprNumber);
            return true;
        }

        async Task<bool> DeletePerson(BrokerContext brokerContext, PersonIdentifier personIdentifier)
        {
            BrokerContext.Current = brokerContext;
            CprBroker.Data.Part.Person.Delete(personIdentifier);
            return true;
        }

        async Task<bool> DeletePersonFromDBR(BrokerContext brokerContext, DbrQueue dbr, PersonIdentifier personIdentifier)
        {
            using (var dbrDataContext = new DPRDataContext(dbr.ConnectionString))
            {
                BrokerContext.Current = brokerContext;
                CprConverter.DeletePersonRecords(personIdentifier.CprNumber, dbrDataContext);
                dbrDataContext.SubmitChanges();
                return true;
            }
        }



    }
}
