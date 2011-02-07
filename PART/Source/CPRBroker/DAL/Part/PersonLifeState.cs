using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonLifeState
    {
        public static LivStatusType ToXmlType(PersonLifeState db)
        {
            if (db != null)
            {
                return new LivStatusType()
                {
                    LivStatusKode = (LivStatusKodeType)db.LifeStatusCodeTypeId,
                    TilstandVirkning = Effect.ToTilstandVirkningType(db.Effect),
                };
            }
            return null;
        }

        public static PersonLifeState FromXmlType(LivStatusType oio)
        {
            if (oio != null)
            {
                return new PersonLifeState()
                {
                    LifeStatusCodeTypeId = (int)oio.LivStatusKode,
                    Effect = Effect.FromTilstandVirkningType(oio.TilstandVirkning)
                };
            }
            return null;
        }
    }
}
