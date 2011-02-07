using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonCivilState
    {
        public static CivilStatusType ToXmlType(PersonCivilState db)
        {
            if (db != null)
            {
                return new CivilStatusType()
                {
                    CivilStatusKode = (CivilStatusKodeType)db.CivilStatusCodeTypeId,
                    TilstandVirkning = Effect.ToTilstandVirkningType(db.Effect),
                };
            }
            return null;
        }

        public static PersonCivilState FromXmlType(CivilStatusType oio)
        {
            if (oio != null)
            {
                return new PersonCivilState()
                {
                    CivilStatusCodeTypeId = (int)oio.CivilStatusKode,
                    Effect = Effect.FromTilstandVirkningType(oio.TilstandVirkning)
                };
            }
            return null;
        }
    }
}
