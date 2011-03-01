using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the PersonCivilState table
    /// </summary>
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

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonLifeState>(pls => pls.LifeStatusCodeType);
            loadOptions.LoadWith<PersonLifeState>(pls => pls.Effect);
        }
    }
}
