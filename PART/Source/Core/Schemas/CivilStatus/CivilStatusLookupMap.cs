using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;


namespace CprBroker.Schemas.Part
{
    public class CivilStatusLookupMap : LookupMap<char, CivilStatusKodeType>
    {
        public CivilStatusLookupMap()
        {
            AddMapping(MaritalStatus.Deceased, CivilStatusKodeType.Ugift);
            AddMapping(MaritalStatus.Widow, CivilStatusKodeType.Enke);
            AddMapping(MaritalStatus.Divorced, CivilStatusKodeType.Skilt);
            AddMapping(MaritalStatus.Married, CivilStatusKodeType.Gift);
            AddMapping(MaritalStatus.LongestLivingPartner, CivilStatusKodeType.Laengstlevende);
            AddMapping(MaritalStatus.AbolitionOfRegisteredPartnership, CivilStatusKodeType.OphaevetPartnerskab);
            AddMapping(MaritalStatus.RegisteredPartnership, CivilStatusKodeType.RegistreretPartner);
            AddMapping(MaritalStatus.Unmarried, CivilStatusKodeType.Ugift);
        }

    }
}
