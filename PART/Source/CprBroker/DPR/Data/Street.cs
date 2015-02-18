using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.DPR
{
    public partial class Street
    {
        static Dictionary<int, Street[]> _Streets = new Dictionary<int, Street[]>();
        public static string GetAddressingName(string connectionString, decimal municipalityCode, decimal streetCode)
        {
            var code = connectionString.GetHashCode();
            if (!_Streets.ContainsKey(code))
            {
                using (var dataContext = new LookupDataContext(connectionString))
                {
                    _Streets[code] = dataContext.Streets.ToArray();
                }
            }

            var ret = _Streets[code]
                    .Where(pd =>
                        pd.KOMKOD == municipalityCode
                        && pd.VEJKOD == streetCode)
                    .SingleOrDefault();

            if (ret != null)
                return ret.VEJADNVN;

            return null;
        }
    }
}
