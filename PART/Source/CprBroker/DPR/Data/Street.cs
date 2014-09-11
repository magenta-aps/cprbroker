using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.DPR
{
    public partial class Street
    {
        public static string GetAddressingName(string connectionString, decimal municipalityCode, decimal streetCode)
        {
            using (var dataContext = new LookupDataContext(connectionString))
            {
                var ret = dataContext.Streets.Where(pd =>
                    pd.KOMKOD == municipalityCode
                    && pd.VEJKOD == streetCode)
                    .SingleOrDefault();

                if (ret != null)
                    return ret.VEJADNVN;

                return null;
            }
        }
    }
}
