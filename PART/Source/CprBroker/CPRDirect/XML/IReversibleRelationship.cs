using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public interface IReversibleRelationship
    {
        string PNR { get; set; }
        string RelationPNR { get; set; }
    }
}
