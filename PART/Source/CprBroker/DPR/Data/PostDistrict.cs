using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.DPR
{
    public partial class PostDistrict
    {
        private static Dictionary<int, PostDistrict[]> _PostDistricts = new Dictionary<int, PostDistrict[]>();
        public static decimal? GetPostCode(string connectionString, decimal municipalityCode, decimal streetCode, string houseNumber)
        {
            var databaseCode = connectionString.GetHashCode();

            if (!_PostDistricts.ContainsKey(databaseCode))
            {
                // Pre fill from database
                using (var dataContext = new LookupDataContext(connectionString))
                {
                    _PostDistricts[databaseCode] = dataContext.PostDistricts.ToArray();
                }
            }

            // Decide even or odd house number
            var evenOdd = EvenOdd.Even;
            if (!string.IsNullOrEmpty(houseNumber))
            {
                var digits = Enumerable.Range(0, 10).Select(i => i.ToString()[0]).ToArray();
                var nums = houseNumber.Where(c => digits.Contains(c)).ToArray();
                var num = int.Parse(new string(nums));
                if (num % 2 == 1)
                    evenOdd = EvenOdd.Odd;
            }

            var ret = _PostDistricts[databaseCode].Where(pd =>
                pd.KOMKOD == municipalityCode
                && pd.VEJKOD == streetCode
                && (
                    houseNumber == null
                    ||
                    (houseNumber.CompareTo(pd.HUSNRFRA) >= 0 && houseNumber.CompareTo(pd.HUSNRTIL) <= 0)
                    )
                && pd.LIGEULIGE == evenOdd
                )
                .SingleOrDefault();

            if (ret != null)
                return ret.POSTNR;
            return null;

        }
    }
}
