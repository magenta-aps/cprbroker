using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL.Part
{
    public partial class MaritalStatusType
    {
        private static List<KeyValuePair<string, Schemas.Part.CivilStatusKodeType>> _PartValues = new List<KeyValuePair<string, Schemas.Part.CivilStatusKodeType>>();

        private static void LoadPartValues()
        {
            lock (_PartValues)
            {
                if (_PartValues.Count == 0)
                {
                    _PartValues.AddRange(CprBroker.Schemas.Util.Enums.GetEnumValues<Schemas.Part.CivilStatusKodeType>());
                }
            }
        }

        public static int? GetPartCode(Schemas.Part.CivilStatusKodeType? maritalStatusCode)
        {
            if (maritalStatusCode.HasValue)
            {
                LoadPartValues();
                var code = (from kvp in _PartValues where kvp.Value == maritalStatusCode select kvp.Key).FirstOrDefault();
                return Convert.ToInt32(code);
            }
            return null;
        }

        public static Schemas.Part.CivilStatusKodeType? GetPartMaritalStatus(int? code)
        {
            if (code.HasValue)
            {
                LoadPartValues();
                string sCode = code.ToString();
                return (from kvp in _PartValues where kvp.Key.Equals(sCode) select kvp.Value).FirstOrDefault();
            }
            return null;
        }
    }
}
