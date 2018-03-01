using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Schemas;
using CprBroker.PartInterface.Tracking;
using System.Threading;
using CprBroker.Slet;
using CprBroker.Engine;

namespace CprBroker.Providers.Slet
{
    public class RemovePersonDataProvider : IRemovePersonDataProvider
    {
        #region IDataProvider members
        public Version Version
        {
            get
            {
                return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Major);
            }
        }

        public bool IsAlive()
        {
            return true;
        }
        #endregion

        public bool RemovePerson(PersonIdentifier personIdentifier)
        {
            PersonRemover personRemover = new PersonRemover();
            var brokerContext = BrokerContext.Current;
            RemovePersonItem removePersonItem = new RemovePersonItem(personIdentifier);
            //personRemover.RemovePerson(brokerContext, removePersonItem, )
            return true;
        }
    }
}
