using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL
{
    /// <summary>
    /// Interface that joins time limited relationshipd together
    /// </summary>
    public interface ITimedRelationship
    {
        TimedRelationship TimedRelationship { get; }
    }

    public partial class CustodyRelationship : ITimedRelationship
    {
    }

    public partial class MaritalRelationship : ITimedRelationship
    {
    }
}
