using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Schemas;
using CprBroker.PartInterface.Tracking;
using CprBroker.Engine;
using System.Threading;

namespace CprBroker.Slet
{
    public class RemovePersonDataProvider : IPartRemovePersonDataProvider
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
            PersonRemover personRemovalManager = new PersonRemover();
            var brokerContext = BrokerContext.Current;
            RemovePersonItem removePersonItem = new RemovePersonItem(personIdentifier);
            //personRemovalManager.RemovePerson(brokerContext, removePersonItem, )
            return true;
        }
    }
}
