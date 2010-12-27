using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;

namespace CprBroker.DAL
{
    public partial class MaritalStatusType
    {
        private static List<KeyValuePair<string, MaritalStatusCodeType>> _Values = new List<KeyValuePair<string, MaritalStatusCodeType>>();

        private static void LoadValues()
        {
            lock (_Values)
            {
                if (_Values.Count == 0)
                {
                    _Values.AddRange(CprBroker.Schemas.Util.Enums.GetEnumValues<MaritalStatusCodeType>());
                }
            }
        }

        public static int GetCode(MaritalStatusCodeType maritalStatusType)
        {
            LoadValues();
            var code = (from kvp in _Values where kvp.Value == maritalStatusType select kvp.Key).FirstOrDefault();
            return Convert.ToInt32(code);
        }

        public static MaritalStatusCodeType GetEnum(int code)
        {
            LoadValues();
            string sCode = code.ToString();
            return (from kvp in _Values where kvp.Key.Equals(sCode) select kvp.Value).FirstOrDefault();
        }
    }
}
