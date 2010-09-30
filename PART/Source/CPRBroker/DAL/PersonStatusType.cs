using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.DAL
{
    public partial class PersonStatusType
    {
        private static List<KeyValuePair<string, PersonCivilRegistrationStatusCodeType>> _Values = new List<KeyValuePair<string, PersonCivilRegistrationStatusCodeType>>();

        private static void LoadValues()
        {
            lock (_Values)
            {
                if (_Values.Count == 0)
                {
                    _Values.AddRange(CPRBroker.Schemas.Util.Enums.GetEnumValues<PersonCivilRegistrationStatusCodeType>());
                }
            }
        }

        public static string GetCode(PersonCivilRegistrationStatusCodeType personStatusType)
        {
            LoadValues();
            return (from kvp in _Values where kvp.Value == personStatusType select kvp.Key).FirstOrDefault();
        }

        public static PersonCivilRegistrationStatusCodeType GetEnum(string code)
        {
            LoadValues();
            return (from kvp in _Values where kvp.Key.Equals(code) select kvp.Value).FirstOrDefault();
        }
    }
}
