using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Util
{
    public class Converters
    {
        public static bool ToSpecielVejkodeIndikator(string streetCode)
        {
            int intCode ;
            if(int.TryParse(streetCode,out intCode))
            {
                return ToSpecielVejkodeIndikator(intCode);
            }
            else
            {
                return false;
            }
        }
        public static bool ToSpecielVejkodeIndikator(int streetCode)
        {
            return ToSpecielVejkodeIndikator((decimal)streetCode);
        }

        public static bool ToSpecielVejkodeIndikator(decimal streetCode)
        {
            if (streetCode >= 0 && streetCode <= 9999)
            {
                return streetCode >= 9900;
            }
            else
            {
                throw new ArgumentException(string.Format("RoadCode <{0}> must be between 0 and 9999", streetCode));
            }
        }
    }
}
