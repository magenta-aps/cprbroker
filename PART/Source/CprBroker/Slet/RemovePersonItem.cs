using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Schemas;
using CprBroker.PartInterface.Tracking;

namespace CprBroker.Slet
{
    class RemovePersonItem
    {
        public Guid PersonUuid { get; set; }
        public string PNR { get; set; }
        public PersonRemovalDecision removalDecision { get; set; }

        public RemovePersonItem(PersonIdentifier personIdentifier)
        {
            PersonUuid = personIdentifier.UUID.Value;
            PNR = personIdentifier.CprNumber;
        }

        public RemovePersonItem(Guid guid, string pnr)
        {
            PersonUuid = guid;
            PNR = pnr;
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
