using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL
{
    /// <summary>
    /// Person detail level
    /// </summary>
    public partial class DetailLevel
    {
        public enum DetailLevelType
        {
            Number = 1,
            Name = 2,
            NameAndAddress = 3,
            BasicData = 4,
            FullData = 5
        }
    }
}
