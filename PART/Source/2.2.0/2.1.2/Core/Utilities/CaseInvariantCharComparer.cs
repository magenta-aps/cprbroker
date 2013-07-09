using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Utilities
{
    public class CaseInvariantCharComparer : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
        {
            return x.ToString().ToLower().Equals(y.ToString().ToLower());
        }

        public int GetHashCode(char obj)
        {
            return obj.GetHashCode();
        }
    }
}
