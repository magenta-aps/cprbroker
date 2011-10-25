using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public class LookupMap<TKey, TValue>
    {
        private  Dictionary<TKey, TValue> Mappings = new Dictionary<TKey, TValue>();

        protected void AddMapping(TKey key, TValue value)
        {
            Mappings[key] = value;
        }

        public bool ContainsKey(TKey key)
        {
            return Mappings.ContainsKey(key);
        }

        public TValue Map(TKey c)
        {
            return Mappings[c];
        }

        public TKey UnMap(TValue status)
        {
            return Mappings.Where((kvp) => kvp.Value.Equals(status)).First().Key;
        }
    }

    public class CivilStatusCodes : LookupMap<char, CivilStatusKodeType>
    {
        public CivilStatusCodes()
        {
            //TODO: Handle 'D' (dead) 
            //TODO: See what fits into CivilStatusKodeType.Separeret

            //AddMapping('D',CivilStatusKodeType.??? );
            AddMapping('E', CivilStatusKodeType.Enke);
            AddMapping('F', CivilStatusKodeType.Skilt);
            AddMapping('G', CivilStatusKodeType.Gift);
            AddMapping('L', CivilStatusKodeType.Laengstlevende);
            AddMapping('O', CivilStatusKodeType.OphaevetPartnerskab);
            AddMapping('P', CivilStatusKodeType.RegistreretPartner);
            AddMapping('U', CivilStatusKodeType.Ugift);
            //AddMapping('???',CivilStatusKodeType.Separeret);
        }

    }
}
