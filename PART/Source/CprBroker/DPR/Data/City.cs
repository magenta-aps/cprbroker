using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.DPR
{
    public partial class City : IHouseLookup
    {
        public static string GetCityName(string connectionString, decimal municipalityCode, decimal streetCode, string houseNumber)
        {
            return HouseLookupHelper<City>
                .GetPostValue<string>(connectionString, municipalityCode, streetCode, houseNumber, dc => dc.Cities, o => o.BYNVN);
        }
    }
}
