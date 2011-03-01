using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    public partial class LifecycleStatus
    {
        private static List<KeyValuePair<string, LivscyklusKodeType>> _Values = new List<KeyValuePair<string, LivscyklusKodeType>>();

        private static void LoadValues()
        {
            lock (_Values)
            {
                if (_Values.Count == 0)
                {
                    _Values.AddRange(CprBroker.Schemas.Util.Enums.GetEnumValues<LivscyklusKodeType>());
                }
            }
        }

        public static int GetCode(LivscyklusKodeType personStatusType)
        {
            LoadValues();
            return (from kvp in _Values where kvp.Value == personStatusType select int.Parse(kvp.Key)).FirstOrDefault();
        }

        public static LivscyklusKodeType GetEnum(int code)
        {
            LoadValues();
            return (from kvp in _Values where kvp.Key.Equals(code.ToString()) select kvp.Value).FirstOrDefault();
        }
    }
}
