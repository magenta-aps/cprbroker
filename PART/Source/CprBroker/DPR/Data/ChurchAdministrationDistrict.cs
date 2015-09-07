using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.DPR
{
    public partial class ChurchAdministrationDistrict : IHouseLookup
    {
        public static decimal GetAuthorityCode(string connectionString, decimal municipalityCode, decimal streetCode, string houseNumber)
        {
            return HouseLookupHelper<ChurchAdministrationDistrict>
                .GetPostValue<decimal>(connectionString, municipalityCode, streetCode, houseNumber, dc => dc.ChurchAdministrationDistricts, o => o.KOMKOD);
        }
    }
}
