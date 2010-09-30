using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.DAL
{
    public partial class Gender
    {
        private static List<KeyValuePair<string, PersonGenderCodeType>> _Values = new List<KeyValuePair<string, PersonGenderCodeType>>();

        private static void LoadValues()
        {
            lock (_Values)
            {
                if (_Values.Count == 0)
                {
                    _Values.AddRange(CPRBroker.Schemas.Util.Enums.GetEnumValues<PersonGenderCodeType>());
                }
            }
        }

        public static int GetCode(PersonGenderCodeType personGenderCode)
        {
            LoadValues();
            var code = (from kvp in _Values where kvp.Value == personGenderCode select kvp.Key).FirstOrDefault();
            return Convert.ToInt32(code);
        }

        public static PersonGenderCodeType GetEnum(int code)
        {
            LoadValues();
            string sCode = code.ToString();
            return (from kvp in _Values where kvp.Key.Equals(sCode) select kvp.Value).FirstOrDefault();
        }
    }
}
