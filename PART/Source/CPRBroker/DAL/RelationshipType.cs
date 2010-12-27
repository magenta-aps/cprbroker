using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL
{
    public partial class RelationshipType
    {
        public enum RelationshipTypes
        {
            ParentChild = 1,
            Marital = 2,
            Custody=3
        }
    }
}
