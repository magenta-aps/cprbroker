using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Schemas;
using CprBroker.PartInterface.Tracking;

namespace CprBroker.Slet
{
    public class RemovePersonItem
    {
        public Guid PersonUuid { get; set; }
        public string PNR { get; set; }
        public PersonRemovalDecision removalDecision { get; set; }
        public bool forceRemoval { get; set; }

        public RemovePersonItem(PersonIdentifier personIdentifier, bool forceRemoval = false)
        {
            PersonUuid = personIdentifier.UUID.Value;
            PNR = personIdentifier.CprNumber;
            this.forceRemoval = forceRemoval;
        }

        public RemovePersonItem(Guid guid, string pnr, bool forceRemoval = false)
        {
            PersonUuid = guid;
            PNR = pnr;
            this.forceRemoval = forceRemoval;
        }

        public PersonIdentifier ToPersonIdentifier()
        {
            return new PersonIdentifier()
            {
                UUID = PersonUuid,
                CprNumber = PNR,
            };
        }
    }
}
