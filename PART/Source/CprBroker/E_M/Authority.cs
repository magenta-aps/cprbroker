using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.E_M
{
    public partial class Authority
    {
        public static string ToAuthorityName(Authority authority)
        {
            if (authority != null)
            {
                if (!string.IsNullOrEmpty(authority.AuthorityName))
                {
                    return authority.AuthorityName;
                }
                else
                {
                    throw new ArgumentException("Authority.AuthorityName cannot be null or empty");
                }
            }
            else
            {
                return null;
            }
        }
    }
}
