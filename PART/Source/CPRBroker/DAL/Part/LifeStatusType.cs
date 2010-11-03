using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL.Part
{
    public partial class LifeStatusType
    {
        private static List<KeyValuePair<string, Schemas.Part.Enums.LifeStatus>> _PartValues = new List<KeyValuePair<string, Schemas.Part.Enums.LifeStatus>>();

        private static void LoadPartValues()
        {
            lock (_PartValues)
            {
                if (_PartValues.Count == 0)
                {
                    _PartValues.AddRange(CPRBroker.Schemas.Util.Enums.GetEnumValues<Schemas.Part.Enums.LifeStatus>());
                }
            }
        }

        public static int GetPartCode(Schemas.Part.Enums.LifeStatus lifeStatusCode)
        {
            LoadPartValues();
            var code = (from kvp in _PartValues where kvp.Value == lifeStatusCode select kvp.Key).FirstOrDefault();
            return Convert.ToInt32(code);
        }

        public static Schemas.Part.Enums.LifeStatus GetPartLifeStatus(int code)
        {
            LoadPartValues();
            string sCode = code.ToString();
            return (from kvp in _PartValues where kvp.Key.Equals(sCode) select kvp.Value).FirstOrDefault();
        }
    }
}
