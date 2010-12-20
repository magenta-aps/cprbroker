using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL.Part
{
    public partial class MaritalStatusType
    {
        private static List<KeyValuePair<string, Schemas.Part.Enums.MaritalStatus>> _PartValues = new List<KeyValuePair<string, Schemas.Part.Enums.MaritalStatus>>();

        private static void LoadPartValues()
        {
            lock (_PartValues)
            {
                if (_PartValues.Count == 0)
                {
                    _PartValues.AddRange(CPRBroker.Schemas.Util.Enums.GetEnumValues<Schemas.Part.Enums.MaritalStatus>());
                }
            }
        }

        public static int? GetPartCode(Schemas.Part.Enums.MaritalStatus? maritalStatusCode)
        {
            if (maritalStatusCode.HasValue)
            {
                LoadPartValues();
                var code = (from kvp in _PartValues where kvp.Value == maritalStatusCode select kvp.Key).FirstOrDefault();
                return Convert.ToInt32(code);
            }
            return null;
        }

        public static Schemas.Part.Enums.MaritalStatus? GetPartMaritalStatus(int? code)
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
