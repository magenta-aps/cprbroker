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
            AddMapping('D', CivilStatusKodeType.Ugift);
            AddMapping('E', CivilStatusKodeType.Enke);
            AddMapping('F', CivilStatusKodeType.Skilt);
            AddMapping('G', CivilStatusKodeType.Gift);
            AddMapping('L', CivilStatusKodeType.Laengstlevende);
            AddMapping('O', CivilStatusKodeType.OphaevetPartnerskab);
            AddMapping('P', CivilStatusKodeType.RegistreretPartner);
            AddMapping('U', CivilStatusKodeType.Ugift);
        }

    }
}
